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
            var task = Session.QueryOver<Task>().Where(t => t.ID == 1).SingleOrDefault();
            var note = new Note()
            {
                NoteDate = DateTime.Now,
                Text = "TestFixture",
                NoteTask = task
            };

            //Act&Assert
            new PersistenceSpecification<Note>(Session)
                .VerifyTheMappings(note);
        }
    }
}
