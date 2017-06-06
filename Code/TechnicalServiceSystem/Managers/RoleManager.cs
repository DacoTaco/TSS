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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TechnicalServiceSystem.Base;
using static TechnicalServiceSystem.Base.RoleInfo;

namespace TechnicalServiceSystem
{
    /// <summary>
    /// Role Manager of TSS. This manages all role data of TSS and handles the roles of users
    /// </summary>
    public static class RoleManager
    {
        /// <summary>
        /// Checks whether the user has the given role or not
        /// </summary>
        /// <param name="user"></param>
        /// <param name="RoleName"></param>
        /// <returns></returns>
        static public bool UserHasRole(UserInfo user, string RoleName)
        {
            bool ret = false;

            if ((user == null) || (user.UserRoles == null) || (user.UserRoles.Count <= 0))
                return false;

            foreach (RoleInfo role in user.UserRoles)
            {
                if (role.Name == RoleName)
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Checks whether the user has the given role or not
        /// </summary>
        /// <param name="user"></param>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        static public bool UserHasRole(UserInfo user, int RoleID)
        {
            if ((user == null) || (user.UserRoles == null) || (user.UserRoles.Count <= 0))
                return false;

            foreach (RoleInfo role in user.UserRoles)
            {
                if (role.ID == RoleID)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Checks whether the user's RolesList has the given role or not
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="RoleName"></param>
        /// <returns></returns>
        static public bool UserHasRole(ObservableCollection<RoleInfo> roles,string RoleName)
        {
            if (roles == null)
                return false;

            foreach (RoleInfo role in roles)
            {
                if (role.Name == RoleName || role.Name == "Admin")
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the user has the given permissions within the system or not
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <param name="roleList"></param>
        /// <returns></returns>
        static public bool UserHasPermission(UserInfo user,RolesPermissions permission,List<RoleInfo> roleList = null)
        {
            if (user == null)
                return false;

            int roles = GetUserPermissions(user, roleList);
            int permissionRole = (int)permission;

            if ((roles & permissionRole) == permissionRole)
                return true;

            return false;
        }

        /// <summary>
        /// Retrieves an Int that has a bitwise int of all permissions
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleList"></param>
        /// <returns></returns>
        static public int GetUserPermissions(UserInfo user,List<RoleInfo> roleList = null)
        {
            int roles = 0;

            if(roleList == null)
            {
                var usrMngr = new UserManager();
                roleList = (usrMngr.GetRoles()).ToList<RoleInfo>();
                if (roleList == null)
                    return 0;
            }

            if (user == null)
                roles = (int)RoleInfo.RolesPermissions.Tasks;
            else
            {
                foreach (RoleInfo role in roleList)
                {
                    if (RoleManager.UserHasRole(user, role.ID))
                    {
                        switch (role.ID)//(role.Name)
                        {
                            case 1://"Admin":
                                roles = int.MaxValue;
                                break;

                            case 4://"User Manager":
                                roles |= (int)RoleInfo.RolesPermissions.ManageUsers;
                                break;
                            case 6://"Suppliers Manager":
                                roles |= (int)RoleInfo.RolesPermissions.ManageSuppliers;
                                roles |= (int)RoleInfo.RolesPermissions.ViewSuppliers;
                                roles |= (int)RoleInfo.RolesPermissions.ManageMachines;
                                break;
                            case 5://"Task Manager":
                                roles |= (int)RoleInfo.RolesPermissions.ManageTasks;
                                roles |= (int)RoleInfo.RolesPermissions.Technician;
                                roles |= (int)RoleInfo.RolesPermissions.ViewSuppliers;
                                roles |= (int)RoleInfo.RolesPermissions.ManageMachines;
                                break;
                            case 3://"Technician":
                                roles |= (int)RoleInfo.RolesPermissions.Technician;
                                roles |= (int)RoleInfo.RolesPermissions.ViewSuppliers;
                                roles |= (int)RoleInfo.RolesPermissions.ManageMachines;
                                break;
                            case 2://"User":
                            default:
                                roles |= (int)RoleInfo.RolesPermissions.Tasks;
                                break;
                        }
                    }
                }
            }
            return roles;
        }
    }
}
