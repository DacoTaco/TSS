using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TechnicalServiceSystem.Entities.Users;

namespace TSS.Web.Feature.Users
{
    public class RoleModel
    {
        public Role Role { get; set; }
        public string RoleName { get; set; }
        public bool IsChecked { get; set; }
        public static RoleModel TryParse(string json)
        {
            try
            {
                var objectData = JsonConvert.DeserializeObject(json);
                if (objectData == null) return null;
                var jObject = objectData as JObject;
                var roleModel = jObject.ToObject<RoleModel>();
                roleModel.RoleName = roleModel.Role.ToString();
                return roleModel;
            }
            catch
            {
                return null;
            }
        }

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