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
using System.Threading.Tasks;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;

namespace TechnicalServiceSystem
{
    public partial class SystemLists
    {
        //----------------
        // LISTS
        //----------------

        //list of Technicians
        private ObservableCollection<UserInfo> technicians;
        /// <summary>
        /// List Containing all Registered Technicians
        /// </summary>
        public ObservableCollection<UserInfo> Technicians
        {
            get
            {
                if (technicians == null)
                    technicians = new ObservableCollection<UserInfo>();
                return technicians;
            }
            set
            {
                var temp = new ObservableCollection<UserInfo>();
                temp.Add(new UserInfo(0, "None", 0, 0, true));
                foreach (UserInfo item in value)
                {
                    temp.Add(item);
                }
                technicians = temp;
                OnPropertyChanged("Technicians");
            }
        }


        //list of active Users, like list of users that can login and access the system
        /// <summary>
        /// List containing all Active users capable of using TSS
        /// </summary>
        public ObservableCollection<UserInfo> ActiveUsers
        {
            get
            {
                ObservableCollection<UserInfo> ret = new ObservableCollection<UserInfo>();
                if (Users == null)
                    Users = new ObservableCollection<UserInfo>();
                else
                {
                    foreach (UserInfo user in Users)
                    {
                        if (user.Active)
                            ret.Add(user);
                    }
                }
                return ret;
            }
        }


        //list of all users
        private ObservableCollection<UserInfo> users;
        /// <summary>
        /// List containing all Users
        /// </summary>
        public ObservableCollection<UserInfo> Users
        {
            get { return users; }
            set { users = value; }
        }

        // list of roles
        private ObservableCollection<RoleInfo> roles;
        /// <summary>
        /// List containing all Roles that users can have
        /// </summary>
        public ObservableCollection<RoleInfo> Roles
        {
            get
            {
                if (roles == null)
                    roles = new ObservableCollection<RoleInfo>();

                return roles;
            }
            set
            {
                roles = value;
                OnPropertyChanged("Roles");
            }
        }




        //----------------------------
        // List retrieval functions
        //----------------------------

        /// <summary>
        /// Retrieve the Lists of Technicians
        /// </summary>
        /// <param name="NotSet"></param>
        private void GetTechnicians(string NotSet = "")
        {      
            try
            {
                string company = Settings.GetAppSetting("company");
                UserManager Usrmgr = new UserManager();

                Technicians = Usrmgr.GetUsersByRole("Technician",company);
                if (Technicians != null && Technicians.Count > 0 && !String.IsNullOrWhiteSpace(NotSet))
                    Technicians[0].Username = NotSet;
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_Technicians : " + ex.Message, ex);
            }
        }

        /// <summary>
        /// retrieve the list of Users
        /// </summary>
        private void GetUsers()
        {
            try
            {
                string company = Settings.GetAppSetting("company");
                UserManager Usrmgr = new UserManager();
                Users = Usrmgr.GetUsers(company);
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_Users : " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieve the list of roles
        /// </summary>
        /// <param name="UserRoles"></param>
        private void GetRoles(string[] UserRoles)
        {
            try
            {
                UserManager Usrmgr = new UserManager();

                ObservableCollection<RoleInfo> roles = Usrmgr.GetRoles();

                //only apply names if we have enough, ignoring 0 on the userRoles("all")
                if(UserRoles != null && UserRoles.Length >= roles.Count+1)
                {
                    for (int i = 0; i < roles.Count; i++)
                    {
                        roles[i].Name = UserRoles[i+1];
                    }
                }
                Roles = roles;
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_Users : " + ex.Message, ex);
            }
        }

        /// <summary>
        /// retrieve all Lists of the Users part of the system
        /// </summary>
        /// <param name="NotSet"></param>
        /// <param name="UserRoles"></param>
        public void GetUserLists(string NotSet = "",string[] UserRoles = null)
        {
            GetUsers();
            GetRoles(UserRoles);
            GetTechnicians(NotSet);
        }

    }
}
