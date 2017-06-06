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
    /// Base class containing all documentation info
    /// </summary>
    public class DocumentationInfo : BaseClass
    {
        //the plan is to make it work like the photos. save it to the server and just link to the data.
        //hence, the path, and the ID
        private string docuPath;
        public string Path
        {
            get { return docuPath; }
            set { docuPath = value; }
        }

        public DocumentationInfo(int DocuID,string path)
        {
            ID = DocuID;
            Path = path;
        }

    }
}
