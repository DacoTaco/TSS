﻿/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
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

using TechnicalServiceSystem.Entities.General;

namespace TechnicalServiceSystem.Mappings.General
{
    public class CompanyMap : GeneralSchemaMapper<Company>
    {
        public CompanyMap() : base("Company")
        {
            Id(c => c.ID)
                .Column("CompanyID");

            Map(c => c.Name)
                .Column("CompanyName");
        }
    }
}