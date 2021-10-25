using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.ProductionStore;
using Wpp.StorageAutomation.ProductionStore.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.UnitTests.ProductionStore.Controllers
{
    [TestClass()]
    public class ListProductionStoreTest
    {
        private readonly Mock<IProductionStores> productionStoresMock;
        private readonly Mock<IBaseSecurity> baseSecurityMock;
        private readonly ListProductionStore listProductionStore;
        public ListProductionStoreTest()
        {
            Environment.SetEnvironmentVariable("AuthorizationEnabled", "false");
            productionStoresMock = new Mock<IProductionStores>();
            baseSecurityMock = new Mock<IBaseSecurity>();
            listProductionStore = new ListProductionStore(productionStoresMock.Object, baseSecurityMock.Object);
        }

        [TestMethod()]
        public async Task ListProductionStore_DoesnotThrowException()
        {
            // Arrange
            var reqMock = new Mock<HttpRequest>();
            var logMock = new Mock<ILogger>();
            List<string> userGrps = new List<string>();

            userGrps.Add("manager group");

            var response = new ProductionStoreListResponse
            {
                ProductionStoreList = new List<ProductionStoreRow>
                {
                    new ProductionStoreRow
                    {
                        Id = "TestId1",
                        Name = "TestName1"
                    },
                    new ProductionStoreRow
                    {
                        Id = "TestId2",
                        Name = "TestName2"
                    }
                }
            };

            productionStoresMock.Setup(x => x.ListProductionStoresAsync(userGrps)).ReturnsAsync(response);

            // Act
            var result = await listProductionStore.List(reqMock.Object, logMock.Object);

            // Assert
            this.productionStoresMock.Verify(x => x.ListProductionStoresAsync(userGrps), Times.Once);
            Assert.IsNotNull(result);
        }
    }
}
