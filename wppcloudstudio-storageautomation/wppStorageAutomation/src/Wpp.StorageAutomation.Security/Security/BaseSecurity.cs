// <copyright file="BaseSecurity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Wpp.StorageAutomation.Entities.Models;
using Wpp.StorageAutomation.Security.Models;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.Security
{
    /// <summary>
    /// Security base class.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.Security.ISecurity" />
    public class BaseSecurity : IBaseSecurity
    {
        private readonly DbContextOptionsBuilder<WppsqldbContext> optionsBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSecurity"/> class.
        /// </summary>
        public BaseSecurity()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<WppsqldbContext>();
        }

        /// <summary>
        /// Gets the user claims.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="log">The log.</param>
        /// <returns>Returns user claims.</returns>
        public UserGroupResponse GetUserGroups(HttpRequest req, ILogger log)
        {
            UserGroupResponse userResp;

            var task = Task.Run(async () => await this.GetUserClaimsFromToken(req, log));
            if (task.IsFaulted && task.Exception != null)
            {
                throw task.Exception;
            }

            var userGroups = task.Result;
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)userGroups).StatusCode != 200)
            {
                userResp = new UserGroupResponse() { StatusCode = (int)((ObjectResult)userGroups).StatusCode, Message = (string)((ObjectResult)userGroups).Value, HasAccess = false, Groups = null };
            }
            else
            {
                List<string> groups = ((AuthResponse)((ObjectResult)userGroups).Value).UserInfo.Groups.ToList();
                userResp = new UserGroupResponse() { StatusCode = (int)((ObjectResult)userGroups).StatusCode, Message = string.Empty, HasAccess = true, Groups = groups };
            }

            return userResp;
        }

        /// <summary>
        /// Validates the user groups access.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="log">The log.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>Returns has access value.</returns>
        public UserGroupResponse ValidateUserGroupsAccess(HttpRequest req, ILogger log, string productionStoreId)
        {
            List<ProductionStoreEntity> prodstores;
            UserGroupResponse userGroupResp = this.GetUserGroups(req, log);

            if (userGroupResp.HasAccess && userGroupResp.StatusCode == 200)
            {
                if (productionStoreId != null && productionStoreId != string.Empty)
                {
                    prodstores = this.GetAuthorizedProductionStore(userGroupResp.Groups, productionStoreId);
                }
                else
                {
                    prodstores = this.GetAuthorizedProductionStores(userGroupResp.Groups);
                }

                userGroupResp.HasAccess = prodstores.Any();
                if (!prodstores.Any())
                {
                    log.LogInformation("Groups does not match");
                }
            }

            return userGroupResp;
        }

        /// <summary>
        /// Validates the user groups access.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="log">The log.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>Returns has access value.</returns>
        public UserGroupResponse ValidateManagerGroupsAccesstoStores(HttpRequest req, ILogger log, string productionStoreId)
        {
            UserGroupResponse userGroupResp = this.GetUserGroups(req, log);

            if (userGroupResp.HasAccess && userGroupResp.StatusCode == 200)
            {
                    using (var db = new WppsqldbContext(this.optionsBuilder.Options))
                    {
                        ProductionStoreEntity prod = db.ProductionStore.Where(x => x.Id == productionStoreId).FirstOrDefault();
                        var managerGroups = prod.ManagerRoleGroupNames.Split(",");
                        foreach (string group in managerGroups)
                        {
                            if (userGroupResp.Groups.Any(x => x.Equals(group.Trim())))
                            {
                                userGroupResp.HasAccess = true;
                                return userGroupResp;
                            }
                        }

                        userGroupResp.HasAccess = false;
                    }
            }

            return userGroupResp;
        }

        /// <summary>
        /// GetUserClaims.
        /// </summary>
        /// <param name="req">req.</param>
        /// <param name="log">log.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        private async Task<IActionResult> GetUserClaimsFromToken(HttpRequest req, ILogger log)
        {
            var authForClaims = await this.ValidateTokenAndGetUserClaims(req, log);
            return authForClaims;
        }

        /// <summary>
        /// Gets the authorized production stores list.
        /// </summary>
        /// <param name="groupList">The group list.</param>
        /// <returns>Returns authorized production stores.</returns>
        private List<ProductionStoreEntity> GetAuthorizedProductionStores(List<string> groupList)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                var groups = db.ProductionStore.AsEnumerable()
                    .Select(x => new
                    {
                        groupList = x.ManagerRoleGroupNames.Split(",").Concat(x.UserRoleGroupNames.Split(",")).Where(item => groupList.Contains(item)),
                        x.Id,
                        x.Name,
                        x.Region,
                        x.Wipurl,
                        x.WipallocatedSize,
                        x.ArchiveUrl,
                        x.ArchiveAllocatedSize,
                        x.ScaleDownTime,
                        x.ScaleUpTimeInterval,
                        x.MinimumFreeSize,
                        x.MinimumFreeSpace,
                        x.OfflineTime,
                        x.OnlineTime,
                        x.ProductionOfflineTimeInterval,
                        x.ManagerRoleGroupNames,
                        x.UserRoleGroupNames,
                        x.WipkeyName,
                        x.ArchiveKeyName
                    }).Where(x => x.groupList.Any()).Select(x => new ProductionStoreEntity()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Region = x.Region,
                        Wipurl = x.Wipurl,
                        WipallocatedSize = x.WipallocatedSize,
                        ArchiveUrl = x.ArchiveUrl,
                        ArchiveAllocatedSize = x.ArchiveAllocatedSize,
                        ScaleDownTime = x.ScaleDownTime,
                        ScaleUpTimeInterval = x.ScaleUpTimeInterval,
                        MinimumFreeSize = x.MinimumFreeSize,
                        MinimumFreeSpace = x.MinimumFreeSpace,
                        OfflineTime = x.OfflineTime,
                        OnlineTime = x.OnlineTime,
                        ProductionOfflineTimeInterval = x.ProductionOfflineTimeInterval,
                        ManagerRoleGroupNames = x.ManagerRoleGroupNames,
                        UserRoleGroupNames = x.UserRoleGroupNames,
                        WipkeyName = x.WipkeyName,
                        ArchiveKeyName = x.ArchiveKeyName
                    }).OrderBy(x => x.Region).ThenBy(x => x.Name).ToList();

                return groups;
            }
        }

        /// <summary>
        /// Gets the authorized production store.
        /// </summary>
        /// <param name="groupList">The group list.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>Returns true if Productionstore has access.</returns>
        private List<ProductionStoreEntity> GetAuthorizedProductionStore(List<string> groupList, string productionStoreId)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                var groups = db.ProductionStore.AsEnumerable().Where(x => x.Id == productionStoreId)
                    .Select(x => new
                    {
                        groupList = x.ManagerRoleGroupNames.Split(",").Concat(x.UserRoleGroupNames.Split(",")).Where(item => groupList.Contains(item)),
                        x.Id,
                        x.Name,
                        x.Region,
                        x.Wipurl,
                        x.WipallocatedSize,
                        x.ArchiveUrl,
                        x.ArchiveAllocatedSize,
                        x.ScaleDownTime,
                        x.ScaleUpTimeInterval,
                        x.MinimumFreeSize,
                        x.MinimumFreeSpace,
                        x.OfflineTime,
                        x.OnlineTime,
                        x.ProductionOfflineTimeInterval,
                        x.ManagerRoleGroupNames,
                        x.UserRoleGroupNames,
                        x.WipkeyName,
                        x.ArchiveKeyName
                    }).Where(x => x.groupList.Any()).Select(x => new ProductionStoreEntity()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Region = x.Region,
                        Wipurl = x.Wipurl,
                        WipallocatedSize = x.WipallocatedSize,
                        ArchiveUrl = x.ArchiveUrl,
                        ArchiveAllocatedSize = x.ArchiveAllocatedSize,
                        ScaleDownTime = x.ScaleDownTime,
                        ScaleUpTimeInterval = x.ScaleUpTimeInterval,
                        MinimumFreeSize = x.MinimumFreeSize,
                        MinimumFreeSpace = x.MinimumFreeSpace,
                        OfflineTime = x.OfflineTime,
                        OnlineTime = x.OnlineTime,
                        ProductionOfflineTimeInterval = x.ProductionOfflineTimeInterval,
                        ManagerRoleGroupNames = x.ManagerRoleGroupNames,
                        UserRoleGroupNames = x.UserRoleGroupNames,
                        WipkeyName = x.WipkeyName,
                        ArchiveKeyName = x.ArchiveKeyName
                    }).ToList();

                return groups;
            }
        }

        /// <summary>
        /// ValidateTokenAndSetClaims.
        /// </summary>
        /// <param name="req">req.</param>
        /// <param name="log">log.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        private async Task<IActionResult> ValidateTokenAndGetUserClaims(HttpRequest req, ILogger log)
        {
            var headers = req.Headers;

            if (!headers.TryGetValue("AppAuthToken", out var bearertoken))
            {
                log.LogInformation("BadRequest : Invalid Bearer Auth Header Token");
                return new UnauthorizedObjectResult("BadRequest : Invalid Bearer Auth Header Token.");
            }

            var bearerAuthToken = bearertoken.First().Split(" ")[1];

            var authIntrospectUrl = Convert.ToString(Environment.GetEnvironmentVariable("AuthorizationServerUrl")) + "/introspect";
            var clientId = Convert.ToString(Environment.GetEnvironmentVariable("AuthorizationServerClientId"));

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("token", bearerAuthToken),
                new KeyValuePair<string, string>("token_type_hint", "access_token"),
                new KeyValuePair<string, string>("client_id", clientId)
            });

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(authIntrospectUrl, content);

            log.LogInformation("HTTP trigger function processed an external API call to Okta.");

            if (!response.IsSuccessStatusCode)
            {
                log.LogInformation("UnAuthorised Claim API call to Okta.");
                return new UnauthorizedObjectResult("UnAuthorised Claim API call to Okta.");
            }

            var result = await response.Content.ReadAsStringAsync();

            JObject json = JObject.Parse(result);
            string isActive = json["active"].ToString();
            log.LogInformation(isActive);

            if (isActive == "True")
            {
                var authUserInfoResponse = await this.GetUserInfo(req, log);
                return authUserInfoResponse;
            }
            else
            {
                log.LogInformation("AuthToken is Invalid or Expired.");
                return new UnauthorizedObjectResult("AuthToken is Invalid or Expired.");
            }
        }

        /// <summary>
        /// ValidateTokenAndSetClaims.
        /// </summary>
        /// <param name="req">req.</param>
        /// <param name="log">log.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        private async Task<IActionResult> GetUserInfo(HttpRequest req, ILogger log)
        {
            LoginUserInfo userInfo;
            var groups = new List<string>();
            var headers = req.Headers;

            if (!headers.TryGetValue("AppAuthToken", out var bearertoken))
            {
                log.LogInformation("BadRequest : Invalid Bearer Auth Header Token");
                return new BadRequestResult();
            }

            var bearerAuthToken = bearertoken.First().Split(" ")[1];

            var authGroupUrl = Convert.ToString(Environment.GetEnvironmentVariable("AuthorizationServerUrl")) + "/userinfo";

            using (var httpClientUserInfo = new HttpClient())
            {
                httpClientUserInfo.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerAuthToken);
                var userInfoResponse = await httpClientUserInfo.GetAsync(authGroupUrl);
                if (!userInfoResponse.IsSuccessStatusCode)
                {
                    return new UnauthorizedObjectResult("UnAuthorised UserInfo API call to Okta.");
                }

                var userInfoResult = await userInfoResponse.Content.ReadAsStringAsync();
                log.LogInformation("UserInfoJson : " + userInfoResult.ToString());

                JObject jsonUserInfo = JObject.Parse(userInfoResult);
                var groupTokens = jsonUserInfo["groups"] == null ? null : jsonUserInfo["groups"].ToList();

                if (groupTokens != null)
                {
                    foreach (var groupName in groupTokens)
                    {
                        groups.Add(groupName.Value<string>());
                    }
                }

                userInfo = new LoginUserInfo(jsonUserInfo["sub"].ToString(), groups, jsonUserInfo["email"].ToString());

                log.LogInformation("SubjectId : " + userInfo.SubjectId + " Groups: " + string.Join(",", groups) + " Email :  " + userInfo.Email);
            }

            var authResp = new AuthResponse() { ResponseStatus = "Success", UserInfo = userInfo };
            return new OkObjectResult(authResp);
        }
    }
}