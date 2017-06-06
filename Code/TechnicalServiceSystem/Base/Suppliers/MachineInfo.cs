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

namespace TechnicalServiceSystem.Base
{
    /// <summary>
    /// Base Class containing all information about a machine
    /// </summary>
    public class MachineInfo : BaseClass
    {

        //--------------------
        //      Properties
        //--------------------
        private string machineName;
        public string Name
        {
            get { return machineName; }
            set { machineName = value; }
        }

        private string machSerialNummer;
        public string SerialNumber
        {
            get { return machSerialNummer; }
            set { machSerialNummer = value; }
        }

        private string modelNumber;
        public string ModelNumber
        {
            get { return modelNumber; }
            set { modelNumber = value; }
        }

        private string modelName;
        public string ModelName
        {
            get { return modelName; }
            set { modelName = value; }
        }


        private int supplierID;
        public int SupplierID
        {
            get { return supplierID; }
            set { supplierID = value; }
        }

        private int typeID;
        public int TypeID
        {
            get { return typeID; }
            set { typeID = value; }
        }

        private ObservableCollection<PhotoInfo> photos;
        public ObservableCollection<PhotoInfo> Photos
        {
            get
            {
                if (photos == null)
                    photos = new ObservableCollection<PhotoInfo>();

                return photos;
            }
            set
            {
                photos = value;
            }
        }

        private ObservableCollection<DocumentationInfo> documentations;
        public ObservableCollection<DocumentationInfo> Documentations
        {
            get
            {
                if (documentations == null)
                    documentations = new ObservableCollection<DocumentationInfo>();

                return documentations;
            }
            set { documentations = value; }
        }


        //extension of the base clone so the Observablecollections done share the same object/reference/pointer
        public new MachineInfo Clone()
        {
            MachineInfo output = base._clone<MachineInfo>();

            output.Photos = new ObservableCollection<PhotoInfo>();
            foreach (var item in Photos)
            {
                output.Photos.Add(item);
            }

            output.Documentations = new ObservableCollection<DocumentationInfo>();
            foreach (var item in Documentations)
            {
                output.Documentations.Add(item);
            }

            return output;
        }

        protected MachineInfo() { }
        /// <summary>
        /// Default constructor of MachineInfo
        /// </summary>
        /// <param name="MachineID"></param>
        /// <param name="name"></param>
        /// <param name="serialNumber"></param>
        /// <param name="model_number"></param>
        /// <param name="model_name"></param>
        /// <param name="supplierId"></param>
        /// <param name="typeId"></param>
        public MachineInfo(int MachineID,string name,string serialNumber,string model_number,string model_name,int supplierId,int typeId)
        {
            ID = MachineID;
            Name = name;
            SerialNumber = serialNumber;
            ModelNumber = model_number;
            ModelName = model_name;
            SupplierID = supplierId;
            TypeID = typeId;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
