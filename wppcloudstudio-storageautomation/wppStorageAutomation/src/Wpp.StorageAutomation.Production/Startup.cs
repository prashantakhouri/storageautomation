// <copyright file="Startup.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Security;

[assembly: FunctionsStartup(typeof(Wpp.StorageAutomation.Production.Startup))]

namespace Wpp.StorageAutomation.Production
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
            builder.Services.AddScoped<IProduction, Productions>();
            builder.Services.AddScoped<IStorageAccountConfig, StorageAccountConfig>();
            builder.Services.AddScoped<ICloudStorageUtility, CloudStorageUtility>();
            builder.Services.AddSingleton<IFunctionFilter, FunctionExceptionFilter>();
            builder.Services.AddScoped<IProductionRepository, ProductionRepository>();
            builder.Services.AddScoped<IProductionStoreRepository, ProductionStoreRepository>();
            builder.Services.AddScoped<IActiveDirectoryUtility, ActiveDirectoryUtility>();
            builder.Services.AddScoped<IGroupsRepository, GroupsRepository>();
            builder.Services.AddScoped<ISddlBuilderUtility, SddlBuilderUtility>();
            builder.Services.AddScoped<IBaseSecurity, BaseSecurity>();
        }
    }
}
