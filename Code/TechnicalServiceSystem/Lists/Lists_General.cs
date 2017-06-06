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
using TechnicalServiceSystem.Base;
using TechnicalServiceSystem;

namespace TechnicalServiceSystem
{
    public partial class SystemLists
    {

        //----------------
        // LISTS
        //----------------

        //List of all departments in the database
        private ObservableCollection<DepartmentInfo> department;
        public ObservableCollection<DepartmentInfo> Departments
        {
            get { return department; }
            set
            {
                if (value != null && value.Count == 0)
                    value.Add(new DepartmentInfo(0, "Other"));
                department = value;
                OnPropertyChanged("Departments");
            }
        }

        //List of all locations in the database
        private ObservableCollection<LocationInfo> locations;
        public ObservableCollection<LocationInfo> Locations
        {
            get { return locations; }
            set
            {
                if (value != null && value.Count == 0)
                    value.Add(new LocationInfo(0, "Other", 0));
                locations = value;
                OnPropertyChanged("Locations");
            }
        }

        /// <summary>
        /// Retrieve list of all departments from database
        /// </summary>
        public void GetDepartments()
        {
            try
            {
                string company = Settings.GetAppSetting("company");
                var generalMgr = new GeneralManager();
                Departments = generalMgr.GetDepartments(company);
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_Departments : " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieve list of all locations from database
        /// </summary>
        public void GetLocations()
        {
            try
            {
                var generalMgr = new GeneralManager();
                Locations = generalMgr.GetLocations();
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_Locations : " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieve all list of Departments,Locations etc etc
        /// </summary>
        public void GetGeneralLists()
        {
            GetDepartments();
            GetLocations();
        }
    }
}
