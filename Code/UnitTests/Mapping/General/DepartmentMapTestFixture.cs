using FluentNHibernate.Testing;
using NUnit.Framework;
using System.Collections.Generic;
using TechnicalServiceSystem.Entities.General;

namespace UnitTests.Mapping.General
{
    [TestFixture]
    public class DepartmentMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapDepartment()
        {
            //Arrange
            var department = new Department()
            {
                Description = "PoLiCe",
                ParentDepartment = new Department() { Description = "Random HeadDepartment"},
                Company = new List<Company>() { new Company() { Name = "LoL" } }
            };
            //Act&Assert
            new PersistenceSpecification<Department>(Session)
                .VerifyTheMappings(department);
        }
    }
}
