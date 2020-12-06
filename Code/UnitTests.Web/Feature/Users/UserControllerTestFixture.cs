using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnitTests.Web.Feature.Users
{
    [TestFixture]
    public class UserControllerTestFixture : ControllerTestFixture
    {
        [Test]
        [TestCase("Admin", true, TestName = "Admin")]
        [TestCase("Technician", false, TestName = "Technician")]
        public async Task CanVerifyIfAdminRoleIsAdmin(string roleName, bool expectedResult)
        {
            //Arrange & Act
            var data = await GetAsync($"api/User/Role/{roleName}/IsAdmin").ConfigureAwait(false);
            var result = await data.Content.ReadAsAsync<bool>().ConfigureAwait(false);

            //Assert
            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
