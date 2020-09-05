using FluentNHibernate.Testing;
using NUnit.Framework;
using System;
using TechnicalServiceSystem.Entities.Tasks;

namespace UnitTests.Mapping.Tasks
{
    [TestFixture]
    public class RepeatingInfoMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapRepeatingInfo()
        {
            //Arrange
            var repeatingInfo = new RepeatingInfo()
            {
                ActivationDate = DateTime.Now,
                Interval = TimeSpan.FromDays(20),
                ParentTask = Session.QueryOver<Task>().Where(t => t.ID == 1).SingleOrDefault()
            };

            //Act&Assert
            new PersistenceSpecification<RepeatingInfo>(Session)
                .VerifyTheMappings(repeatingInfo);
        }
    }
}
