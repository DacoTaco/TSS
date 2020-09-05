using FluentNHibernate.Testing;
using NUnit.Framework;
using TechnicalServiceSystem.Entities.General;

namespace UnitTests.Mapping.General
{
    [TestFixture]
    public class CompanyMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapCompany()
        {
            //Arrange
            var company = new Company()
            {
                Name = "ATLANTIS LTD"
            };
            //Act&Assert
            new PersistenceSpecification<Company>(Session)
                .VerifyTheMappings(company);
        }
    }
}
