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

using System.Linq;
using NUnit.Framework;
using TechnicalServiceSystem.Entities.Tasks;

namespace UnitTests.Mapping
{
    [TestFixture]
    public class TaskMappingsTestFixture : NhibernateTestFixture
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
    }
}

