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

using FluentNHibernate;
using TechnicalServiceSystem.Entities.Tasks;

namespace TechnicalServiceSystem.Mappings.Tasks
{
    public class TaskMap : TaskSchemaMapper<Task>
    {
        public TaskMap() : base("Task")
        {
            Id(x => x.ID).Column("TaskID");
            Map(x => x.Description).Column("TaskDescription");
            Map(x => x.IsUrguent).Column("Urguent");
            Map(x => x.CreationDate).Column("CreationDate");
            Map(x => x.LastModifiedOn).Column("DateLastAdjustment");
            Map(x => x.OpenedBy).Column("UserOpened");
            Map(x => x.OpenedTimeDue).Column("OpenTimeDue");
            Map(t => t.TypeID).Column("TypeID");
            Map(t => t._statusID).Column("StatusID");

            References(t => t._technician)
                .Column("TechnicianID")
                .NotFound.Ignore();
            References(t => t.Location)
                .Column("LocationID");
            References(t => t.Device)
                .Column("MachineID")
                .NotFound.Ignore();

            HasOne(t => t.RepeatingInfo)
                .PropertyRef("ParentTask");

            References(t => t.ReporterUser)
                .Column("ReporterID")
                .NotFound.Ignore();
            Map(Reveal.Member<Task>("_reporterName")).Column("ReporterName");

            HasMany(t => t.Notes)
                .KeyColumn("TaskID")
                .NotFound.Ignore();

            HasManyToMany(t => t.Photos)
                .ParentKeyColumn("TaskID")
                .ChildKeyColumn("PhotoID")
                .Cascade.SaveUpdate()
                .Table("TaskPhotos")
                .Schema("Tasks")
                .AsSet()
                .NotFound.Ignore();
        }
    }
}