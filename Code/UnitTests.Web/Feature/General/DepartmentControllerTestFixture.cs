using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TSS.Web.Feature.General;

namespace UnitTests.Web.Feature.General
{
    [TestFixture]
    public class DepartmentControllerTestFixture : ControllerTestFixture
    {
        [Test]
        public async Task GetLocationsByDepartment()
        {
            //Arrange & Act
            var data = await GetAsync("api/Department/2/Locations").ConfigureAwait(false);
            var list = await data.Content.ReadAsAsync<IList<LocationModel>>().ConfigureAwait(false);

            //Assert
            Assert.NotNull(list);
            Assert.That(list, Has.Count.EqualTo(7));
            Assert.True(list.Any(x => x.Description == "Badkamer"));
        }
    }
}
