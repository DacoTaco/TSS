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
    /// class that contains information of a Task's Type
    /// </summary>
    public class TaskType : BaseClass
    {
        private string typeName;

        public string Type
        {
            get { return typeName; }
            set { typeName = value; }
        }

        public TaskType(int TypeID,string type)
        {
            ID = TypeID;
            Type = type;
        }

        public override string ToString()
        {
            return Type;
        }
    }
}
