using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wpp.StorageAutomation.UnitTests.Common
{
    [TestClass()]
    public class ExtensionMethodTest
    {
        [TestMethod()]
        public void RemoveSpecialChars_RemovesSpecialChars()
        {
            var validText = "Hello World";
            var invalidText = "He//o W@rld.,";

            var result_valid = ExtensionMethod.ExtensionMethod.RemoveSpecialChars(validText);
            var result_invalid = ExtensionMethod.ExtensionMethod.RemoveSpecialChars(invalidText);

            Assert.IsNotNull(result_valid);
            Assert.AreEqual(validText, result_valid);
            Assert.AreEqual("Heo Wrld", result_invalid);
        }

        [TestMethod()]
        public void RemoveSlash_RemovesSlash()
        {
            var validText = "Hello World";
            var invalidText = "He//o W@rld.";

            var result_valid = ExtensionMethod.ExtensionMethod.RemoveSlash(validText);
            var result_invalid = ExtensionMethod.ExtensionMethod.RemoveSlash(invalidText);

            Assert.IsNotNull(result_valid);
            Assert.AreEqual(validText, result_valid);
            Assert.AreEqual("Heo Wrld", result_invalid);
        }
    }
}
