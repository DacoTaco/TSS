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

using System;
using System.Collections.Generic;
using System.Linq;
using TechnicalServiceSystem.Entities.General;

namespace TechnicalServiceSystem.Entities.Suppliers
{
    public class Machine : BaseEntity
    {
        public Machine()
        {
            _documentations = new List<Documentation>();
            _photos = new List<Photo>();
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

        protected IList<Documentation> _documentations;
        public virtual IEnumerable<Documentation> Documentations => _documentations.AsEnumerable();

        protected IList<Photo> _photos;
        public virtual IEnumerable<Photo> Photos => _photos.AsEnumerable();

        public virtual void AddPhoto(Photo photo) => _photos.Add(photo);

        public virtual void AddDocumentation(Documentation documentation)
        {
            documentation.ParentMachine = this;
            _documentations.Add(documentation);
        }

        public override string ToString() => Description;

    }
}