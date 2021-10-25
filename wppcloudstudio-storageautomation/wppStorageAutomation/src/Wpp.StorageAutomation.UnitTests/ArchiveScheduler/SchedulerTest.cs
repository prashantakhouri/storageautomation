using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.ArchiveScheduler;
using Wpp.StorageAutomation.Common;
using ProductionEntity = Wpp.StorageAutomation.Entities.Models.Production;

namespace Wpp.StorageAutomation.UnitTests.ArchiveScheduler
{
    public class SchedulerTest
    {
        private readonly Scheduler scheduler;
        private readonly Mock<IProductionRepository> productionRepositoryMock;
        private readonly Mock<ICloudStorageUtility> cloudStorageUtilitymock;
        private readonly Mock<Wpp.StorageAutomation.Common.IStorageAccountConfig> storageAccountConfigMock;

        public SchedulerTest()
        {
            productionRepositoryMock = new Mock<IProductionRepository>();
            storageAccountConfigMock = new Mock<Wpp.StorageAutomation.Common.IStorageAccountConfig>();
            cloudStorageUtilitymock = new Mock<Wpp.StorageAutomation.Common.ICloudStorageUtility>();
            scheduler = new Scheduler(productionRepositoryMock.Object, storageAccountConfigMock.Object, cloudStorageUtilitymock.Object);
        }

        public async Task ProductionValidator_ValidatesProductions()
        {
            TimerInfo myTimer = null;
            var logMock = new Mock<ILogger>();
            var productions = new List<ProductionEntity> 
            { 
                new ProductionEntity
                {
                    Id = "Id1",
                    Name = "Name1"
                },
                new ProductionEntity
                {
                    Id = "Id2",
                    Name = "Name2"
                },
                new ProductionEntity
                {
                    Id = "Id3",
                    Name = "Name3"
                }
            };
            double durationTime = 15;
            productionRepositoryMock.Setup(x => x.GetAllOnlineProductions(durationTime)).ReturnsAsync(productions);

            // Act
            await scheduler.Run(myTimer, logMock.Object);

            Assert.IsTrue(true);
        }
    }
}
