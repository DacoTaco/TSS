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

using Equin.ApplicationFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;

namespace TechnicalServiceSystem.DataSourceManagers
{
    /// <summary>
    /// Data Source manager to retrieve data from the database for a Datasource object. wrapper of the data manager of users
    /// </summary>
    [DataObject(true)]
    public class UsersManager
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public BindingListView<UserInfo> GetUsers(string contains, int? RoleID = null, string SortBy = null)
        {
            try
            {
                int roleID = 0;

                if (RoleID.HasValue)
                    roleID = RoleID.Value;

                var userManager = new UserManager();
                string company = Settings.GetAppSetting("company");

                //update the systemlist tasks so we are insync with the rest of the system
                SystemLists Lists = SystemLists.GetInstance();
                Lists.Users = userManager.GetUsers(company, contains, roleID);

                List<UserInfo> ret;

                if (Lists.Users != null)
                    ret = Lists.Users.ToList();
                else
                    ret = new List<UserInfo>();

                BindingListView<UserInfo> list = new BindingListView<UserInfo>(ret);
                if (!String.IsNullOrWhiteSpace(SortBy))
                    list.Sort = SortBy;

                return list;
            }
            catch
            {
                return new BindingListView<UserInfo>(new List<UserInfo>());
            }

        }
    }
}
