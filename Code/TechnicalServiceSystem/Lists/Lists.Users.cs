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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using TechnicalServiceSystem.Entities.Users;

namespace TechnicalServiceSystem.Lists
{
    public class UserList
    {

        private readonly string RoleList = "RoleList";
        private readonly string TechnicianList = "TechnicianList";
        private ObservableCollection<Role> _roles;
        public ObservableCollection<Role> Roles
        {
            get
            {
                ObservableCollection<Role> ret = null;
                //in ASP we can't use the static stuff since static is the same for all requests/sessions. so savinig to sessions it is xD
                if (Settings.IsWebEnvironment)
                    ret = Settings.GetSessionSetting<ObservableCollection<Role>>(RoleList);
                else
                    ret = _roles;

                if (ret == null)
                {     
                    ret = new ObservableCollection<Role>(Enum.GetValues(typeof(Role)).OfType<Role>().ToList());

                    if (Settings.IsWebEnvironment)
                        Settings.SetSessionSetting(RoleList, ret);
                    else
                        _roles = ret;
                }

                return ret;
            }
        }

        private ObservableCollection<User> _technicians;

        public ObservableCollection<User> Technicians
        {
            get
            {
                ObservableCollection<User> ret = null;
                if (Settings.IsWebEnvironment)
                    ret = Settings.GetSessionSetting<ObservableCollection<User>>(TechnicianList);
                else
                    ret = _technicians;

                if (ret == null)
                {
                    var UserMngr = new UserManager();
                    ret = UserMngr.GetUsersByRole(Entities.Users.Role.Technician, Settings.GetCompanyName());

                    if (Settings.IsWebEnvironment)
                        Settings.SetSessionSetting(TechnicianList, ret);
                    else
                        _technicians = ret;
                }

                return ret;
            }
        }

        public ObservableCollection<User> TranslatedTechnicians(string TranslationNotSet)
        {
            if (Technicians.First()?.UserName == TranslationNotSet)
                return Technicians;

            var newList = new ObservableCollection<User>();
            newList.Add(new User(0) {UserName = TranslationNotSet});
            foreach (var technician in Technicians)
            {
                newList.Add(technician);
            }

            if (Settings.IsWebEnvironment)
                Settings.SetSessionSetting(TechnicianList, newList);
            else
                _technicians = newList;

            return newList;
        }
    }
}
