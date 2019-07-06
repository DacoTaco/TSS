/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
Copyright(C) 2019 - Joris 'DacoTaco' Vermeylen

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see http://www.gnu.org/licenses */

using FluentNHibernate.Testing;
using NUnit.Framework;
using System;
using System.Linq;
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Tasks;

namespace UnitTests.Mapping
{
    [TestFixture]
    public class TaskMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapTasks()
        {
            var _task = TestSession.QueryOver<Task>()
                .Where(x => x.ID == 7)
                .List()
                .First();

            Assert.NotNull(_task);
            Assert.AreEqual(7,_task.ID);
            Assert.NotNull(_task.Technician);
            Assert.That(_task.TechnicianID,Is.Not.Null,"Technician ID");
            Assert.That(_task.Photos,Is.Not.Null,"Photos List not null");
            Assert.That(_task.Photos.Count,Is.GreaterThan(0),"Photos List is empty");
            Assert.That(_task.Notes.Count,Is.GreaterThan(0),"Notes list is empty");
        }
        [Test]
        public void CanInsertTask()
        {
            //Arrange
            var task = new Task()
            {
                Description = "TestFixture",
                IsUrguent = false,
                CreationDate = DateTime.Now,
                LastModifiedOn = DateTime.Now,
                Reporter = "TestFixture",
                TypeID = 1,
                StatusID = 1,
                Location = TestSession.QueryOver<Location>().Where(l => l.ID == 1).SingleOrDefault()
            };
            task.Notes.Clear();

            //Act&Assert
            new PersistenceSpecification<Task>(TestSession)
                .VerifyTheMappings(task);
        }
    }
}

