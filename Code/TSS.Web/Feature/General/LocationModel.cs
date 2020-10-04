using TechnicalServiceSystem.Entities.General;

namespace TSS.Web.Feature.General
{
    public class LocationModel
    {
        public string Description { get; }
        public int ID { get; }

        public LocationModel(Location location)
        {
            ID = location.ID;
            Description = location.Description;
        }
    }
}