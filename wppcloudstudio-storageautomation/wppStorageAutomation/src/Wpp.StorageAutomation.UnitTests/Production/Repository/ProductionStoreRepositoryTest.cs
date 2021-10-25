using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpp.StorageAutomation.Entities.Models;
using Wpp.StorageAutomation.Production;
using Wpp.StorageAutomation.UnitTests.DataMovement.Repository;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.UnitTests.Production.Repository
{
    [TestClass()]
    public class ProductionStoreRepositoryTest : WppsqldbContextTest
    {
        public ProductionStoreRepositoryTest() : base(
            new DbContextOptionsBuilder<WppsqldbContext>()
                .UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=Wpp.StorageAutomation.WppDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
                .Options)
        {
            Environment.SetEnvironmentVariable("WPPSQLDBConnection", @"Data Source=(localdb)\ProjectsV13;Initial Catalog=Wpp.StorageAutomation.WppDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        }
        [TestMethod()]
        public async Task AddProductionStore_Test()
        {
            var prod = new ProductionStoreEntity()
            {
                Id = "123456",
                Name = "UnitTestProductionStore",
                Region = "Asia",
                WipkeyName = "TestKey1",
                ArchiveKeyName = "TestKey1"
            };

            using (var context = new WppsqldbContext(ContextOptions))
            {
                var productionStoreRepository = new ProductionStoreRepository();

                await productionStoreRepository.AddProductionStore(prod);
                var items = await productionStoreRepository.GetAllProductionStores();

                Assert.IsNotNull(items);
                Assert.AreEqual(prod.Id, items.First().Id);
            }
        }

        [TestMethod()]
        public async Task DeleteProductionStore_Test()
        {
            var ProductionStoreId = "ProdStore3";

            using (var context = new WppsqldbContext(ContextOptions))
            {
                var productionStoreRepository = new ProductionStoreRepository();

                await productionStoreRepository.DeleteProductionStore(ProductionStoreId);
                var items = await productionStoreRepository.GetAllProductionStores();
                var item = await productionStoreRepository.GetProductionStoreByName("ProdStore Name 3");

                Assert.IsNotNull(items);
                Assert.IsNull(item);
            }
        }

        [TestMethod()]
        public async Task UpdateProductionStore_Test()
        {
            var prod = new ProductionStoreEntity()
            {
                Id = "ProdStore4",
                Name = "ProdStore4Updated",
                Region = "Asia",
                WipkeyName = "TestKey1",
                ArchiveKeyName = "TestKey1"
            };

            using (var context = new WppsqldbContext(ContextOptions))
            {
                var productionStoreRepository = new ProductionStoreRepository();

                await productionStoreRepository.UpdateProductionStore(prod);
                var item = await productionStoreRepository.GetProductionStoreByName("ProdStore4Updated");

                Assert.IsNotNull(item);
                Assert.AreEqual("ProdStore4", item.Id);
            }
        }
    }
}