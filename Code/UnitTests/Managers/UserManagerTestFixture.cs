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
            new object[] { 13, null },
            new object[] { 12, true },
            new object[] { 1, false }
        };
        [Test, TestCaseSource(nameof(UserCases))]
        public void CanRetrieveUsersList(int userListCount,bool? activeOnly)
        {
            var userList = userManager.GetUsers(null,null,0,activeOnly);

            Assert.NotNull(userList);
            Assert.AreEqual(userListCount,userList.Count);
        }

        [Test]
        public void CanVerifyUserHash()
        {
            //Arrange
            var user = userManager.GetUsers("Sint-Elisabeth", "test", 0, true).SingleOrDefault();
            user.UserHash = "B0668584D7E16CC479B6F225AD166E2F89C44E1449182392B3333F6A1B98918037E1FFADE1DCE623E96589FEBA9C9E2481131FD09AF1F1698EE10E067C225017";

            //Act
            var result = userManager.CheckUserHash(user);

            //Assert
            Assert.True(result);
        }

        [Test]
        public void CanLoginUser()
        {
            //Arrange
            var user = userManager.GetUsers("Sint-Elisabeth", "test", 0, true).SingleOrDefault();
            user.Password = "testerino";

            //Act&Arrange
            Assert.NotNull(user);
            Assert.IsTrue(userManager.LoginUser(ref user, "test"));
            Assert.AreEqual("B0668584D7E16CC479B6F225AD166E2F89C44E1449182392B3333F6A1B98918037E1FFADE1DCE623E96589FEBA9C9E2481131FD09AF1F1698EE10E067C225017", user.UserHash);
            Assert.IsTrue(string.IsNullOrWhiteSpace(user.Password));
        }
    }
}
