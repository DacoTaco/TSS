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

            return user.Roles.Any(x => x == role);
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

            return roles.Any(x => x == permission);
        }

        /// <summary>
        ///     Retrieves an Int that has a bitwise int of all permissions
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleList"></param>
        /// <returns></returns>
        public static IList<RolesPermissions> GetUserPermissions(User user)
        {
            var roles = new List<RolesPermissions>();
            IList<Role> roleList = SystemLists.User.Roles;
            if (roleList == null)
                return roles;

            if (user == null)
                roles.Add(RolesPermissions.Tasks);
            else
                foreach (var role in roleList)
                {
                    if (UserHasRole(user, role))
                        switch (role)
                        {
                            case Role.Admin:
                                var enumValues = typeof(RolesPermissions).GetEnumValues().Cast<RolesPermissions>();
                                roles.AddRange(enumValues);
                                break;
                            case Role.UserManager:
                                roles.Add(RolesPermissions.ManageUsers);
                                break;
                            case Role.SuppliersManager:
                                roles.Add(RolesPermissions.ManageSuppliers);
                                roles.Add(RolesPermissions.ViewSuppliers);
                                roles.Add(RolesPermissions.ManageMachines);
                                break;
                            case Role.TaskManager:
                                roles.Add(RolesPermissions.ManageTasks);
                                roles.Add(RolesPermissions.Technician);
                                roles.Add(RolesPermissions.ViewSuppliers);
                                roles.Add(RolesPermissions.ManageMachines);
                                break;
                            case Role.Technician:
                                roles.Add(RolesPermissions.Technician);
                                roles.Add(RolesPermissions.ViewSuppliers);
                                roles.Add(RolesPermissions.ManageMachines);
                                break;
                            case Role.User:
                            default:
                                roles.Add(RolesPermissions.Tasks);
                                break;
                        }
                }                   
            return roles;
        }
    }
}