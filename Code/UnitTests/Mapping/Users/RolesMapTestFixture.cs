using FluentNHibernate.Testing;
using NUnit.Framework;
using TechnicalServiceSystem.Entities.Users;

namespace UnitTests.Mapping.Users
{
    [TestFixture]
    public class RolesMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapRoles()
        {
            //Arrange
            var role = new Role()
            {
                RoleName = "Test Role"
            };

            //Act&Assert
            new PersistenceSpecification<Role>(Session)
                .VerifyTheMappings(role);
        }
    }
}
