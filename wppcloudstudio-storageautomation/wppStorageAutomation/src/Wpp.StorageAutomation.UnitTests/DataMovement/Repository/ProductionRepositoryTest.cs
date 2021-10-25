using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wpp.StorageAutomation.DataMovement.Repository;
using Wpp.StorageAutomation.Entities.Models;
using Wpp.StorageAutomation.UnitTests.DataMovement.Repository;
using ProductionEntity = Wpp.StorageAutomation.Entities.Models.Production;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Wpp.StorageAutomation.UnitTests.DataMovement
{
    [TestClass()]
    public class ProductionRepositoryTest : WppsqldbContextTest
    {
        public ProductionRepositoryTest() : base(
            new DbContextOptionsBuilder<WppsqldbContext>()
                .UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=Wpp.StorageAutomation.WppDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
                .Options)
        {
            Environment.SetEnvironmentVariable("WPPSQLDBConnection", @"Data Source=(localdb)\ProjectsV13;Initial Catalog=Wpp.StorageAutomation.WppDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        [TestMethod()]
        public async Task GetOnlineProductionsByProdStoreId_ShouldListProductionsByProdStoreId()
        {
            var prod = new ProductionEntity()
            {
                Id = "Prod1",
                Name = "prod 1",
                ProductionStoreId = "ProdStore2",
                Wipurl = "ProdStore2/prod 1",
                Status = "Online"
            };

            var productionRepository = new ProductionRepository();
            var items = await productionRepository.GetOnlineProductionsByProdStoreId(prod.ProductionStoreId);

            Assert.IsNotNull(items);
            Assert.AreEqual(2, items.ToList().Count);
            Assert.AreEqual(prod.ProductionStoreId, items.First().ProductionStoreId);
        }

        [TestMethod()]
        public async Task UpdateLastSyncDateByProdStoreId_ShouldUpdateProduction()
        {
            var productionStoreId = "ProdStore2";
            var lastSyncDate = DateTime.UtcNow.Date;

            var productionRepository = new ProductionRepository();

            await productionRepository.UpdateLastSyncDateByProdStoreId(productionStoreId, lastSyncDate);
            var item = await productionRepository.GetProductionByName("ProdStore2", "prod 3");

            Assert.IsNotNull(item);
            Assert.AreEqual(lastSyncDate, item.LastSyncDateTime);
        }

        [TestMethod()]
        public async Task UpdateProduction_ShouldUpdateProduction()
        {
            var prod = new ProductionEntity()
            {
                Id = "Prod4",
                Name = "prod 4 - updated",
                ProductionStoreId = "ProdStore5",
                Wipurl = "ProdStore5/prod 4",
                Status = "Online",
                CreatedDateTime = DateTime.UtcNow
            };

            var productionRepository = new ProductionRepository();

            await productionRepository.UpdateProduction(prod);
            var item = await productionRepository.GetProductionByName("ProdStore5", "prod 4 - updated");

            Assert.IsNotNull(item);
            Assert.AreEqual("Prod4", item.Id);
        }
    }
}
