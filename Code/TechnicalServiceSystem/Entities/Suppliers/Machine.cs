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

using System.Collections.Generic;
using TechnicalServiceSystem.Entities.General;

namespace TechnicalServiceSystem.Entities.Suppliers
{
    public class Machine : BaseEntity
    {
        public Machine()
        {
            Documentations = new List<Documentation>();
            Photos = new List<Photo>();
        }
        public Machine(int id) : base()
        {
            ID = id;
        }

        public virtual string Description { get; set; }
        public virtual string SerialNumber { get; set; }
        public virtual string ModelNumber { get; set; }
        public virtual string ModelName { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual MachineType Type { get; set; }
        public virtual ICollection<Documentation> Documentations { get; protected set; }
        public virtual ICollection<Photo> Photos { get; protected set; }

        public override string ToString()
        {
            return Description;
        }
    }
}