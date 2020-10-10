using System;
using TechnicalServiceSystem.Entities.Users;

namespace TSS.Web.Feature.Users
{
    public class RoleModel
    {
        public Role Role { get; set; }
        public string RoleName { get; set; }

        public static RoleModel CreateModel(Role role, string translatedName = "")
        {
            return new RoleModel()
            {
                Role = role,
                RoleName = string.IsNullOrWhiteSpace(translatedName) ? Enum.GetName(typeof(Role),role) : translatedName
            };
        }

        public override string ToString() => RoleName;
    }
}