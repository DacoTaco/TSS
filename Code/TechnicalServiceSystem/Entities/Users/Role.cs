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

namespace TechnicalServiceSystem.Entities.Users
{
    public class Role : BaseEntity
    {
        public Role() { }
        public Role(int id) : base()
        {
            ID = id;
        }

        public virtual string RoleName { get; set; }

        public override string ToString()
        {
            return RoleName;
        }     
    }

    /// <summary>
    ///     The User Permissions. these have int values that allow easy identification using a bitwise and
    /// </summary>
    public enum RolesPermissions
    {
        //current known functions : 
        // Tasks : 1
        // Technician : 2
        // Manage Tasks : 4
        // Machines : 8
        // Suppliers : 16
        // View Suppliers : 32
        // Users : 64
        Tasks = 1,
        Technician = 2,
        ManageTasks = 4,
        ManageMachines = 8,
        ManageSuppliers = 16,
        ViewSuppliers = 32,
        ManageUsers = 64
    }
    public enum Roles
    {
        Admin = 1,
        User = 2,
        Technician = 3,
        UserManager = 4,
        TaskManager = 5,
        SupplierManager = 6
    }
}