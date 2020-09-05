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
using System.Linq;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.Suppliers;

namespace UnitTests.Managers
{
    [TestFixture]
    class SupplierManagerTestFixture : NhibernateTestFixture
    {
        private readonly SupplierManager supplierManager = new SupplierManager();

        [Test]
        public void CanRetrieveSingleMachine()
        {
            Machine _machine = supplierManager.GetMachine(1);

            Assert.NotNull(_machine);
            Assert.AreEqual(1, _machine.ID);
            Assert.That(_machine.Description, Is.EqualTo("Houten Bed"), "Device Name");
            Assert.That(_machine.ModelNumber,Is.EqualTo("WB-010101"),"Device Model Number");
            Assert.That(_machine.ModelName,Is.EqualTo("Estetica"),"Device Model Name");
            Assert.IsTrue(_machine.Photos.Count > 0);
        }

        [Test]
        public void GetMachineTypesRetrievesAllMachineTypes()
        {
            //Arrange & Act
            var result = supplierManager.GetMachineTypes();

            //Assert
            Assert.NotNull(result);
            Assert.That(result, Has.Count.EqualTo(11));
            Assert.AreEqual(1, result.First().ID);
            Assert.AreEqual("Hoog Laag Bed", result.First().TypeName);
            Assert.AreEqual(11, result.Last().ID);
            Assert.AreEqual("Andere", result.Last().TypeName);
        }
    }
}
