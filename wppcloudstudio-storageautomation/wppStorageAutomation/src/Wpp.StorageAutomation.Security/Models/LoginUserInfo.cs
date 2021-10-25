// <copyright file="LoginUserInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Wpp.StorageAutomation.Security.Models
{
    /// <summary>
    /// Gets AuthRoles.
    /// </summary>
    public class LoginUserInfo
    {
        private readonly string subjectIdValue;
        private readonly List<string> groupsValue;
        private readonly string emailValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginUserInfo"/> class.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="groups">The groups.</param>
        /// <param name="email">The email.</param>
        public LoginUserInfo(string subjectId, List<string> groups, string email)
        {
            this.subjectIdValue = subjectId;
            this.groupsValue = groups;
            this.emailValue = email;
        }

        /// <summary>
        /// Gets SubjectId.
        /// </summary>
        public string SubjectId
        {
            get { return this.subjectIdValue; }
        }

        /// <summary>
        /// Gets Groups.
        /// </summary>
        public List<string> Groups
        {
            get { return this.groupsValue; }
        }

        /// <summary>
        /// Gets Email.
        /// </summary>
        public string Email
        {
            get { return this.emailValue; }
        }
    }
}
