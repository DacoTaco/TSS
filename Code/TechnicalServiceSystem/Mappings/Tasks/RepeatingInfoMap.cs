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
    public class RepeatingInfoMap : TaskSchemaMapper<RepeatingInfo>
    {
        public RepeatingInfoMap() : base("RepeatingInfo")
        {
            Id(x => x.ID).Column("InfoID");
            Map(Reveal.Member<RepeatingInfo>("_activationDate"))
                .Column("ActivationDate");
            Map(Reveal.Member<RepeatingInfo>("_interval"))
                .Column("RepeatInterval");
            References(x => x.ParentTask)
                .Column("ParentTaskID");
            References(x => x.ChildTask)
                .Column("ChildTaskID")
                .NotFound.Ignore();
        }
    }
}