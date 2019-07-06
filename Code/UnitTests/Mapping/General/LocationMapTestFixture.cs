using FluentNHibernate.Testing;
using NUnit.Framework;
using TechnicalServiceSystem.Entities.General;

namespace UnitTests.Mapping.General
{
    [TestFixture]
    public class LocationMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapLocation()
        {
            //Arrange
            var location = new Location()
            {
                Department = new Department() { Description = "lol"},
                Description = "SOME Random Location"
            };
            //Act&Assert
            new PersistenceSpecification<Location>(TestSession)
                .VerifyTheMappings(location);
        }
    }
}
