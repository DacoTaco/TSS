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
    /// Base class containing a supplier's contact information.
    /// </summary>
    public class ContactInfo : BaseClass
    {
        private string conName;
        public string Name
        {
            get { return conName; }
            set { conName = value; }
        }

        private string conStatus;
        public string Status
        {
            get { return conStatus; }
            set { conStatus = value; }
        }

        private string conNr;
        public string PhoneNr
        {
            get { return conNr; }
            set { conNr = value; }
        }

        private string conEmail;
        public string Email
        {
            get { return conEmail; }
            set { conEmail = value; }
        }

        private string conFax;
        public string Fax
        {
            get { return conFax; }
            set { conFax = value; }
        }

        private int conAddressID;
        public int AddressID
        {
            get { return conAddressID; }
            set { conAddressID = value; }
        }

        private int conSupplierID;
        public int SupplierID
        {
            get { return conSupplierID; }
            set { conSupplierID = value; }
        }

        public ContactInfo(int ContactID,string name,string status,string phoneNr,string faxNr,string email,int address,int supplier)
        {
            ID = ContactID;
            Name = name;
            Status = status;
            PhoneNr = phoneNr;
            Fax = faxNr;
            Email = email;
            AddressID = address;
            SupplierID = supplier;
        }
    }
}
