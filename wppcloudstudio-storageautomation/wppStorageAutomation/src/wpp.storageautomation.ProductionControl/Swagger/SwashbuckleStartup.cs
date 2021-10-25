using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.OpenApi;
using Wpp.StorageAutomation.Common.Constants;
using Wpp.StorageAutomation.ProductionControl;

[assembly: WebJobsStartup(typeof(SwashBuckleStartup))]

namespace Wpp.StorageAutomation.ProductionControl
{
    /// <summary>
    /// Swashbuckle startup.
    /// </summary>
    /// <seealso cref="Microsoft.Azure.WebJobs.Hosting.IWebJobsStartup" />
    internal class SwashBuckleStartup : IWebJobsStartup
    {
        /// <summary>
        /// Configures the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
            {
                opts.SpecVersion = OpenApiSpecVersion.OpenApi3_0;
                opts.AddCodeParameter = true;
                opts.PrependOperationWithRoutePrefix = true;
                opts.Documents = new[]
                {
                    new SwaggerDocument
                    {
                        Name = "v1",
                        Title = SwaggerConstants.Title,
                        Description = SwaggerConstants.DescriptionPC,
                        Version = "v2"
                    }
                };
                opts.Title = SwaggerConstants.Header;
            });
        }
    }
}
