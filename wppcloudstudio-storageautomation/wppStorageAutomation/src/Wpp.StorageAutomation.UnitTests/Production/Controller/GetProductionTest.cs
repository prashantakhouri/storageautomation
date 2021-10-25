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
using GetProductionFunction = Wpp.StorageAutomation.Production.GetProduction;

namespace Wpp.StorageAutomation.UnitTests.Production.Controller
{
    [TestClass()]
    public class GetProductionTest
    {
        private readonly Mock<IProduction> productionMock;
        private readonly Mock<IBaseSecurity> baseSecurityMock;
        private readonly Mock<IProductionStoreRepository> productionStoreRepositoryMock;

        public GetProductionTest()
        {
            productionMock = new Mock<IProduction>();
            baseSecurityMock = new Mock<IBaseSecurity>();
            productionStoreRepositoryMock = new Mock<IProductionStoreRepository>();

        }
        [TestMethod()]
        public async Task GetProductions_Test()
        {
            Environment.SetEnvironmentVariable("AuthorizationEnabled", "false");
            var reqMock = new Mock<HttpRequest>();
            var logMock = new Mock<ILogger>();
            string productionStoreId = "test1";
            var output = new ProductionListResponse();
            List<string> userGrps = new List<string>();
            userGrps.Add("manager group");
            productionMock.Setup(x => x.GetProductionsByProductionStoreAsync(productionStoreId, userGrps)).Returns(Task<ProductionListResponse>.FromResult(output));
            GetProductionFunction getProduction = new GetProductionFunction(productionMock.Object, baseSecurityMock.Object, productionStoreRepositoryMock.Object);
            Response expected = new Response
            {
                Success = true,
                StatusCode = "200-OK"
            };
            Response result = (Response)await getProduction.Get(reqMock.Object, productionStoreId, logMock.Object);
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.StatusCode, result.StatusCode);
            Assert.AreEqual(expected.Success, result.Success);
    }
}
}