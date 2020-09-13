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
using System.ComponentModel;
using System.Linq;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.Tasks;

namespace UnitTests.Managers
{
    [TestFixture]
    public class TaskManagerTestFixture : NhibernateTestFixture
    {
        private readonly TaskManager _taskManager = new TaskManager();

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

        [Test]
        public void GetTaskStatusesRetrievesCorrectList()
        {
            //Arrange & Act
            var result = _taskManager.GetTaskStatuses();

            //Assert
            Assert.NotNull(result);
            Assert.That(result, Has.Count.EqualTo(9));
            Assert.AreEqual(1, result.First().ID);
            Assert.AreEqual("Being Processed", result.First().Description);
            Assert.AreEqual(9, result.Last().ID);
        }

        [Test]
        public void GetTaskTypesRetrievesCorrectList()
        {
            //Arrange & Act
            var result = _taskManager.GetTaskTypes();

            //Assert
            Assert.NotNull(result);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.AreEqual(1, result.First().ID);
            Assert.AreEqual("Normal Task", result.First().Description);
            Assert.AreEqual(2, result.Last().ID);
        }

        [Test]
        public void TaskAvailableIfTaskOpen()
        {
            //Arrange & Act
            var result = _taskManager.TaskEditable(1, "someHash");

            //Assert
            Assert.True(result);
        }

        [TestCase(nameof(Task.ID), 1, TestName = "Can Sort By ID Asc")]
        [TestCase(nameof(Task.LocationName) + " DESC", 7, TestName = "Can Sort By LocationName Desc")]
        [TestCase(nameof(Task.Description), 3, TestName = "Can Sort By Description")]
        [TestCase(nameof(Task.TechnicianName) + " DESC", 4, TestName = "Can Sort By Technician")]
        [Test]
        public void CanGetTasksSorted(string sortBy, int expectedID)
        {
            //Arrange & Act
            var result = _taskManager.GetTasks(null, sortBy, null);

            //Assert
            Assert.NotNull(result);
            Assert.That(result, Has.Count.GreaterThan(1));
            Assert.AreEqual(expectedID, result.First().ID);
        }

        [Test]
        public void GetTasksIsSetAsDataObjectMethodAttributeForDataGrid()
        {
            //Arrange&Act
            var attributes = typeof(TaskManager).GetMethod(nameof(TaskManager.GetTasks), new[] { typeof(int?), typeof(string), typeof(string) });

            //Assert
            var customAttribute = attributes?.CustomAttributes?.SingleOrDefault(x => x.AttributeType == typeof(DataObjectMethodAttribute));
            Assert.NotNull(customAttribute);
        }
    }
}
