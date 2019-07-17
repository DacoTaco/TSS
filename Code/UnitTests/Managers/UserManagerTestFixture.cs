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
using TechnicalServiceSystem.Entities.Users;

namespace UnitTests.Managers
{
    [TestFixture]
    class UserManagerTestFixture : NhibernateTestFixture
    {
        private UserManager userManager = new UserManager();
        [Test]
        public void CanRetrieveUserListsOfCertainRole()
        {
            var _userList = userManager.GetUsersByRole("Technician", Settings.GetCompanyName());

            Assert.NotNull(_userList);
            Assert.That(_userList.Count,Is.GreaterThan(2));
        }

        private static object[] UserCases = 
        {
            new object[] { 12,null },
            new object[] { 11,true },
            new object[] { 1, false }
        };
        [Test, TestCaseSource("UserCases")]
        public void CanRetrieveUsersList(int userListCount,bool? activeOnly)
        {
            var userList = userManager.GetUsers(null,null,0,activeOnly);

            Assert.NotNull(userList);
            Assert.AreEqual(userListCount,userList.Count);
        }

        [Test]
        public void CanLoginUser()
        {
            //Arrange
            var user = userManager.GetUsers("Sint-Elisabeth", "test", 0, true).SingleOrDefault();

            //Act&Arrange
            Assert.NotNull(user);
            Assert.IsTrue(userManager.LoginUser(ref user, "test"));
            Assert.AreEqual(user.UserHash,"69A7455D2BA647835B8BBA43CB0C1CBAF36914D39E45055F1188E6D63D14A54B5508A9DB9C93B3BFB5343CDA822E3131B5963B78A7D2E95AB38D12F6E51572EF");
            Assert.IsTrue(string.IsNullOrWhiteSpace(user.Password));
        }
    }
}
