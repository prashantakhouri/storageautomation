using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wpp.StorageAutomation.DataMovement.Repository;
using Wpp.StorageAutomation.Entities.Models;
using Wpp.StorageAutomation.UnitTests.DataMovement.Repository;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.UnitTests.DataMovement
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
        public async Task GetAllProductionStores_ShouldListAllProductionStores()
        {
            Environment.SetEnvironmentVariable("WPPSQLDBConnection", @"Data Source=(localdb)\ProjectsV13;Initial Catalog=Wpp.StorageAutomation.WppDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            var prod = new ProductionStoreEntity()
            {
                Id = "ProdStore1",
                Name = "ProdStore Name 1",
            };

            var productionStoreRepository = new ProductionStoreRepository();

            var items = await productionStoreRepository.GetAllProductionStores();

            Assert.IsNotNull(items);
            Assert.AreEqual(5, items.ToList().Count);
            Assert.AreEqual(prod.Id, items.First().Id);
        }

        [TestMethod()]
        public async Task GetProductionStoreByName_ShouldReturnProductionStore()
        {
            var prod = new ProductionStoreEntity()
            {
                Id = "ProdStore1",
                Name = "ProdStore Name 1"
            };

            var productionStoreRepository = new ProductionStoreRepository();

            var item = await productionStoreRepository.GetProductionStoreByName(prod.Name);

            Assert.IsNotNull(item);
            Assert.AreEqual(prod.Id, item.Id);
        }

        [TestMethod()]
        public async Task AddProductionStore_ShouldAddProductionStore()
        {
            var prod = new ProductionStoreEntity()
            {
                Id = "ProdStore6",
                Name = "ProdStore Name 6",
                WipkeyName = "TestKey1",
                ArchiveKeyName = "TestKey1"
            };

            var productionStoreRepository = new ProductionStoreRepository();

            await productionStoreRepository.AddProductionStore(prod);
            var items = await productionStoreRepository.GetAllProductionStores();

            Assert.IsNotNull(items);
            Assert.AreEqual(6, items.ToList().Count);
            Assert.AreEqual(prod.Id, items.Last().Id);
        }

        [TestMethod()]
        public async Task DeleteProductionStore_ShouldRemoveProductionStore()
        {
            var ProductionStoreId = "ProdStore3";

            var productionStoreRepository = new ProductionStoreRepository();

            await productionStoreRepository.DeleteProductionStore(ProductionStoreId);
            var items = await productionStoreRepository.GetAllProductionStores();
            var item = await productionStoreRepository.GetProductionStoreByName("ProdStore Name 3");

            Assert.IsNotNull(items);
            Assert.AreEqual(4, items.ToList().Count);
            Assert.IsNull(item);
        }

        [TestMethod()]
        public async Task UpdateProductionStore_ShouldUpdateProductionStore()
        {
            var prod = new ProductionStoreEntity()
            {
                Id = "ProdStore4",
                Name = "ProdStore Name 4 - updated",
                WipkeyName = "TestKey1",
                ArchiveKeyName = "TestKey1"
            };

            var productionStoreRepository = new ProductionStoreRepository();

            await productionStoreRepository.UpdateProductionStore(prod);
            var item = await productionStoreRepository.GetProductionStoreByName("ProdStore Name 4 - updated");

            Assert.IsNotNull(item);
            Assert.AreEqual("ProdStore4", item.Id);
        }
    }
}
