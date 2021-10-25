using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Wpp.StorageAutomation.DataMovement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wpp.StorageAutomation.DataMovement.Models;
using Wpp.StorageAutomation.DataMovement.Repository;

namespace Wpp.StorageAutomation.UnitTests.DataMovement
{
    [TestClass()]
    public class ArchiveProductionTest : ControllerBase
    {
        private readonly Mock<IProductionStoreRepository> productionStoreRepositoryMock;
        private readonly Mock<IProductionRepository> productionRepositoryMock;
        private readonly Archive archive;

        public ArchiveProductionTest()
        {
            Environment.SetEnvironmentVariable("AuthorizationEnabled", "false");
            this.productionStoreRepositoryMock = new Mock<IProductionStoreRepository>();
            this.productionRepositoryMock = new Mock<IProductionRepository>();
            this.archive = new Archive(this.productionStoreRepositoryMock.Object, this.productionRepositoryMock.Object);
        }

        [TestMethod()]
        public async Task ArchiveStarter_DoesnotThrowException()
        {
            // Arrange
            var requestMock = new Mock<HttpRequest>();
            var clientMock = new Mock<IDurableOrchestrationClient>();
            var logMock = new Mock<ILogger>();
            var id = "8e503c5e-19de-40e1-932d-298c4263115c";
            var expectedResponse = Ok();

            clientMock.Setup(client => client.StartNewAsync(It.IsAny<string>(), null)).Returns(Task.FromResult<string>(id));
            clientMock.Setup(client => client.CreateCheckStatusResponse(It.IsAny<HttpRequest>(), It.IsAny<string>(), false)).Returns(expectedResponse);

            // Act
            var result = await archive.ArchiveAll(requestMock.Object, clientMock.Object, logMock.Object);

            // Assert
            clientMock.Verify(client => client.StartNewAsync("ArchiveOrchestrator", null), Times.Once);
            clientMock.Verify(client => client.CreateCheckStatusResponse(requestMock.Object, id, false), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveProductionStoreStarter_DoesnotThrowException()
        {
            // Arrange
            var requestMock = new Mock<HttpRequest>();
            var productionStoreId = "testproductionstore-id";
            var clientMock = new Mock<IDurableOrchestrationClient>();
            var logMock = new Mock<ILogger>();
            var id = "8e503c5e-19de-40e1-932d-298c4263115c";
            var expectedResponse = Ok();

            clientMock.Setup(client => client.StartNewAsync<string>(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<string>(id));
            clientMock.Setup(client => client.CreateCheckStatusResponse(It.IsAny<HttpRequest>(), It.IsAny<string>(), false)).Returns(expectedResponse);

            // Act
            var result = await archive.ArchiveProductionStore(requestMock.Object, productionStoreId, clientMock.Object, logMock.Object);

            // Assert
            clientMock.Verify(client => client.StartNewAsync<string>("ArchiveProductionStoreOrchestrator", It.IsAny<string>()), Times.Once);
            clientMock.Verify(client => client.CreateCheckStatusResponse(requestMock.Object, id, false), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveProductionStoreStarter_ValidatesRequests()
        {
            // Arrange
            var requestMock = new Mock<HttpRequest>();
            var productionStoreId = "$pec1@l Charac+er$";
            var clientMock = new Mock<IDurableOrchestrationClient>();
            var logMock = new Mock<ILogger>();
            var id = "8e503c5e-19de-40e1-932d-298c4263115c";
            var expectedResponse = Ok();

            clientMock.Setup(client => client.StartNewAsync<string>(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<string>(id));
            clientMock.Setup(client => client.CreateCheckStatusResponse(It.IsAny<HttpRequest>(), It.IsAny<string>(), false)).Returns(expectedResponse);

            // Act
            var result = await archive.ArchiveProductionStore(requestMock.Object, productionStoreId, clientMock.Object, logMock.Object);

            // Assert
            clientMock.Verify(client => client.StartNewAsync<string>("ArchiveProductionStoreOrchestrator", It.IsAny<string>()), Times.Never);
            clientMock.Verify(client => client.CreateCheckStatusResponse(requestMock.Object, id, false), Times.Never);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveProductionStarter_DoesnotThrowException()
        {
            // Arrange
            var requestMock = new Mock<HttpRequest>();
            var productionStoreId = "TestProductionStoreId";
            var productionId = "TestProductionId";
            var clientMock = new Mock<IDurableOrchestrationClient>();
            var logMock = new Mock<ILogger>();
            var id = "8e503c5e-19de-40e1-932d-298c4263115c";
            var expectedResponse = Ok();

            clientMock.Setup(client => client.StartNewAsync<ProductionRequest>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Returns(Task.FromResult<string>(id));
            clientMock.Setup(client => client.CreateCheckStatusResponse(It.IsAny<HttpRequest>(), It.IsAny<string>(), false)).Returns(expectedResponse);

            // Act
            var result = await archive.ArchiveProduction(requestMock.Object, productionStoreId, productionId, clientMock.Object, logMock.Object);

            // Assert
            clientMock.Verify(client => client.StartNewAsync<ProductionRequest>("ArchiveProductionOrchestrator", It.IsAny<ProductionRequest>()), Times.Once);
            clientMock.Verify(client => client.CreateCheckStatusResponse(requestMock.Object, id, false), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task ArchiveProductionStarter_ValidatesRequests()
        {
            // Arrange
            var requestMock = new Mock<HttpRequest>();
            var productionStoreId = "$pec1@l Charac+er$";
            var productionId = "$pec1@l Charac+er$ 2";
            var clientMock = new Mock<IDurableOrchestrationClient>();
            var logMock = new Mock<ILogger>();
            var id = "8e503c5e-19de-40e1-932d-298c4263115c";
            var expectedResponse = Ok();

            clientMock.Setup(client => client.StartNewAsync<ProductionRequest>(It.IsAny<string>(), It.IsAny<ProductionRequest>())).Returns(Task.FromResult<string>(id));
            clientMock.Setup(client => client.CreateCheckStatusResponse(It.IsAny<HttpRequest>(), It.IsAny<string>(), false)).Returns(expectedResponse);

            // Act
            var result = await archive.ArchiveProduction(requestMock.Object, productionStoreId, productionId, clientMock.Object, logMock.Object);

            // Assert
            clientMock.Verify(client => client.StartNewAsync<ProductionRequest>("ArchiveProductionOrchestrator", It.IsAny<ProductionRequest>()), Times.Never);
            clientMock.Verify(client => client.CreateCheckStatusResponse(requestMock.Object, id, false), Times.Never);
            Assert.IsNotNull(result);
        }
    }
}
