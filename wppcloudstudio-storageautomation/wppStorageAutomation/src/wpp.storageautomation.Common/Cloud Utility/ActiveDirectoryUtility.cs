// <copyright file="ActiveDirectoryUtility.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Wpp.StorageAutomation.Common
{
    /// <summary>
    /// ActiveDirectoryUtility.
    /// </summary>
    public class ActiveDirectoryUtility : IActiveDirectoryUtility
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectoryUtility"/> class.
        /// </summary>
        /// <param name="groupsRepository">The groups repository.</param>
        public ActiveDirectoryUtility()
        {
        }

        /// <summary>
        /// Ads the group details from graph API.
        /// </summary>
        /// <param name="groupsList">The groups.</param>
        /// <returns>
        /// ActiveDirectoryGroupInfo.
        /// </returns>
        public List<ActiveDirectoryGroupInfo> AdGroupDetailsFromGraphApi(List<string> groupsList)
        {
            var task = Task.Run(async () => await this.GetGraphApiClient());
            if (task.IsFaulted && task.Exception != null)
            {
                throw task.Exception;
            }

            GraphServiceClient client = task.Result;

            // GraphServiceClient client = this.GetGraphApiClient().Result;.
            var allAdGroups = new List<ActiveDirectoryGroupInfo>();
            var groups = client.Groups.Request().GetAsync().Result;

            while (groups.Count > 0)
            {
                var groupSids = (from grp in groups
                                 where groupsList.Contains(grp.OnPremisesSamAccountName) && grp.OnPremisesSecurityIdentifier != null
                                 select grp).ToList();

                allAdGroups = groupSids.Select(x => new ActiveDirectoryGroupInfo()
                {
                    Id = Convert.ToString(x.Id),
                    GroupName = x.OnPremisesSamAccountName,
                    SecurityIdentifier = x.SecurityIdentifier,
                    OnPremisesSecurityIdentifier = x.OnPremisesSecurityIdentifier
                }).ToList();

                if (groups.NextPageRequest != null)
                {
                    groups = groups.NextPageRequest.GetAsync().Result;
                }
                else
                {
                    break;
                }
            }

            return allAdGroups;
        }

        /// <summary>
        /// Gets all ad groups.
        /// </summary>
        /// <returns>Returns all groups.</returns>
        public List<ActiveDirectoryGroupInfo> GetAllAdGroups()
        {
            var task = Task.Run(async () => await this.GetGraphApiClient());
            if (task.IsFaulted && task.Exception != null)
            {
                throw task.Exception;
            }

            GraphServiceClient client = task.Result;

            var allAdGroups = new List<ActiveDirectoryGroupInfo>();

            var groups = client.Groups.Request().GetAsync().Result;

            while (groups.Count > 0)
            {
                allAdGroups.AddRange(
                           groups.Select(a =>
                               new ActiveDirectoryGroupInfo()
                               {
                                   Id = a.Id,
                                   GroupName = a.OnPremisesSamAccountName,
                                   SecurityIdentifier = a.SecurityIdentifier,
                                   OnPremisesSecurityIdentifier = a.OnPremisesSecurityIdentifier
                               }));

                if (groups.NextPageRequest != null)
                {
                    groups = groups.NextPageRequest.GetAsync().Result;
                }
                else
                {
                    break;
                }
            }

            return allAdGroups;
        }

        /// <summary>
        /// Gets the graph API client.
        /// </summary>
        /// <returns>returns client.</returns>
        public async Task<GraphServiceClient> GetGraphApiClient()
        {
            var graphapiclientId = Environment.GetEnvironmentVariable("GraphApiClientId");
            var graphapisecret = Environment.GetEnvironmentVariable("GraphApiClientSecret");
            var graphapidomain = Environment.GetEnvironmentVariable("GraphApiDomain");

            var credentials = new ClientCredential(graphapiclientId, graphapisecret);
            var authContext = new AuthenticationContext($"https://login.microsoftonline.com/{graphapidomain}/");
            var token = await authContext.AcquireTokenAsync("https://graph.microsoft.com/", credentials);
            var accessToken = token.AccessToken;

            var graphServiceClient = new GraphServiceClient(
                new DelegateAuthenticationProvider((requestMessage) =>
                {
                    requestMessage
                .Headers
                .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                    return Task.CompletedTask;
                }));

            return graphServiceClient;
        }
    }
}
