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

using TechnicalServiceSystem.Entities.Tasks;

namespace TechnicalServiceSystem.Mappings.Tasks
{
    public class NoteMap : TaskSchemaMapper<Note>
    {
        public NoteMap() : base("Notes")
        {
            Id(n => n.ID).Column("NoteID");
            Map(n => n.Text).Column("Note");
            Map(n => n.NoteDate).Column("NoteDate");

            //not sure if this mapping is needed once we add notes. probably we do
            References(n => n.NoteTask)
                .Column("TaskID")
                .Cascade.All()
                .ReadOnly();
        }
    }
}