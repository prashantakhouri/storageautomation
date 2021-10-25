// <copyright file="CreateProduction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation
{
    /// <summary>
    /// Create production.
    /// </summary>
    public static class CreateProduction
    {
        /// <summary>
        /// Create production starter.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="starter">The starter.</param>
        /// <param name="log">The log.</param>
        /// <returns>Http response message.</returns>
        [FunctionName(nameof(CreateProductionStarter))]
        public static async Task<IActionResult> CreateProductionStarter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ProductionStores/{id}/Productions")] HttpRequest req,
            string id,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
             // to check the group access.
             // var grouplist = ((AuthResponse)((OkObjectResult)actionResult).Value).Groups;.
             // if (validateLoginUserGroupsWithBackend(grouplist,id))  //createSomeMethod to check user groups with productionstore id groups in DB
             // {.
                var jsonReq = await req.ReadAsStringAsync();
                ProductionRequest data = JsonConvert.DeserializeObject<ProductionRequest>(jsonReq);
                data.ProductionStoreId = id;
                var validationResult = ValidateDirectoryStructure(data);

                if (validationResult != null && !validationResult.IsValid)
                {
                    return new UnprocessableEntityObjectResult(validationResult.Errors.Select(e => new
                    {
                        Field = e.PropertyName,
                        AttemptedValue = e.AttemptedValue,
                        Error = e.ErrorMessage,
                    }));
                }

                // Function input comes from the request content.
                string instanceId = await starter.StartNewAsync("CreateProductionOrchestrator", data);
                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

                return starter.CreateCheckStatusResponse(req, instanceId);

                // }.
                // else
                // {.
                //     return new UnauthorizedResult();. // or return group validation reasons from backend
                //  }.
        }

        /// <summary>
        /// Validates the directory structure.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IActionResult.</returns>
        public static FluentValidation.Results.ValidationResult ValidateDirectoryStructure(ProductionRequest request)
        {
            var validator = new ProductionValidator();

            var validationResult = validator.Validate(request);
            if (validationResult.IsValid)
            {
                foreach (var dir in request.DirectoryTree)
                {
                    foreach (var subItem in dir.SubItems)
                    {
                        var subValidator = new SubItemValidator();

                        var subValidationResult = subValidator.Validate(subItem);
                        if (!subValidationResult.IsValid)
                        {
                            return subValidationResult;
                        }
                        else
                        {
                            var valResult = ValidateSubDirectoryStructure(subItem);
                            if (valResult != null && !valResult.IsValid)
                            {
                                return valResult;
                            }
                        }
                    }
                }
            }
            else
            {
                return validationResult;
            }

            return null;
        }

        /// <summary>
        /// Validates the sub directory structure.
        /// </summary>
        /// <param name="subItem">The sub item.</param>
        /// <returns>IActionResult.</returns>
        public static FluentValidation.Results.ValidationResult ValidateSubDirectoryStructure(SubItem subItem)
        {
            var subValidationResult = new FluentValidation.Results.ValidationResult();
            foreach (var item in subItem.SubItems)
            {
                var subValidator = new SubItemValidator();

                subValidationResult = subValidator.Validate(item);
                if (subValidationResult.IsValid)
                {
                    return ValidateSubDirectoryStructure(item);
                }
                else
                {
                    return subValidationResult;
                }
            }

            return subValidationResult;
        }
    }
}