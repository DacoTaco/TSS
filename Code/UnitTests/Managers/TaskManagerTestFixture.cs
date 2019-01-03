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

using NUnit.Framework;
using TechnicalServiceSystem;

namespace UnitTests.Managers
{
    [TestFixture]
    public class TaskManagerTestFixture
    {
        private TaskManager _taskManager = null;
        [SetUp]
        public void SetupTestFixture()
        {
            _taskManager = new TaskManager();
        }
        [Test]
        public void CanRetrieveSingleTask()
        {
            var _task = _taskManager.GetTasks(Settings.GetCompanyName(), 7);

            Assert.NotNull(_task);
            Assert.AreEqual(7, _task.ID);
            Assert.That(_task.Photos, Is.Not.Null, "Photos List not null");
            Assert.That(_task.Photos.Count, Is.GreaterThan(0), "Photos List is empty");
            Assert.That(_task.Notes.Count, Is.GreaterThan(0), "Notes list is empty");
        }
    }
}
