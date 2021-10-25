// <copyright file="Startup.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Entities.Models;
using Wpp.StorageAutomation.ProductionControl.Repository;

[assembly: FunctionsStartup(typeof(Wpp.StorageAutomation.ProductionControl.Startup))]

namespace Wpp.StorageAutomation.ProductionControl
{
    /// <summary>
    /// Class Startup.
    /// Implements the <see cref="Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup" />.
    /// </summary>
    /// <seealso cref="Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup" />
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

        /// <summary>
        /// Configures the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IProduction, Production>();
            builder.Services.AddScoped<IStorageAccountConfig, StorageAccountConfig>();
            builder.Services.AddScoped<ICloudStorageUtility, CloudStorageUtility>();
            builder.Services.AddSingleton<IFunctionFilter, FunctionExceptionFilter>();
            builder.Services.AddScoped<IProductionRepository, ProductionRepository>();
            builder.Services.AddScoped<IProductionStoreRepository, ProductionStoreRepository>();

            string sqlConnection = Environment.GetEnvironmentVariable("WPPSQLDBConnection");
            builder.Services.AddDbContext<WppsqldbContext>(options => options.UseSqlServer(sqlConnection, options => options.EnableRetryOnFailure()));
        }
    }
}
