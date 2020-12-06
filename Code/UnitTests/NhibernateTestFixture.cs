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
using System.Transactions;
using TechnicalServiceSystem.Utilities;

namespace UnitTests
{
    [TestFixture]
    public class NhibernateTestFixture : DatabaseManager
    {
        //run this code block in a transaction that will get rollbacked later
        private TransactionScope _transactionScope;

        [TearDown]
        protected void TearDownSession()
        {
            _transactionScope.Dispose();
            Session.Clear();
        }

        [SetUp]
        protected void SetUpSession()
        {
            _transactionScope = new TransactionScope();
        }
    }
}
