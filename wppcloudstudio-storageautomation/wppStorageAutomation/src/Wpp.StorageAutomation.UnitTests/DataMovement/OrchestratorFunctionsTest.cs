using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Wpp.StorageAutomation.DataMovement;
using Wpp.StorageAutomation.DataMovement.Models;

namespace Wpp.StorageAutomation.UnitTests.DataMovement
{
    [TestClass()]
    public class OrchestratorFunctionsTest
    {
        [TestMethod()]
        public async Task ArchiveOrchestrator_DoesnotThrowException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var response = new ArchiveAllResponse()
            {
                Message = $"Archived successfully",
                ArchiveDate = DateTime.UtcNow
            };
            starterMock.Setup(x => x.CallActivityAsync<ArchiveAllResponse>(It.IsAny<string>(), null)).Returns(Task.FromResult(response));

            // Act
            var result = await OrchestratorFunctions.ArchiveOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ArchiveAllResponse>("ArchiveAsync", null), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveOrchestrator_ThrowsException()
        {
            // Arrange
            var clientMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("An error has occurred");

            clientMock.Setup(x => x.CallActivityAsync<ArchiveAllResponse>(It.IsAny<string>(), null)).Throws(exception);

            //Act
            var result = await OrchestratorFunctions.ArchiveOrchestrator(clientMock.Object, logMock.Object);

            //Assert
            clientMock.Verify(x => x.CallActivityAsync<ArchiveAllResponse>("ArchiveAsync", null), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveOrchestrator_ThrowsSqlException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("A SqlException error has occurred");

            starterMock.Setup(x => x.CallActivityAsync<ArchiveAllResponse>(It.IsAny<string>(), null)).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.ArchiveOrchestrator(starterMock.Object, logMock.Object);


            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ArchiveAllResponse>("ArchiveAsync", null), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveOrchestrator_ThrowsTimeoutException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("A timeout error has occurred");

            starterMock.Setup(x => x.CallActivityAsync<ArchiveAllResponse>(It.IsAny<string>(), null)).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.ArchiveOrchestrator(starterMock.Object, logMock.Object);


            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ArchiveAllResponse>("ArchiveAsync", null), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveProductionStoreOrchestrator_DoesnotThrowException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var productionStoreId = "TestProductionStoreId";
            var response = new ArchiveProductionStoreResponse()
            {
                Message = $"Archived {productionStoreId} successfully",
                Name = productionStoreId,
                ArchiveDate = DateTime.UtcNow
            };

            starterMock.Setup(x => x.GetInput<string>()).Returns(productionStoreId);
            starterMock.Setup(x => x.CallActivityAsync<ArchiveProductionStoreResponse>(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(response));

            // Act - change later
            var result = await OrchestratorFunctions.ArchiveOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ArchiveProductionStoreResponse>("ArchiveProductionStoreAsync", It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(result);

        }

        [TestMethod()]
        public async Task ArchiveProductionStoreOrchestrator_ThrowsException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var productionStoreId = "TestProductionStoreId";
            var exception = new Exception("An error has occurred");

            starterMock.Setup(x => x.GetInput<string>()).Returns(productionStoreId);
            starterMock.Setup(client => client.CallActivityAsync<ArchiveProductionStoreResponse>(It.IsAny<string>(), It.IsAny<string>())).Throws(exception);

            // Act - change later
            var result = await OrchestratorFunctions.ArchiveOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ArchiveProductionStoreResponse>("ArchiveProductionStoreAsync", It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveProductionStoreOrchestrator_ThrowsSqlException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var productionStoreId = "TestProductionStoreId";
            var exception = new Exception("A SqlException error has occurred");

            starterMock.Setup(x => x.GetInput<string>()).Returns(productionStoreId);
            starterMock.Setup(client => client.CallActivityAsync<ArchiveProductionStoreResponse>(It.IsAny<string>(), It.IsAny<string>())).Throws(exception);

            // Act - change later
            var result = await OrchestratorFunctions.ArchiveOrchestrator(starterMock.Object, logMock.Object);


            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ArchiveProductionStoreResponse>("ArchiveProductionStoreAsync", It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(result);

        }

        [TestMethod()]
        public async Task ArchiveProductionStoreOrchestrator_ThrowsTimeoutException()
        {
            // Arrange
            var startMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var productionStoreId = "TestProductionStoreId";
            var exception = new Exception("A timeout error has occurred");

            startMock.Setup(x => x.GetInput<string>()).Returns(productionStoreId);
            startMock.Setup(client => client.CallActivityAsync<ArchiveProductionStoreResponse>(It.IsAny<string>(), It.IsAny<string>())).Throws(exception);

            // Act - change later
            var result = await OrchestratorFunctions.ArchiveOrchestrator(startMock.Object, logMock.Object);

            // Assert
            startMock.Verify(x => x.CallActivityAsync<ArchiveProductionStoreResponse>("ArchiveProductionStoreAsync", It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(result);

        }

        [TestMethod()]
        public async Task ArchiveProductionStoreOrchestrator_ThrowsDoesnotExistException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var productionStoreId = "TestProductionStoreId";
            var exception = new Exception("does not exist in WIP");

            starterMock.Setup(x => x.GetInput<string>()).Returns(productionStoreId);
            starterMock.Setup(client => client.CallActivityAsync<ArchiveProductionStoreResponse>(It.IsAny<string>(), It.IsAny<string>())).Throws(exception);

            // Act - change later
            var result = await OrchestratorFunctions.ArchiveOrchestrator(starterMock.Object, logMock.Object);


            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ArchiveProductionStoreResponse>("ArchiveProductionStoreAsync", It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(result);

        }

        [TestMethod()]
        public async Task ArchiveProductionOrchestrator_DoesnotThrowException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var archiveProductionRequest = new ProductionRequest()
            {
                ProductionId = "TestProductionId",
                ProductionStoreId = "TestProductionStoreId"
            };
            var response = new ProductionResponse();

            starterMock.Setup(x => x.GetInput<ProductionRequest>()).Returns(archiveProductionRequest);
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Returns(Task.FromResult(response));

            // Act
            var result = await OrchestratorFunctions.ArchiveProductionOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("ArchiveProductionAsync", It.IsAny<ProductionRequest>()), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveProductionOrchestrator_ThrowsException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var archiveProductionRequest = new ProductionRequest()
            {
                ProductionId = "TestProductionId",
                ProductionStoreId = "TestProductionStoreId"
            };
            var exception = new Exception("An error has occurred");

            starterMock.Setup(x => x.GetInput<ProductionRequest>()).Returns(archiveProductionRequest);
            starterMock.Setup(client => client.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            //Act
            var result = await OrchestratorFunctions.ArchiveProductionOrchestrator(starterMock.Object, logMock.Object);

            //Assert
            starterMock.Verify(client => client.CallActivityAsync<ProductionResponse>("ArchiveProductionAsync", It.IsAny<ProductionRequest>()), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveProductionOrchestrator_ThrowsSqlException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var archiveProductionRequest = new ProductionRequest()
            {
                ProductionId = "TestProductionId",
                ProductionStoreId = "TestProductionStoreId"
            };
            var exception = new Exception("A SqlException error has occurred");

            starterMock.Setup(x => x.GetInput<ProductionRequest>()).Returns(archiveProductionRequest);
            starterMock.Setup(client => client.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            //Act
            var result = await OrchestratorFunctions.ArchiveProductionOrchestrator(starterMock.Object, logMock.Object);

            //Assert
            starterMock.Verify(client => client.CallActivityAsync<ProductionResponse>("ArchiveProductionAsync", It.IsAny<ProductionRequest>()), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveProductionOrchestrator_ThrowsTimeoutException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var archiveProductionRequest = new ProductionRequest()
            {
                ProductionId = "TestProductionId",
                ProductionStoreId = "TestProductionStoreId"
            };
            var exception = new Exception("A timeout error has occurred");

            starterMock.Setup(x => x.GetInput<ProductionRequest>()).Returns(archiveProductionRequest);
            starterMock.Setup(client => client.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            //Act
            var result = await OrchestratorFunctions.ArchiveProductionOrchestrator(starterMock.Object, logMock.Object);

            //Assert
            starterMock.Verify(client => client.CallActivityAsync<ProductionResponse>("ArchiveProductionAsync", It.IsAny<ProductionRequest>()), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveProductionOrchestrator_ThrowsDoesnotExistException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var archiveProductionRequest = new ProductionRequest()
            {
                ProductionId = "TestProductionId",
                ProductionStoreId = "TestProductionStoreId"
            };
            var exception = new Exception("does not exist in WIP");

            starterMock.Setup(x => x.GetInput<ProductionRequest>()).Returns(archiveProductionRequest);
            starterMock.Setup(client => client.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            //Act
            var result = await OrchestratorFunctions.ArchiveProductionOrchestrator(starterMock.Object, logMock.Object);

            //Assert
            starterMock.Verify(client => client.CallActivityAsync<ProductionResponse>("ArchiveProductionAsync", It.IsAny<ProductionRequest>()), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task RestoreProductionOrchestrator_DoesnotThrowException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            ProductionResponse expectedResponse = new ProductionResponse();

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Returns(Task.FromResult(expectedResponse));

            // Act
            var result = await OrchestratorFunctions.RestoreProductionOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("RestoreProductionAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task RestoreProductionOrchestrator_ThrowsException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("An error has occurred");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.RestoreProductionOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("RestoreProductionAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task RestoreProductionOrchestrator_ThrowsDoesnotExistException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("does not exist in Archive");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.RestoreProductionOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("RestoreProductionAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task RestoreProductionOrchestrator_ThrowsSqlException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("A SqlException error has occurred");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.RestoreProductionOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("RestoreProductionAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task RestoreProductionOrchestrator_ThrowsTimeoutException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("A timeout error has occurred");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.RestoreProductionOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("RestoreProductionAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task RestoreProductionOrchestrator_StatusOnlineException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("already exists in WIP");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.RestoreProductionOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("RestoreProductionAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task RestoreProductionOrchestrator_MetadataFileNotFoundException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("was not found in Archive. As a result, created date and updated date didn't get preserved. Empty directories are lost.");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.RestoreProductionOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("RestoreProductionAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task MakeProductionOfflineOrchestrator_DoesnotThrowException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            ProductionResponse expectedResponse = new ProductionResponse();

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Returns(Task.FromResult(expectedResponse));

            //Act
            var result = await OrchestratorFunctions.MakeProductionOfflineOrchestrator(starterMock.Object, logMock.Object);

            //Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("MakeProductionOfflineAsync", It.IsAny<ProductionResponse>()), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task MakeProductionOfflineOrchestrator_ThrowsException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("An error has occurred");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.MakeProductionOfflineOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("MakeProductionOfflineAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task MakeProductionOfflineOrchestrator_ThrowsSqlException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("A SqlException error has occurred");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.MakeProductionOfflineOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("MakeProductionOfflineAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task MakeProductionOfflineOrchestrator_ThrowsTimeoutException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("timeout");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.MakeProductionOfflineOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("MakeProductionOfflineAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task MakeProductionOfflineOrchestrator_AlreadyOfflineException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("already offline");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.MakeProductionOfflineOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("MakeProductionOfflineAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task MakeProductionOfflineOrchestrator_DoesNotExistException()
        {
            // Arrange
            var starterMock = new Mock<IDurableOrchestrationContext>();
            var logMock = new Mock<ILogger>();
            var exception = new Exception("does not exist");

            ProductionRequest request = starterMock.Object.GetInput<ProductionRequest>();
            starterMock.Setup(x => x.CallActivityAsync<ProductionResponse>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Throws(exception);

            // Act
            var result = await OrchestratorFunctions.MakeProductionOfflineOrchestrator(starterMock.Object, logMock.Object);

            // Assert
            starterMock.Verify(x => x.CallActivityAsync<ProductionResponse>("MakeProductionOfflineAsync", request), Times.Once);
            Assert.IsNotNull(result);
        }
    }
}
