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

using FluentNHibernate.Mapping;
using TechnicalServiceSystem.Entities.Suppliers;

namespace TechnicalServiceSystem.Mappings.Suppliers
{
    public class MachineMap : SupplierSchemaMapper<Machine>
    {
        public MachineMap() : base("Machine")
        {
            Id(m => m.ID).Column("MachineID");
            Map(m => m.Description).Column("MachineName");
            Map(m => m.SerialNumber).Column("SerialNumber");
            Map(m => m.ModelNumber).Column("ModelNumber");
            Map(m => m.ModelName).Column("ModelName");

            References(m => m.Type).Column("TypeID");
            References(m => m.Supplier).Column("SupplierID");

            HasMany(m => m.Documentations)
                .KeyColumn("MachineID")
                .NotFound.Ignore()
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.All()
                .LazyLoad();

            HasManyToMany(t => t.Photos)
                .ParentKeyColumn("MachineID")
                .ChildKeyColumn("PhotoID")
                .Table("MachinePhotos")
                .Schema("Suppliers")
                .Access.CamelCaseField(Prefix.Underscore)
                .AsBag()
                .Cascade.All()
                .NotFound.Ignore();
        }
    }
}