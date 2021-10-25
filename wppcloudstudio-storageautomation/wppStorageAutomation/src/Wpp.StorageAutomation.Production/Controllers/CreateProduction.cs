// <copyright file="CreateProduction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.Production
{
    /// <summary>
    /// Create production.
    /// </summary>
    public class CreateProduction
    {
        private readonly IProduction prod;
        private readonly IProductionStoreRepository productionStoreRepository;
        private readonly IBaseSecurity baseSecurity;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProduction"/> class.
        /// </summary>
        /// <param name="prod">The product.</param>
        /// <param name="baseSecurity">The base security.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        public CreateProduction(IProduction prod, IBaseSecurity baseSecurity, IProductionStoreRepository productionStoreRepository)
        {
            this.prod = prod;
            this.baseSecurity = baseSecurity;
            this.productionStoreRepository = productionStoreRepository;
        }

        /// <summary>
        /// Validates the directory structure.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>
        /// IActionResult.
        /// </returns>
        public static FluentValidation.Results.ValidationResult ValidateDirectoryStructure(ProductionRequest request, string productionStoreId)
        {
            var pathValidator = new ProductionStoreIdValidator();
            var pathValidationResult = pathValidator.Validate(productionStoreId);
            if (!pathValidationResult.IsValid)
            {
                return pathValidationResult;
            }

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

        /// <summary>
        /// Create production starter.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="productionStoreId">The identifier.</param>
        /// <param name="log">The log.</param>
        /// <returns>Http response message.</returns>
        [FunctionName("CreateProduction")]
        public async Task<object> Create(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "production-stores/{productionStoreId}/productions/")] HttpRequest req, string productionStoreId, ILogger log)
        {
            try
            {
                log.LogInformation("Create production API triggered.");
                var jsonReq = await req.ReadAsStringAsync();
                ProductionRequest data = JsonConvert.DeserializeObject<ProductionRequest>(jsonReq);
                data.ProductionStoreId = productionStoreId;

                var prodStore = await this.productionStoreRepository.GetProductionStoreById(productionStoreId);
                if (prodStore == null)
                {
                    return new ObjectResult(new { id = string.Empty, name = string.Empty, error = $"No production store exists with: {productionStoreId} id." }) { StatusCode = (int)HttpStatusCode.NotFound };
                }

                var userAccess = this.baseSecurity.ValidateUserGroupsAccess(req, log, productionStoreId);
                if (userAccess.HasAccess)
                {
                    var validationResult = ValidateDirectoryStructure(data, productionStoreId);

                    if (validationResult != null && !validationResult.IsValid)
                    {
                        UnprocessableEntityObjectResult validationErrorMsg = new UnprocessableEntityObjectResult(validationResult.Errors.Select(e => new
                        {
                            Field = e.PropertyName,
                            AttemptedValue = e.AttemptedValue,
                            Error = e.ErrorMessage,
                        }));
                        Response response = new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.UnprocessableEntity)}-{HttpStatusCode.UnprocessableEntity}", Data = null, Error = validationErrorMsg.Value };
                        return new UnprocessableEntityObjectResult(response);
                    }

                    var outputs = new List<ProductionResponse>();
                    outputs.Add(await this.prod.CreateProduction(data));
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK}", Data = outputs, Error = null };
                }
                else
                {
                    log.LogError("Access forbidden to Create production API.");
                    return new ObjectResult(new { id = prodStore.Id, name = prodStore.Name }) { StatusCode = (int)HttpStatusCode.Forbidden };
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error while creating a production: {ex.Message}");
                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("Production store not found"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.NotFound)}-{HttpStatusCode.NotFound}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("Duplicate"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.UnprocessableEntity)}-{HttpStatusCode.UnprocessableEntity}", Data = null, Error = ex.Message };
                }

                if (ex.Message.ToLower().Contains("timeout"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout}", Data = null, Error = ex.Message };
                }

                return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
            }
        }
    }
}
