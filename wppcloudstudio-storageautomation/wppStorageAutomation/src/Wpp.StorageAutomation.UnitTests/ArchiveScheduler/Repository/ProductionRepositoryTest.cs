// <copyright file="ProductionRepositoryTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wpp.StorageAutomation.ArchiveScheduler;
using Wpp.StorageAutomation.Entities.Models;
using ProductionEntity = Wpp.StorageAutomation.Entities.Models.Production;

namespace Wpp.StorageAutomation.UnitTests.ArchiveScheduler.Repository
{
    /// <summary>
    /// Production store repository test.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.UnitTests.ArchiveScheduler.Repository.WppsqldbContextTest" />
    [TestClass]
    public class ProductionRepositoryTest : WppsqldbContextTest
    {
        public ProductionRepositoryTest()
            : base(
            new DbContextOptionsBuilder<WppsqldbContext>()
                .UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=Wpp.StorageAutomation.WppDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
                .Options)
        {
            Environment.SetEnvironmentVariable("WPPSQLDBConnection", @"Data Source=(localdb)\ProjectsV13;Initial Catalog=Wpp.StorageAutomation.WppDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        /// <summary>
        /// Gets all production stores should list all production stores.
        /// </summary>
        /// <returns>A task.</returns>
        [TestMethod]
        public async Task GetAllProductionStores_ShouldListAllProductionStores()
        {
            Environment.SetEnvironmentVariable("WPPSQLDBConnection", @"Data Source=(localdb)\ProjectsV13;Initial Catalog=Wpp.StorageAutomation.WppDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            var prod = new ProductionEntity()
            {
                Id = "ProdStore1",
                Name = "ProdStore Name 1",
            };

            var productionRepository = new ProductionRepository();
            double interval = 15;
            var items = await productionRepository.GetAllValidProductions(interval);

            Assert.IsNotNull(items);
            Assert.AreEqual(5, items.ToList().Count);
            Assert.AreEqual(prod.Id, items.First().Id);
        }
    }
}
