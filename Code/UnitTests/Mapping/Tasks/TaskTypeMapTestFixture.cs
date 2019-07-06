using FluentNHibernate.Testing;
using NUnit.Framework;
using TechnicalServiceSystem.Entities.Tasks;

namespace UnitTests.Mapping.Tasks
{
    [TestFixture]
    public class TaskTypeMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapTaskType()
        {
            //Arrange
            var taskType = new TaskType()
            {
                Description = "testType"
            };

            //Act&Assert
            new PersistenceSpecification<TaskType>(TestSession)
                .VerifyTheMappings(taskType);
        }
    }
}
