using NUnit.Framework;
using TechnicalServiceSystem.Utilities;

namespace UnitTests.Utilities
{
    [TestFixture]
    public class AESHandlerTestFixture
    {
        [Test]
        public void CanEncryptAndDecryptContentCorrectly()
        {
            //Arrange
            var input = "suck it jying yang ~Gilfoyle";

            //Act & Assert
            var encrypted = AESHandler.EncryptString(input);
            Assert.AreEqual("4dfe0810d3dac9cdecf71ad4ad03c80ccddefed9d7b762f80399edd6a1cc07e2", encrypted);
            var decrypted = AESHandler.DecryptString(encrypted);
            Assert.AreEqual(input, decrypted);
        }
    }
}
