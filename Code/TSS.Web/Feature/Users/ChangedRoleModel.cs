using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TSS.Web.Feature.Users
{
    public class ChangedRoleModel
    {
        private ChangedRoleModel() { }
        public int ID { get; set; }
        public bool IsChecked { get; set; }
        public static ChangedRoleModel TryParse(string json)
        {
            try
            {
                var objectData = JsonConvert.DeserializeObject(json);
                if (objectData == null) return null;
                var jObject = objectData as JObject;
                return jObject.ToObject<ChangedRoleModel>();
            }
            catch
            {
                return null;
            }
        }
    }
}