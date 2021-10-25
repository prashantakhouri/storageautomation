using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Wpp.StorageAutomation.DataMovement.Contracts;
using Wpp.StorageAutomation.DataMovement.Models;

namespace Wpp.StorageAutomation.UnitTests.DataMovement
{
    [TestClass()]
    public class ActivityFunctionsTest
    {
        private readonly Mock<IDataMovement> dataMovementMock;
        private readonly Wpp.StorageAutomation.DataMovement.ActivityFunctions activityFunctions;
        public ActivityFunctionsTest()
        {
            dataMovementMock = new Mock<IDataMovement>();
            activityFunctions = new Wpp.StorageAutomation.DataMovement.ActivityFunctions(dataMovementMock.Object);
        }

        [TestMethod()]
        public async Task ArchiveAsync_DoesnotThrowException()
        {
            // Arrange
            var name = String.Empty;
            var logMock = new Mock<ILogger>();
            var response = new ArchiveAllResponse()
            {
                Message = $"Archived successfully",
                ArchiveDate = DateTime.UtcNow
            };
            this.dataMovementMock.Setup(x => x.ArchiveAsync()).Returns(Task.FromResult(response));

            // Act
            var result = await activityFunctions.ArchiveAsync(name, logMock.Object);

            // Assert
            this.dataMovementMock.Verify(x => x.ArchiveAsync(), Times.Once);
            Assert.AreEqual(expected: response.ArchiveDate, actual: result.ArchiveDate);
            Assert.AreEqual(expected: response.Message, actual: result.Message);
        }

        [TestMethod()]
        public async Task ArchiveProductionStoreAsync_DoesnotThrowException()
        {
            // Arrange
            var productionStoreId = "TestProductionStoreId";
            var logMock = new Mock<ILogger>();
            var response = new ArchiveProductionStoreResponse() 
            {
                Message = $"Archived {productionStoreId} successfully",
                Name=productionStoreId,
                ArchiveDate = DateTime.UtcNow
            };
            this.dataMovementMock.Setup(x => x.ArchiveProductionStoreAsync(It.IsAny<string>())).Returns(Task.FromResult(response));


            // Act
            var result = await activityFunctions.ArchiveProductionStoreAsync(productionStoreId, logMock.Object);

            // Assert
            this.dataMovementMock.Verify(x => x.ArchiveProductionStoreAsync(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(expected: response.Name, actual: result.Name);
            Assert.AreEqual(expected: response.Message, actual: result.Message);
        }

        [TestMethod()]
        public async Task ArchiveProductionAsync_SuccessFlag_Is_True()
        {
            // Arrange
            var archiveProductionRequest = new ProductionRequest()
            {
                ProductionId = "TestProductionId",
                ProductionStoreId = "TestProductionStoreId"
            };
            var logMock = new Mock<ILogger>();
            this.dataMovementMock.Setup(x => x.ArchiveProductionAsync(It.IsAny<ProductionRequest>())).Returns(Task.FromResult(new ArchiveProductionResponse()));

            // Act
            var result = await activityFunctions.ArchiveProductionAsync(archiveProductionRequest, logMock.Object);

            // Assert
            this.dataMovementMock.Verify(x => x.ArchiveProductionAsync(It.IsAny<ProductionRequest>()), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task Run_RestoreProductionAsync()
        {
            // Arrange
            var request = new ProductionRequest()
            {
                ProductionStoreId = "testProdStoreId123",
                ProductionId = "testProdId123"
            };
            var logMock = new Mock<ILogger>();
            var resultExpected = new ProductionResponse();
            resultExpected.Status = "online";
            this.dataMovementMock.Setup(x => x.RestoreProductionAsync(It.IsAny<ProductionRequest>())).Returns(Task.FromResult(resultExpected));

            // Act
            var result = await activityFunctions.RestoreProductionAsync(request, logMock.Object);

            // Assert
            Assert.AreEqual(expected: resultExpected.Id, actual: result.Id);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task Run_MakeProductionOfflineAsync()
        {
            // Arrange
            var request = new ProductionRequest()
            {
                ProductionStoreId = "testProdStoreId123",
                ProductionId = "testProdId123"
            };
            var logMock = new Mock<ILogger>();
            var resultExpected = new ProductionResponse();
            resultExpected.Status = "online";
            this.dataMovementMock.Setup(x => x.MakeProductionOfflineAsync(It.IsAny<ProductionRequest>())).Returns(Task.FromResult(resultExpected));

            // Act
            var result = await activityFunctions.MakeProductionOfflineAsync(request, logMock.Object);

            // Assert
            Assert.AreEqual(expected: resultExpected.Id, actual: result.Id);
            Assert.IsNotNull(result);

        }
    }
}
