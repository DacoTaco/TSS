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
    public class Address : BaseClass
    {
        private string addressLine1;

        public string AddressLine1
        {
            get { return addressLine1; }
            set { addressLine1 = value; }
        }

        private string addressLine2;

        public string AddressLine2
        {
            get { return addressLine2; }
            set { addressLine2 = value; }
        }

        public string AddressLine
        {
            get
            {
                return String.Format("{0}{1}{2}", AddressLine, Environment.NewLine, AddressLine2);
            }
        }

        private int addressNr;

        public int AddressNr
        {
            get { return addressNr; }
            set { addressNr = value; }
        }

        private int addressBus;

        public int AddressBus
        {
            get { return addressBus; }
            set { addressBus = value; }
        }

        private String postCode;

        public String Postcode
        {
            get { return postCode; }
            set { postCode = value; }
        }

        private string city;

        public string City
        {
            get { return city; }
            set { city = value; }
        }


        private string region;

        public string Region
        {
            get { return region; }
            set { region = value; }
        }


        private string country;

        public string Country
        {
            get { return country; }
            set { country = value; }
        }


        public Address(int AddressID, string line1, string line2, int nr, int bus, string postcode, string city, string region, string country)
        {
            ID = AddressID;
            AddressLine1 = line1;
            AddressLine2 = line2;
            AddressNr = nr;
            AddressBus = bus;
            Postcode = postcode;
            City = city;
            Region = region;
            Country = country;
        }



    }
}
