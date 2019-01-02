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

using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Users;

namespace TechnicalServiceSystem.Mappings
{
    public class UserMap : UserSchemaMapper<User>
    {
        public UserMap() : base("Users")
        {
            Id(x => x.ID).Column("UserID");
            Map(x => x.UserName).Column("UserName");
            Map(x => x.IsActive).Column("Active");

            References<Department>(x => x.Department)
                .Column("DepartmentID");

            References<Photo>(x => x.Photo)
                .Column("PhotoID")
                .Cascade.SaveUpdate()
                .Not.LazyLoad()
                .NotFound.Ignore();

            HasManyToMany(r => r.Roles)
                .ParentKeyColumn("UserID")
                .ChildKeyColumn("RoleID")
                .Table("UserRoles")
                .Schema("Users")
                .AsSet()
                .Not.LazyLoad()
                .NotFound.Ignore();
        }
    }
}