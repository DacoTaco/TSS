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
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.Users;

namespace UnitTests.Managers
{
    [TestFixture]
    class RoleManagerTestFixture
    {
        private UserManager userManager = new UserManager();
        [Test]
        public void CanCalculateUserPermissions()
        {
            var _user = userManager.GetUserByID(1);
            var _admin = userManager.GetUserByID(4);

            Assert.NotNull(_user,"test user task only");
            Assert.NotNull(_admin,"test user admin");
            Assert.True(RoleManager.UserHasPermission(_user, RolesPermissions.Tasks), "User has permissions to view tasks");
            Assert.False(RoleManager.UserHasPermission(_user,RolesPermissions.ManageTasks),"User doesn't have tasks manage permissions");
            Assert.False(RoleManager.UserHasPermission(_user,RolesPermissions.Technician),"User is not technician");
            Assert.False(RoleManager.UserHasPermission(_user,RolesPermissions.ManageMachines),"User does not have permissions to manage machines");
            Assert.False(RoleManager.UserHasPermission(_user,RolesPermissions.ManageSuppliers),"User does not have permissions to manage supplier info");
            Assert.False(RoleManager.UserHasPermission(_user,RolesPermissions.ManageUsers),"User does not have permissions to manage users");
            Assert.False(RoleManager.UserHasPermission(_user,RolesPermissions.ViewSuppliers),"User does not have permissions to view suppliers");

            Assert.True(RoleManager.UserHasPermission(_admin, RolesPermissions.Tasks), "User has permissions to view tasks");
            Assert.True(RoleManager.UserHasPermission(_admin, RolesPermissions.ManageTasks), "User has tasks manage permissions");
            Assert.True(RoleManager.UserHasPermission(_admin, RolesPermissions.Technician), "User is technician");
            Assert.True(RoleManager.UserHasPermission(_admin, RolesPermissions.ManageMachines), "User has permissions to manage machines");
            Assert.True(RoleManager.UserHasPermission(_admin, RolesPermissions.ManageSuppliers), "User has permissions to manage supplier info");
            Assert.True(RoleManager.UserHasPermission(_admin, RolesPermissions.ManageUsers), "User has permissions to manage users");
            Assert.True(RoleManager.UserHasPermission(_admin, RolesPermissions.ViewSuppliers), "User has permissions to view suppliers");
        }

    }
}
