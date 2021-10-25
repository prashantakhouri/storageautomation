// <copyright file="SddlBuilderUtility.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Security.Repository;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.Security
{
    /// <summary>
    /// SDDL Builder Utility.
    /// </summary>
    public class SddlBuilderUtility : ISddlBuilderUtility
    {
        private readonly IStorageAccountConfig storageAccountConfig;
        private readonly IActiveDirectoryUtility activeDirectoryUtility;
        private readonly IGroupsRepository groupsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SddlBuilderUtility" /> class.
        /// </summary>
        /// <param name="storageAccountConfig">The storage account configuration.</param>
        /// <param name="activeDirectoryUtility">The active directory utility.</param>
        /// <param name="groupsRepository">The groups repository.</param>
        public SddlBuilderUtility(
            IStorageAccountConfig storageAccountConfig,
            IActiveDirectoryUtility activeDirectoryUtility,
            IGroupsRepository groupsRepository)
        {
            this.storageAccountConfig = storageAccountConfig;
            this.activeDirectoryUtility = activeDirectoryUtility;
            this.groupsRepository = groupsRepository;
        }

        /// <inheritdoc/>
        public string BuildSDDL(ProductionStoreEntity productionStore, ActivityType activity)
        {
            string sddlTemplate = string.Empty;

            bool isGraphSIDS = Convert.ToBoolean(this.storageAccountConfig.IsGraphSIDS);
            if (activity == ActivityType.Create)
            {
                sddlTemplate = this.storageAccountConfig.WPPSDDLConfig != null ? Convert.ToString(this.storageAccountConfig.WPPSDDLConfig) : string.Empty;
            }
            else if (activity == ActivityType.Restore)
            {
                sddlTemplate = this.storageAccountConfig.WPPFullControlSDDLConfig != null ? Convert.ToString(this.storageAccountConfig.WPPFullControlSDDLConfig) : string.Empty;
            }

            List<string> mgrGroups = new List<string>();
            List<string> userGroups = new List<string>();
            List<string> groups;
            List<string> sddlList = new List<string>();

            if (!string.IsNullOrEmpty(sddlTemplate))
            {
                List<ActiveDirectoryGroupInfo> adGroupInfo;
#pragma warning disable S2259 // Null pointers should not be dereferenced
                sddlList.AddRange(sddlTemplate.ToString().Split('|').Select(i => i));
#pragma warning restore S2259 // Null pointers should not be dereferenced

                // Trim group name for exact match.
                mgrGroups.AddRange(productionStore.ManagerRoleGroupNames.ToString().Trim().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()));
                userGroups.AddRange(productionStore.UserRoleGroupNames.ToString().Trim().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()));
                groups = mgrGroups.Concat(userGroups).ToList();

                if (isGraphSIDS)
                {
                    adGroupInfo = this.activeDirectoryUtility.AdGroupDetailsFromGraphApi(groups);
                }
                else
                {
                    adGroupInfo = this.GetGroupSids(groups);
                }

                if (adGroupInfo.Any())
                {
                    var secGroups = groups.Where(p => adGroupInfo.All(p2 => p2.GroupName != p)).ToList();
                    if (secGroups.Any())
                    {
                        throw new KeyNotFoundException($"Security Identifiers not found for groups: {string.Join(",", secGroups).ToString()}. Please contact System Administrator.");
                    }

                    List<ActiveDirectoryGroupInfo> mgrSids = (from grp in adGroupInfo
                                                              where mgrGroups.Contains(grp.GroupName)
                                                              select grp).ToList();

                    List<ActiveDirectoryGroupInfo> userSids = (from grp in adGroupInfo
                                                               where userGroups.Contains(grp.GroupName)
                                                               select grp).ToList();

                    // Validate SID's
                    List<ActiveDirectoryGroupInfo> allSIDs = mgrSids.Concat(userSids).ToList();
                    this.ValidateSID(allSIDs);

                    var sddlFormat = new System.Text.StringBuilder();

                    sddlFormat.Append(sddlList.FirstOrDefault());
                    foreach (var sid in mgrSids)
                    {
                        sddlFormat.Append(sddlList[1].ToString().Replace("[SIDMGR]", sid.OnPremisesSecurityIdentifier));
                    }

                    foreach (var sid in userSids)
                    {
                        sddlFormat.Append(sddlList[2].ToString().Replace("[SIDUSR]", sid.OnPremisesSecurityIdentifier));
                    }

                    return sddlFormat.ToString();
                }
                else
                {
                    throw new KeyNotFoundException("Security groups Identifier not found. Please contact System Administrator.");
                }
            }
            else
            {
                throw new KeyNotFoundException("SDDL template not found. Please contact System Administrator.");
            }
        }

        private bool IsValidateSID(string value, out SecurityIdentifier result)
        {
            try
            {
                result = new SecurityIdentifier(value);
                return true;
            }
            catch (ArgumentException)
            {
                result = null;
                return false;
            }
        }

        private List<ActiveDirectoryGroupInfo> GetGroupSids(List<string> groups)
        {
            List<ActiveDirectoryGroupInfo> adGroupInfo;
            var groupslist = this.groupsRepository.GetGroups(groups);

            adGroupInfo = groupslist.Select(x => new ActiveDirectoryGroupInfo()
            {
                Id = Convert.ToString(x.Id),
                GroupName = x.GroupName,
                SecurityIdentifier = x.GroupSid,
                OnPremisesSecurityIdentifier = x.GroupSid
            }).ToList();

            return adGroupInfo;
        }

        private void ValidateSID(List<ActiveDirectoryGroupInfo> sidsList)
        {
            foreach (var sid in sidsList)
            {
                bool isSidValid = this.IsValidateSID(sid.OnPremisesSecurityIdentifier.ToString(), out var secSid);
                if (!isSidValid)
                {
                    throw new FormatException($"Invalid Security Identifier: {sid.OnPremisesSecurityIdentifier}. Please contact System Administrator.");
                }
            }
        }
    }
}
