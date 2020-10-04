using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.General;

namespace TSS.Web.Feature.General
{
    //[Route("~/api/Locations")]
    [RoutePrefix("api/Department")]
    public class DepartmentController : ApiController
    {
        // GET: api/Locations
        //[Route("")]
        //[HttpGet]
        //public string Get()
        //{
        //    return "";
        //}

        // GET: api/Locations/5
        [Route("{id}/Locations")]
        [HttpGet]
        [HttpPost]
        public List<LocationModel> Get(int id)
        {
            var list = new List<Location>();
            try
            {
                var generalMngr = new GeneralManager();
                list = generalMngr.GetLocations(id, Settings.GetCompanyName())?.ToList() ?? new List<Location>();
                return list.Select(x => new LocationModel(x)).ToList();
            }
            catch
            {
                return new List<LocationModel>();
            }           
        }
    }
}
