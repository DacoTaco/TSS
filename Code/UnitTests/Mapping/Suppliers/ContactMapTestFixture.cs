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
using TechnicalServiceSystem.Entities.Suppliers;

namespace UnitTests.Mapping
{
    [TestFixture]
    public class ContactMapTestFixture : NhibernateTestFixture
    {
        [Test]
        public void CanMapContactInfo()
        {
            var _contacts = TestSession.QueryOver<Contact>()
                .Where(x => x.Supplier.ID == 1)
                .OrderBy(c => c.ID).Asc
                .List();

            Assert.NotNull(_contacts);
            Assert.AreEqual(2,_contacts.Count);

            var contact = _contacts.First();

            Assert.AreEqual(1,contact.ID);
            Assert.AreEqual(1,contact.Supplier.ID);
            Assert.AreEqual("Wissner-Bosserhoff",contact.Name);
            Assert.That(contact.Position, Is.EqualTo("General"));
            Assert.That(contact.PhoneNumber, Is.EqualTo("+32 (0) 15210841"));
            Assert.IsNotNull(contact.Address);

            contact = _contacts.Last();
            Assert.IsNull(contact.Address);
            Assert.AreEqual("sales@wi-bo.be",contact.Email);
            Assert.IsNull(contact.FaxNumber);

        }
    }
}

