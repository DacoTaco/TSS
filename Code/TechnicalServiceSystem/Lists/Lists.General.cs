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

using System.Collections.ObjectModel;
using TechnicalServiceSystem.Entities.General;

namespace TechnicalServiceSystem.Lists
{
    public class GeneralLists
    {
        private ObservableCollection<Department> _departments;

        public ObservableCollection<Department> Departments
        {
            get
            {
                ObservableCollection<Department> ret = null;
                //in ASP we can't use the static stuff since static is the same for all requests/sessions. so savinig to sessions it is xD
                if (Settings.IsWebEnvironment)
                    ret = Settings.GetSessionSetting<ObservableCollection<Department>>("DepartmentList");                  
                else
                    ret = _departments;

                if (ret == null)
                {
                    var Mngr = new GeneralManager();
                    ret = Mngr.GetDepartments(Settings.GetCompanyName());

                    if (Settings.IsWebEnvironment)
                        Settings.SetSessionSetting("DepartmentList", ret);
                    else
                        _departments = ret;
                }

                return ret;
            }
        }

        private ObservableCollection<Location> _locations;
        public ObservableCollection<Location> Locations
        {

            get
            {
                ObservableCollection<Location> ret = null;
                if (Settings.IsWebEnvironment)
                    ret = Settings.GetSessionSetting<ObservableCollection<Location>>("LocationList");
                else
                    ret = _locations;

                if (ret == null)
                {
                    var Mngr = new GeneralManager();
                    ret = Mngr.GetLocations();

                    if (Settings.IsWebEnvironment)
                        Settings.SetSessionSetting("LocationList", ret);
                    else
                        _locations = ret;
                }

                return ret;
            }
        }
    }
}
