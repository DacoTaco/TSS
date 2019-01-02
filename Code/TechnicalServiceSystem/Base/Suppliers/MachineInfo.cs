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

using System.Collections.ObjectModel;
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Suppliers;

namespace TechnicalServiceSystem.Base
{
    /// <summary>
    ///     Base Class containing all information about a machine
    /// </summary>
    public class MachineInfo : BaseClass
    {
        private ObservableCollection<Documentation> documentations;

        //--------------------
        //      Properties
        //--------------------

        private ObservableCollection<Photo> photos;


        protected MachineInfo()
        {
        }

        /// <summary>
        ///     Default constructor of MachineInfo
        /// </summary>
        /// <param name="MachineID"></param>
        /// <param name="name"></param>
        /// <param name="serialNumber"></param>
        /// <param name="model_number"></param>
        /// <param name="model_name"></param>
        /// <param name="supplierId"></param>
        /// <param name="typeId"></param>
        public MachineInfo(int MachineID, string name, string serialNumber, string model_number, string model_name,
            int supplierId, int typeId)
        {
            ID = MachineID;
            Name = name;
            SerialNumber = serialNumber;
            ModelNumber = model_number;
            ModelName = model_name;
            SupplierID = supplierId;
            TypeID = typeId;
        }

        public string Name { get; set; }

        public string SerialNumber { get; set; }

        public string ModelNumber { get; set; }

        public string ModelName { get; set; }

        public int SupplierID { get; set; }

        public int TypeID { get; set; }

        public ObservableCollection<Photo> Photos
        {
            get
            {
                if (photos == null)
                    photos = new ObservableCollection<Photo>();

                return photos;
            }
            set { photos = value; }
        }

        public ObservableCollection<Documentation> Documentations
        {
            get
            {
                if (documentations == null)
                    documentations = new ObservableCollection<Documentation>();

                return documentations;
            }
            set { documentations = value; }
        }


        //extension of the base clone so the Observablecollections done share the same object/reference/pointer
        public new MachineInfo Clone()
        {
            var output = base._clone<MachineInfo>();

            output.Photos = new ObservableCollection<Photo>();
            foreach (var item in Photos) output.Photos.Add(item);

            output.Documentations = new ObservableCollection<Documentation>();
            foreach (var item in Documentations) output.Documentations.Add(item);

            return output;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}