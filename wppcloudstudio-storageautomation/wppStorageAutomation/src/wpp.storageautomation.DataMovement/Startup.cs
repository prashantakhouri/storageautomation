// <copyright file="Startup.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.DataMovement.Contracts;
using Wpp.StorageAutomation.DataMovement.Repository;
using Wpp.StorageAutomation.Security;

[assembly: FunctionsStartup(typeof(Wpp.StorageAutomation.DataMovement.Startup))]

namespace Wpp.StorageAutomation.DataMovement
{
    /// <summary>
    ///   The startup.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>Configures the application configuration.</summary>
        /// <param name="builder">The builder.</param>
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "local.settings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"local.settings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        /// <summary>Configures the specified builder.</summary>
        /// <param name="builder">The builder.</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IDataMovement, DataMovement>();
            builder.Services.AddScoped<IStorageAccountConfig, StorageAccountConfig>();
            builder.Services.AddScoped<ICloudStorageUtility, CloudStorageUtility>();
            builder.Services.AddScoped<IDirectoryHelper, DirectoryHelper>();

            builder.Services.AddScoped<IProductionRepository, ProductionRepository>();
            builder.Services.AddScoped<IProductionStoreRepository, ProductionStoreRepository>();
            builder.Services.AddScoped<IGroupsRepository, GroupsRepository>();
            builder.Services.AddScoped<IBaseSecurity, BaseSecurity>();
            builder.Services.AddScoped<ISddlBuilderUtility, SddlBuilderUtility>();
            builder.Services.AddScoped<IActiveDirectoryUtility, ActiveDirectoryUtility>();
        }
    }
}