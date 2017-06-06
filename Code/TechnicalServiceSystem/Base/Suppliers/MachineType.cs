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
    /// <summary>
    /// Base class containing all information on the types of machines saved in the database
    /// </summary>
    public class MachineType : BaseClass
    {
        private string typeName;

        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }


        public MachineType(int TypeID,string type)
        {
            ID = TypeID;
            TypeName = type;
        }

        public override string ToString()
        {
            return TypeName;
        }

    }
}
