/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
Copyright(C) 2017 - Joris 'DacoTaco' Vermeylen

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

using System.Collections.Generic;
using System.Linq;
using NHibernate.Util;
using TechnicalServiceSystem.Entities.Users;
using TechnicalServiceSystem.Lists;

namespace TechnicalServiceSystem
{
    /// <summary>
    ///     Role Manager of TSS. This manages all role data of TSS and handles the roles of users
    /// </summary>
    public static class RoleManager
    {
        /// <summary>
        ///     Checks whether the user has the given role or not
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool UserHasRole(User user, Role role)
        {
            if (user == null || user.Roles == null || user.Roles.Count <= 0)
                return false;

            if (user.Roles.Where(x => x == role).FirstOrNull() != null)
                return true;

            return false;
        }

        /// <summary>
        ///     Checks whether the user has the given permissions within the system or not
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <param name="roleList"></param>
        /// <returns></returns>
        public static bool UserHasPermission(User user, RolesPermissions permission)
        {
            var roles = GetUserPermissions(user);

            return ((roles & (int)permission) > 0);
        }

        /// <summary>
        ///     Retrieves an Int that has a bitwise int of all permissions
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleList"></param>
        /// <returns></returns>
        public static int GetUserPermissions(User user)
        {
            var roles = 0;
            IList<Role> roleList = SystemLists.User.Roles;
            if (roleList == null)
                return 0;

            if (user == null)
                roles = (int) RolesPermissions.Tasks;
            else
                foreach (var role in roleList)
                    if (UserHasRole(user, role))
                        switch (role)
                        {
                            case Role.Admin:
                                roles = int.MaxValue;
                                break;

                            case Role.UserManager:
                                roles |= (int) RolesPermissions.ManageUsers;
                                break;
                            case Role.SuppliersManager:
                                roles |= (int) RolesPermissions.ManageSuppliers;
                                roles |= (int) RolesPermissions.ViewSuppliers;
                                roles |= (int) RolesPermissions.ManageMachines;
                                break;
                            case Role.TaskManager:
                                roles |= (int) RolesPermissions.ManageTasks;
                                roles |= (int) RolesPermissions.Technician;
                                roles |= (int) RolesPermissions.ViewSuppliers;
                                roles |= (int) RolesPermissions.ManageMachines;
                                break;
                            case Role.Technician:
                                roles |= (int) RolesPermissions.Technician;
                                roles |= (int) RolesPermissions.ViewSuppliers;
                                roles |= (int) RolesPermissions.ManageMachines;
                                break;
                            case Role.User:
                            default:
                                roles |= (int) RolesPermissions.Tasks;
                                break;
                        }
            return roles;
        }
    }
}