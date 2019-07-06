using FluentNHibernate.Testing;
using NUnit.Framework;
using TechnicalServiceSystem.Entities.General;

namespace UnitTests.Mapping.General
{
    [TestFixture]
    class AddressMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapAddress()
        {
            //Arrange
            var address = new Address()
            {
                AddressLine = "Line1",
                ExtraAddressLine = "Line2",
                Nr = 40,
                Bus = "1F",
                PostCode = "9001",
                City = "somewhere",
                Region = "some region",
                Country = "ATLANTIS"
            };
            //Act&Assert
            new PersistenceSpecification<Address>(TestSession)
                .VerifyTheMappings(address);
        }
    }
}
