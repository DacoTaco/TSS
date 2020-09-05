using FluentNHibernate.Testing;
using NUnit.Framework;
using TechnicalServiceSystem.Entities.Tasks;

namespace UnitTests.Mapping.Tasks
{
    [TestFixture]
    public class TaskStatusMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapTaskType()
        {
            //Arrange
            var taskStatus = new TaskStatus()
            {
                Description = "testStatus"
            };

            //Act&Assert
            new PersistenceSpecification<TaskStatus>(Session)
                .VerifyTheMappings(taskStatus);
        }
    }
}
