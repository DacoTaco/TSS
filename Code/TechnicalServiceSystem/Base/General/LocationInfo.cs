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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalServiceSystem.Base
{
    public class LocationInfo : BaseClass
    {
        private string locationName;

        public string Name
        {
            get { return locationName; }
            set { locationName = value; }
        }

        private int departmentID;

        public int DepartmentID
        {
            get { return departmentID; }
            set { departmentID = value; }
        }

        public LocationInfo(int LocationID,string name,int departmentId)
        {
            ID = LocationID;
            Name = name;
            DepartmentID = departmentId;
        }

        public override string ToString()
        {
            return locationName;
        }
    }
}
