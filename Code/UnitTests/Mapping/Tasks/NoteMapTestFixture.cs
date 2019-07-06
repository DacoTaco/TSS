using FluentNHibernate.Testing;
using NUnit.Framework;
using System;
using TechnicalServiceSystem.Entities.Tasks;

namespace UnitTests.Mapping
{
    [TestFixture]
    public class NoteMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapTaskNote()
        {
            //Arrange
            var note = new Note()
            {
                NoteDate = DateTime.Now,
                Text = "TestFixture",
                NoteTask = TestSession.QueryOver<Task>().Where(t => t.ID == 1).SingleOrDefault()
            };

            //Act&Assert
            new PersistenceSpecification<Note>(TestSession)
                .VerifyTheMappings(note);
        }
    }
}
