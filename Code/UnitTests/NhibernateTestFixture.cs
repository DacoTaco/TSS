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

using NHibernate;
using NUnit.Framework;
using TechnicalServiceSystem.Utilities;

namespace UnitTests
{
    [TestFixture]
    public class NhibernateTestFixture : DatabaseManager
    {
        [TearDown]
        protected void TearDownSession()
        {
            Session.Transaction.Rollback();
        }

        [SetUp]
        protected void SetUpSession()
        {
            Session = GetSession();
            Session.BeginTransaction();
            Assert.NotNull(Session,"Unable to retrieve the TestSession");
            Session.FlushMode = FlushMode.Never;
        }
    }
}
