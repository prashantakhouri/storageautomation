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
using ProductionEntity = Wpp.StorageAutomation.Entities.Models.Production;


namespace Wpp.StorageAutomation.UnitTests.Production.Repository
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
        public async Task GetAllProductions_ShouldListAllProductions()
        {
            var prod = new ProductionEntity()
            {
                Id = "Prod1",
                Name = "prod 1",
                ProductionStoreId = "ProdStore2",
                Wipurl = "ProdStore2/prod 1",
                Status = "ONLINE"
            };

            var productionRepository = new ProductionRepository();
            var items = await productionRepository.GetAllProductions();

            Assert.IsNotNull(items);
            Assert.AreEqual(7, items.ToList().Count);
            Assert.AreEqual(prod.ProductionStoreId, items.First().ProductionStoreId);
        }

        [TestMethod()]
        public async Task AddProduction_ShouldAddProduction()
        {
            var prod = new ProductionEntity()
            {
                Id = "Prod8",
                Name = "prod 8",
                ProductionStoreId = "ProdStore2",
                Wipurl = "ProdStore2/prod 8",
                Status = "ONLINE",
                CreatedDateTime = DateTime.UtcNow
            };

            var productionRepository = new ProductionRepository();

            await productionRepository.AddProduction(prod);
            var items = await productionRepository.GetAllProductions();

            Assert.IsNotNull(items);
            Assert.AreEqual(8, items.ToList().Count);
            Assert.AreEqual(prod.ProductionStoreId, items.Last().ProductionStoreId);
        }

        [TestMethod()]
        public async Task DeleteProduction_ShouldRemoveProduction()
        {
            var ProductionId = "Prod3";

            var productionRepository = new ProductionRepository();

            await productionRepository.DeleteProduction(ProductionId);
            var items = await productionRepository.GetAllProductions();
            var item = await productionRepository.GetProductionByName("ProdStore2", "prod 3");

            Assert.IsNotNull(items);
            Assert.AreEqual(6, items.ToList().Count);
            Assert.IsNull(item);
        }

        [TestMethod()]
        public async Task UpdateProduction_ShouldUpdateProduction()
        {
            var productionRepository = new ProductionRepository();

            var prod = await productionRepository.GetProductionByName("ProdStore3", "prod 4");
            prod.Name = "prod 4 - updated";
            prod.ProductionStoreId = "ProdStore5";
            prod.Wipurl = "ProdStore5/prod 4 - updated";
            prod.ArchiveUrl = "ProdStore5/prod 4 - updated";
            prod.Status = "ARCHIVE";
            prod.CreatedDateTime = DateTime.UtcNow.Date;
            prod.LastSyncDateTime = DateTime.UtcNow.Date;
            prod.SizeInBytes = 24;

            await productionRepository.UpdateProduction(prod);
            var item = await productionRepository.GetProductionByName(prod.ProductionStoreId, prod.Name);

            Assert.IsNotNull(item);
            Assert.AreEqual(prod.Id, item.Id);
            Assert.AreEqual(prod.ArchiveUrl, item.ArchiveUrl);
            Assert.AreEqual(prod.Status, item.Status);
            Assert.AreEqual(prod.CreatedDateTime, item.CreatedDateTime);
            Assert.AreEqual(prod.SizeInBytes, item.SizeInBytes);
        }

        [TestMethod()]
        public async Task GetProductionByProductionStoreId_ShouldListProductionsByProdStoreId()
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
            var items = await productionRepository.GetProductionByProductionStoreId(prod.ProductionStoreId);
            Assert.IsNotNull(items);
            Assert.AreEqual(3, items.ToList().Count);
            Assert.AreEqual(prod.ProductionStoreId, items.First().ProductionStoreId);
        }
    }
}

