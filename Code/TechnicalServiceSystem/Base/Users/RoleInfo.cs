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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalServiceSystem.Base
{
    /// <summary>
    /// Class containing all Role information
    /// </summary>
    public class RoleInfo : BaseClass
    {

        /// <summary>
        /// The User Permissions. these have int values that allow easy identification using a bitwise and
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


        //---------------------
        //      Properties
        //---------------------

        private string roleName;
        public string Name
        {
            get { return roleName; }
            set { roleName = value; }
        }


        //---------------------
        //      Functions
        //---------------------

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public RoleInfo(int id,string name)
        {
            ID = id;
            Name = name;
        }

        /// <summary>
        /// Return the rolename when we want to convert this class to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            //return base.Equals(obj);
            bool ret = false;

            RoleInfo otherObj = obj as RoleInfo;

            if (otherObj == null)
                return false;

            if (otherObj.ID == this.ID)
                ret = true;

            return ret;
        }

        public override int GetHashCode()
        {
            //return base.GetHashCode();
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)17;
                hash = (hash * 23) ^ this.ID.GetHashCode();
                hash = (hash * 23) ^ this.Name.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Function to generate a Datatable containing all the roles of list
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        static public DataTable GenerateRolesTable(List<RoleInfo> roles)
        {
            DataTable table = new DataTable();
            table.Columns.Add("roleID", typeof(string));
            table.Columns.Add("roleName", typeof(string));

            foreach (RoleInfo role in roles)
            {
                table.Rows.Add(new object[] { role.ID, role.Name });
            }

            return table;
        }
    }

}
