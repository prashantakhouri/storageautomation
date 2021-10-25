using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Wpp.StorageAutomation.Common;

namespace Wpp.StorageAutomation.UnitTests.Common
{
    [TestClass()]
    public class ProductionStoreValidatorTest
    {
        private readonly ProductionStoreIdValidator productionStoreValidator;

        public ProductionStoreValidatorTest()
        {
            productionStoreValidator = new ProductionStoreIdValidator();
        }

        [TestMethod()]
        public async Task ProductionStoreValidator_ValidatesProductionStore()
        {
            // Arrange
            var validProductionStore = "test-productionstore";
            var invalidProductionStore = "Test+ProdStore";

            try
            {
                // Act
                var result_valid = await productionStoreValidator.ValidateAsync(validProductionStore);
                var result_invalid = await productionStoreValidator.ValidateAsync(invalidProductionStore);

                // Assert
                Assert.IsNotNull(result_valid);
                Assert.AreEqual(true, result_valid.IsValid);

                Assert.IsNotNull(result_invalid);
                Assert.AreEqual(false, result_invalid.IsValid);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}