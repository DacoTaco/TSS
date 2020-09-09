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
using System.Collections.Generic;
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Suppliers;

namespace UnitTests.Mapping.Suppliers
{
    [TestFixture]
    public class MachineMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapMachine()
        {
            //Arrange
            var supplier = Session.Get<Supplier>(1);
            var machineType = Session.Get<MachineType>(1);
            Assert.NotNull(supplier);
            Assert.NotNull(machineType);

            var machine = new Machine()
            {
                Description = "descript",
                ModelName = "modelName",
                ModelNumber = "9001",
                SerialNumber = "1987-15681",
                Supplier = supplier,
                Type = machineType
            };

            machine.AddPhoto(new Photo() { FileName = "test9001.jpg" });
            machine.AddDocumentation(new Documentation() { DocumentationPath = @".\bloop" });

            //Act & Assert
            new PersistenceSpecification<Machine>(Session)
                .VerifyTheMappings(machine);
        }
    }
}
