using NUnit.Framework;
using TechnicalServiceSystem.Entities.Users;
using TSS.Web.Feature.Users;

namespace UnitTests.Web.Feature.Users
{
    [TestFixture]
    public class RoleModelTestFixture
    {
        [Test]
        public void CanParseJson()
        {
            //Arrange
            var json = "{\"Role\":\"User\",\"IsChecked\":true}";

            //Act
            var result = RoleModel.TryParse(json);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.IsChecked);
            Assert.That(result.Role, Is.EqualTo(Role.User));
            Assert.AreEqual(nameof(Role.User), result.RoleName);
        }
    }
}
