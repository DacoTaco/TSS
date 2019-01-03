﻿/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
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

using System.Collections.Generic;
using System.Data;
using TechnicalServiceSystem.Entities.General;

namespace TechnicalServiceSystem.Entities.Users
{
    public class User : BaseEntity
    {
        public User()
        {
            Roles = new List<Role>();
        }

        public virtual string UserName { get; set; }
        public virtual string UserHash { get; set; }
        public virtual string Password { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual Photo Photo { get; set; }
        public virtual Department Department { get; set; }

        public override string ToString()
        {
            return UserName;
        }

        /// <summary>
        ///     Generate a DataTable used by TSS as Userinformation. used for tasks in the database
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static DataTable GenerateUserTable(int UserID, string UserName)
        {
            var table = new DataTable();
            table.Columns.Add("userID", typeof(int));
            table.Columns.Add("userName", typeof(string));

            table.Rows.Add(UserID, UserName);

            return table;
        }
    }
}