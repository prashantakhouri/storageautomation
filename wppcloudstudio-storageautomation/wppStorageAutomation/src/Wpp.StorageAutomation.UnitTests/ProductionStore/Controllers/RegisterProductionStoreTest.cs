using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.ProductionStore;
using Wpp.StorageAutomation.ProductionStore.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Newtonsoft.Json;

namespace Wpp.StorageAutomation.UnitTests.ProductionStore.Controllers
{
    [TestClass()]
    public class RegisterProductionStoreTest
    {
        private readonly Mock<IProductionStores> productionStoresMock;
        private readonly RegisterProductionStore registerProductionStore;
        public RegisterProductionStoreTest()
        {
            Environment.SetEnvironmentVariable("AuthorizationEnabled", "false");
            productionStoresMock = new Mock<IProductionStores>();
            registerProductionStore = new RegisterProductionStore(productionStoresMock.Object);
        }

        private static Mock<HttpRequest> CreateMockRequest(object body)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }

        [TestMethod()]
        public async Task RegisterProductionStore_Test()
        {
            // Arrange
            
            var logMock = new Mock<ILogger>();
            var request = new ProductionStoreRequest
            {
                Name = "test-productionstore",
                Region = "Europe",
                ArchiveURL = "/test-productionstore",
                ManagerRoleGroupNames = "TestAdmin",
                UserRoleGroupNames = "TestUser",
                WIPURL = "/test-productionstore"
            };
            var requestMock = CreateMockRequest(request);
            var response = new ProductionStoreResponse
            {
                Name = "test-productionstore",
                CreatedDate = DateTime.UtcNow
            };

            productionStoresMock.Setup(x => x.SaveProductionStore(request)).Returns(Task.FromResult(response));

            // Act
            var result = await registerProductionStore.Register(requestMock.Object, logMock.Object);

            // Assert
            Assert.IsNotNull(result);
        }

    }
}
