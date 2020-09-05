using FluentNHibernate.Testing;
using NUnit.Framework;
using TechnicalServiceSystem.Entities.Users;
using TechnicalServiceSystem.Entities.General;

namespace UnitTests.Mapping.Users
{
    [TestFixture]
    public class UserMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapUser()
        {
            //Arrange
            var user = new User()
            {
                UserName = "TextFixture Username",
                IsActive = false,
                Department = new Department(5) {Description = "test department"},
                Photo = new Photo() { FileName = "UserMapTest.jpg"}
            };

            //Act&Assert
            new PersistenceSpecification<User>(Session)
                .VerifyTheMappings(user);
        }
    }
}
