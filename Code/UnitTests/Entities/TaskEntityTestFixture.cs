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

using NHibernate.Type;
using NUnit.Framework;
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Tasks;
using TechnicalServiceSystem.Entities.Users;

namespace UnitTests.Entities
{
    [TestFixture]
    public class TaskEntityTestFixture
    {
        private Task _task;

        [SetUp]
        public void SetUpEntity()
        {
            _task = new Task()
            {
                ID = 1,
                Location = new Location()
                {
                    ID = 1,
                    Department = new Department()
                    {
                        ID = 5
                    }
                }
            };
        }

        [Test]
        public void LocationDepartmentLinkIsOk()
        {
            Assert.AreEqual(_task.Department.ID , _task.DepartmentID, 5,"Department ID");
            Assert.AreEqual(_task.Location.Department,_task.Department);
        }

        [Test]
        public void IfTechnicianSetStatusChanged()
        {
            var tech = new User() {ID = 5};
            var task = new Task()
            {
                ID = 1,
                StatusID = 2,
                Technician = new User() {ID = 2}
            };

            Assert.That(task.TechnicianID, Is.EqualTo(2),"Initial value of Technician ID");
            Assert.That(task.StatusID, Is.EqualTo(2),"Initial Value of Status ID");

            task.Technician = null;

            Assert.That(task.TechnicianID, Is.EqualTo(0),"Technician ID after technician is set null");
            Assert.That(task.StatusID, Is.EqualTo(1), "StatusID after technician is set null");

            task.Technician = tech;

            Assert.AreEqual(5, task.TechnicianID, "TechnicianID after having set Technician");
            Assert.AreEqual(2, task.StatusID, "StatusID after having set Technician");
        }

        [Test]
        public void IfStatusSetTechnicianChanged()
        {
            var task = new Task()
            {
                ID = 1,
                StatusID = 2,
                Technician = new User() { ID = 2 }
            };
            Assert.That(task.TechnicianID, Is.EqualTo(2), "Initial value of Technician ID");
            Assert.That(task.StatusID, Is.EqualTo(2), "Initial Value of Status ID");

            task.StatusID = 1;

            Assert.AreEqual(1,task.StatusID,"status id after setting status");
            Assert.IsNull(task.Technician,"technician after setting status");

            task.StatusID = 2;
            Assert.AreEqual(1, task.StatusID, "status id after resetting status");
            Assert.IsNull(task.Technician, "technician after resetting status");
        }
    }
}
