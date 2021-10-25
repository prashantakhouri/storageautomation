using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.Production;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.UnitTests.Production.Controller
{
    [TestClass()]
    public class CreateProductionTest
    {
        private readonly Mock<IProduction> productionMock;
        private readonly Mock<IBaseSecurity> baseSecurityMock;
        private readonly Mock<IProductionStoreRepository> productionStoreRepositoryMock;
         
        public CreateProductionTest()
        {
            productionMock = new Mock<IProduction>();
            baseSecurityMock = new Mock<IBaseSecurity>();
            productionStoreRepositoryMock = new Mock<IProductionStoreRepository>();

        }

        [TestMethod()]
        public async Task CreateProduction_Test()
        {
            Environment.SetEnvironmentVariable("AuthorizationEnabled", "false");
            var reqMock = new Mock<HttpRequest>();
            var logMock = new Mock<ILogger>();
            string productionStoreId = "productionstore-dummy";
            var productionRequest = new ProductionRequest()
            {
                ProductionStoreId = "productionstore-dummy",
                ProductionName = "TestProductionName",
                ProductionStoreUri = "/productionstore-dummy",
                DirectoryTree = new List<DirectoryTree>(),
                Tokens = new List<Token>()
            };
            var expectedResponse = new ProductionResponse()
            {
                CreatedDateTime = DateTime.Today,
                Name = "TestProductionName"
            };
            productionMock.Setup(x => x.CreateProduction(productionRequest)).Returns(Task.FromResult(expectedResponse));
            CreateProduction createProductionTest = new CreateProduction(productionMock.Object, baseSecurityMock.Object, productionStoreRepositoryMock.Object);
            var result = await createProductionTest.Create(reqMock.Object, productionStoreId, logMock.Object);

            Assert.IsNotNull(result);
        }
    }
}
