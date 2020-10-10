using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.General;

namespace TSS.Web.Feature.General
{
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
        public IHttpActionResult Get(int id)
        {
            if (Settings.RequireLogin() && !LoggedInUser.IsUserLoggedIn)
                return BadRequest("Invalid authorisation.");

            var list = new List<Location>();
            try
            {
                var generalMngr = new GeneralManager();
                list = generalMngr.GetLocations(id, Settings.GetCompanyName())?.ToList() ?? new List<Location>();
                return Ok(list.Select(x => new LocationModel(x)).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
