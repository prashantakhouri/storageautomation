// <copyright file="Production.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ExtensionMethod;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.ProductionControl.Repository;

namespace Wpp.StorageAutomation
{
    /// <summary>
    /// Production.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.IProduction" />
    public class Production : IProduction
    {
        private readonly CloudStorageAccount accountStorageWip;
        private readonly ICloudStorageUtility cloudStorageUtility;
        private readonly IStorageAccountConfig storageAccountConfig;
        private readonly IProductionRepository productionRepository;
        private readonly IProductionStoreRepository productionStoreRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Production" /> class.
        /// </summary>
        /// <param name="storageAccountConfig">The storage account configuration.</param>
        /// <param name="cloudStorageUtility">The cloud storage utility.</param>
        /// <param name="productionRepository">The production repository.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        public Production(
            IStorageAccountConfig storageAccountConfig,
            ICloudStorageUtility cloudStorageUtility,
            IProductionRepository productionRepository,
            IProductionStoreRepository productionStoreRepository)
        {
            this.storageAccountConfig = storageAccountConfig;
            this.cloudStorageUtility = cloudStorageUtility;
            this.accountStorageWip = CloudStorageAccount.Parse(string.Empty);
            this.productionRepository = productionRepository;
            this.productionStoreRepository = productionStoreRepository;
        }

        /// <summary>
        /// Gets the production list.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>ProductionListResponse.</returns>
        public async Task<ProductionListResponse> GetProductionsByProductionStoreAsync(string productionStoreId)
        {
            var prodStore = await this.productionStoreRepository.GetProductionStoreById(productionStoreId);

            if (prodStore != null)
            {
                var productions = await this.productionRepository.GetProductionByProductionStoreId(productionStoreId);
                var productionListResponse = new ProductionListResponse()
                {
                    ProductionList = productions.Select(x => new ProductionRow()
                    {
                        Id = x.Id,
                        ProductionStoreId = x.ProductionStoreId,
                        Name = x.Name,
                        Wipurl = x.Wipurl,
                        ArchiveUrl = x.ArchiveUrl,
                        ArchiveId = x.ArchiveId,
                        Status = x.Status,
                        CreatedDateTime = x.CreatedDateTime,
                        LastSyncDateTime = x.LastSyncDateTime,
                        SizeInBytes = x.SizeInBytes
                    })
                };
                return productionListResponse;
            }
            else
            {
                throw new KeyNotFoundException($"Production store not found. Production Store Id : {productionStoreId}");
            }
        }

        /// <summary>
        /// Creates the production.
        /// </summary>
        /// <param name="productionReq">The production req.</param>
        /// <returns>
        ///   The production response.
        /// </returns>
        public async Task<ProductionResponse> CreateProduction(ProductionRequest productionReq)
        {
            if (productionReq == null || !productionReq.DirectoryTree.Any())
            {
                return null;
            }

            return await this.CreateDirectoryTree(productionReq);
        }

        /// <summary>
        /// Creates the folder structure for a prouction.
        /// </summary>
        /// <param name="productionReq">The production request.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductionResponse> CreateDirectoryTree(ProductionRequest productionReq)
        {
            string parentDirName = string.Empty;
            string parentDirUri = string.Empty;
            ProductionResponse resObj = null;

            // Get a reference to the file share root directory reference.
            CloudFileDirectory fileDirectory = this.cloudStorageUtility.GetCloudFileShare(this.accountStorageWip, productionReq.ProductionStoreId);

            if (await fileDirectory.ExistsAsync())
            {
                foreach (var dir in productionReq.DirectoryTree)
                {
                    // Extracting productionName from the Tokens
                    parentDirName = productionReq.Tokens.FirstOrDefault().ProductionToken.ToString().Trim();
                    CloudFileDirectory rootDirectory = fileDirectory.GetDirectoryReference(parentDirName);

                    if (!await rootDirectory.ExistsAsync())
                    {
                        // Checking for duplicate directory name
                        this.CheckDuplicateDirectoryName(productionReq);

                        CloudFileDirectory parentDirectory = fileDirectory.GetDirectoryReference(parentDirName);
                        parentDirUri = parentDirectory.Uri.AbsolutePath.ToString();
                        resObj = this.CreateProductionResponse(parentDirName, parentDirUri, DirectoryStatus.Online);
                        var production = await this.productionRepository.GetProductionByName(productionReq.ProductionStoreId, resObj.Name);
                        if (production == null)
                        {
                            await this.AddToProductionTable(resObj, productionReq);
                        }
                        else
                        {
                            production.LastSyncDateTime = null;
                            production.CreatedDateTime = DateTime.UtcNow;

                            await this.productionRepository.UpdateProduction(production);
                        }

                        try
                        {
                            await this.CreateDirectory(fileDirectory, dir.Type, parentDirName);
                            await this.CreateSubDirectoryTree(dir, parentDirectory);
                        }
                        catch (Exception)
                        {
                            await this.productionRepository.DeleteProduction(resObj.Id);
                            throw;
                        }
                    }
                    else
                    {
                        throw new DuplicateNameException($"Duplicate name not allowed: {parentDirName}");
                    }
                }
            }
            else
            {
                throw new KeyNotFoundException($"Production store not found. Production store ID: {productionReq.ProductionStoreId}");
            }

            return resObj;
        }

        /// <summary>
        /// Creates the production response.
        /// </summary>
        /// <param name="dirName">Name of the dir.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="status">The status.</param>
        /// <returns>ProductionResponse.</returns>
        private ProductionResponse CreateProductionResponse(string dirName, string uri, DirectoryStatus status)
        {
            ProductionResponse res = new ProductionResponse();

            res.Id = Guid.NewGuid().ToString();
            res.Name = dirName;
            res.Wipurl = uri;
            res.Status = status.ToString();
            res.CreatedDateTime = DateTime.UtcNow;

            return res;
        }

        /// <summary>
        /// Throws the duplicate exception.
        /// </summary>
        /// <param name="duplicateDirectories">The duplicate directories.</param>
        /// <exception cref="System.Data.DuplicateNameException">Duplicate name not allowed: {duplicateStr}.</exception>
        private void ThrowDuplicateException(List<string> duplicateDirectories)
        {
            string duplicateStr = string.Join(", ", duplicateDirectories).RemoveSlash();
            throw new DuplicateNameException($"Duplicate name not allowed: {duplicateStr}");
        }

        /// <summary>
        /// Creates the sub directory tree.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="parentDirectory">The parent directory.</param>
        private async Task CreateSubDirectoryTree(DirectoryTree dir, CloudFileDirectory parentDirectory)
        {
            if (dir.SubItems.Any())
            {
                foreach (var subItem in dir.SubItems)
                {
                    string subDirName = subItem.Path.Trim().RemoveSpecialChars();
                    await this.CreateDirectory(parentDirectory, subItem.Type, subDirName);
                    CloudFileDirectory subDirectory = parentDirectory.GetDirectoryReference(subDirName);
                    await this.TraverseDirectoryTree(subDirectory, subItem);
                }
            }
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="fileDirectory">The file directory.</param>
        /// <param name="type">The type.</param>
        /// <param name="dirName">Name of the dir.</param>
        private async Task CreateDirectory(CloudFileDirectory fileDirectory, ItemType type, string dirName)
        {
            if (type.ToString().ToLower().Equals(ItemType.Folder.ToString().ToLower()))
            {
                CloudFileDirectory customDirectory = fileDirectory.GetDirectoryReference(dirName);
                await customDirectory.CreateIfNotExistsAsync();
            }
        }

        /// <summary>
        /// Checks the name of the duplicate directory.
        /// </summary>
        /// <param name="prouctionReq">The prouction req.</param>
        private void CheckDuplicateDirectoryName(ProductionRequest prouctionReq)
        {
            foreach (var dir in prouctionReq.DirectoryTree)
            {
                var drResult = dir.SubItems.GroupBy(x => x.Path.Trim().ToLower()).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (!drResult.Any())
                {
                    foreach (var subItem in dir.SubItems)
                    {
                        drResult = subItem.SubItems.GroupBy(x => x.Path.Trim().ToLower()).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                        if (!drResult.Any())
                        {
                            this.CheckDuplicateSubDirectory(subItem);
                        }
                        else
                        {
                            this.ThrowDuplicateException(drResult.ToList());
                        }
                    }
                }
                else
                {
                    this.ThrowDuplicateException(drResult.ToList());
                }
            }
        }

        /// <summary>
        /// Traverses the directory tree.
        /// </summary>
        /// <param name="fileDirectory">The file directory.</param>
        /// <param name="subItem">The sub item.</param>
        private async Task TraverseDirectoryTree(CloudFileDirectory fileDirectory, SubItem subItem)
        {
            foreach (var item in subItem.SubItems)
            {
                string subDirName = item.Path.Trim().RemoveSpecialChars();
                await this.CreateDirectory(fileDirectory, item.Type, item.Path.Trim().RemoveSpecialChars());

                // reference of sub-directory
                CloudFileDirectory subDirectory = fileDirectory.GetDirectoryReference(subDirName);
                await this.TraverseDirectoryTree(subDirectory, item); // Recurse here
            }
        }

        /// <summary>
        /// Checks the duplicate sub directory.
        /// </summary>
        /// <param name="subItem">The sub item.</param>
        private void CheckDuplicateSubDirectory(SubItem subItem)
        {
            foreach (var item in subItem.SubItems)
            {
                var subDir = item.SubItems.GroupBy(x => x.Path.Trim().ToLower()).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (!subDir.Any())
                {
                    this.CheckDuplicateSubDirectory(item); // Recurse here
                }
                else
                {
                    this.ThrowDuplicateException(subDir.ToList());
                }
            }
        }

        /// <summary>
        /// Adds to production table.
        /// </summary>
        /// <param name="response">The response.</param>
        private async Task AddToProductionTable(ProductionResponse response, ProductionRequest productionReq)
        {
            var production = new Wpp.StorageAutomation.Entities.Models.Production
            {
                Id = response.Id,
                Name = response.Name,
                ProductionStoreId = productionReq.ProductionStoreId,
                Wipurl = response.Wipurl,
                Status = response.Status,
                ArchiveUrl = response.Wipurl,
                CreatedDateTime = response.CreatedDateTime
            };

            await this.productionRepository.AddProduction(production);
        }
    }
}
