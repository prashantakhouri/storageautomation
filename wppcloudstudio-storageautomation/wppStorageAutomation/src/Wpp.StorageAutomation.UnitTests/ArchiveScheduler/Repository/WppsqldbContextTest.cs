using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Wpp.StorageAutomation.Entities.Models;
using productionEntity = Wpp.StorageAutomation.Entities.Models.Production;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.UnitTests.ArchiveScheduler.Repository
{
    public class WppsqldbContextTest
    {
        protected WppsqldbContextTest(DbContextOptions<WppsqldbContext> contextOptions)
        {
            ContextOptions = contextOptions;
            SeedProductionStore();
            SeedProduction();
        }

        protected DbContextOptions<WppsqldbContext> ContextOptions { get; }

        private void SeedProductionStore()
        {
            using (var context = new WppsqldbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var productionStores = new List<ProductionStoreEntity>()
                {
                    new ProductionStoreEntity()
                    {
                        Id = "ProdStore1",
                        Name = "ProdStore Name 1",
                        Region = "Asia",
                        WipkeyName = "TestKey1",
                        ArchiveKeyName = "TestKey1"
                    },
                    new ProductionStoreEntity()
                    {
                        Id = "ProdStore2",
                        Name = "ProdStore Name 2",
                        Region = "West Europe",
                        WipkeyName = "TestKey1",
                        ArchiveKeyName = "TestKey1"
                    },
                    new ProductionStoreEntity()
                    {
                        Id = "ProdStore3",
                        Name = "ProdStore Name 3",
                        Region = "US",
                        WipkeyName = "TestKey1",
                        ArchiveKeyName = "TestKey1"
                    },
                    new ProductionStoreEntity()
                    {
                        Id = "ProdStore4",
                        Name = "ProdStore Name 4",
                        Region = "Asia",
                        WipkeyName = "TestKey1",
                        ArchiveKeyName = "TestKey1"
                    },
                    new ProductionStoreEntity()
                    {
                        Id = "ProdStore5",
                        Name = "ProdStore Name 5",
                        Region = "West Europe",
                        WipkeyName = "TestKey1",
                        ArchiveKeyName = "TestKey1"
                    },
                };

                foreach(var productionStore in productionStores)
                {
                    context.ProductionStore.Add(productionStore);
                }
                
                context.SaveChanges();
            }
        }

        private void SeedProduction()
        {
            using (var context = new WppsqldbContext(ContextOptions))
            {
                var productions = new List<productionEntity>()
                {
                    new productionEntity()
                    {
                        Id = "Prod1",
                        Name = "prod 1",
                        ProductionStoreId = "ProdStore2",
                        Wipurl = "ProdStore2/prod 1",
                        Status = "Online",
                        CreatedDateTime = DateTime.UtcNow
                    },
                    new productionEntity()
                    {
                        Id = "Prod2",
                        Name = "prod 2",
                        ProductionStoreId = "ProdStore2",
                        Wipurl = "ProdStore2/prod 2",
                        Status = "Archive",
                        CreatedDateTime = DateTime.UtcNow
                    },
                    new productionEntity()
                    {
                        Id = "Prod3",
                        Name = "prod 3",
                        ProductionStoreId = "ProdStore2",
                        Wipurl = "ProdStore2/prod 3",
                        Status = "Online",
                        CreatedDateTime = DateTime.UtcNow
                    },
                    new productionEntity()
                    {
                        Id = "Prod4",
                        Name = "prod 4",
                        ProductionStoreId = "ProdStore3",
                        Wipurl = "ProdStore3/prod 4",
                        Status = "Online",
                        CreatedDateTime = DateTime.UtcNow
                    },
                    new productionEntity()
                    {
                        Id = "Prod5",
                        Name = "prod 5",
                        ProductionStoreId = "ProdStore5",
                        Wipurl = "ProdStore5/prod 5",
                        Status = "Online",
                        CreatedDateTime = DateTime.UtcNow
                    },
                    new productionEntity()
                    {
                        Id = "Prod6",
                        Name = "prod 6",
                        ProductionStoreId = "ProdStore4",
                        Wipurl = "ProdStore4/prod 6",
                        Status = "Online",
                        CreatedDateTime = DateTime.UtcNow
                    },
                    new productionEntity()
                    {
                        Id = "Prod7",
                        Name = "prod 7",
                        ProductionStoreId = "ProdStore4",
                        Wipurl = "ProdStore4/prod 7",
                        Status = "Online",
                        CreatedDateTime = DateTime.UtcNow
                    },
                };

                foreach (var production in productions)
                {
                    context.Production.Add(production);
                }

                context.SaveChanges();
            }
        }
    }
}
