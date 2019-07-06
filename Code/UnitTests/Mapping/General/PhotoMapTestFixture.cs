using FluentNHibernate.Testing;
using NUnit.Framework;
using TechnicalServiceSystem.Entities.General;

namespace UnitTests.Mapping.General
{
    [TestFixture]
    public class PhotoMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapPhoto()
        {
            //Arrange
            var photo = new Photo()
            {
                FileName = @"..\lol\random\long\ass\PathNameToCheckIfDataBaseCanHandleItCorrectly\And\Because\we\Can.jpg"
            };
            //Act&Assert
            new PersistenceSpecification<Photo>(TestSession)
                .VerifyTheMappings(photo);
        }
    }
}
