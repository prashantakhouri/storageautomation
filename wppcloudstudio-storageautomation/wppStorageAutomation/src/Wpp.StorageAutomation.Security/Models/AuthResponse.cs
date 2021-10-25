// <copyright file="AuthResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Wpp.StorageAutomation.Security.Models
{
    /// <summary>
    /// AuthResponse.
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public string ResponseStatus { get; set; }

        /// <summary>
        /// Gets or sets UserId.
        /// </summary>
        public LoginUserInfo UserInfo { get; set; }
    }
}
