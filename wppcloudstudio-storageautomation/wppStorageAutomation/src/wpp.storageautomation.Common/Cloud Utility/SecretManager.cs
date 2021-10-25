// <copyright file="SecretManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Azure;
using Azure.Security.KeyVault.Secrets;

namespace Wpp.StorageAutomation.Common
{
    /// <summary>
    ///  Secret Manager.
    /// </summary>
    public class SecretManager
    {
        private readonly SecretClient secretClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecretManager"/> class.
        /// </summary>
        /// <param name="secretClient">The production repository.</param>
        public SecretManager(SecretClient secretClient)
        {
            this.secretClient = secretClient;
        }

        /// <summary>
        ///  secret name.
        /// </summary>
        /// <summary>Gets or sets the archive connection string.</summary>
        /// <param name = "secretName" > The configuration.</param>
        /// <returns>.</placeholder></returns>
        public async Task<string> GetSecretAsync(string secretName)
        {
            // string keVaultUri = Environment.GetEnvironmentVariable("WPPStorageKeyVaultUri");.
            // var secretClient = new SecretClient(new Uri(keVaultUri), new DefaultAzureCredential());.
            string secretValue = string.Empty;
            try
            {
                KeyVaultSecret secret = await this.secretClient.GetSecretAsync(secretName);
                secretValue = secret.Value;
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    return secretValue;
                }
                else
                {
                    throw;
                }
            }

            return secretValue;
        }
    }
}
