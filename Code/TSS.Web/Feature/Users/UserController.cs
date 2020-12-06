using System;
using System.Web.Http;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.Users;

namespace TSS.Web.Feature.Users
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        [Route("Role/{roleName}/IsAdmin")]
        [HttpGet]
        public IHttpActionResult Get(string roleName)
        {
            if (Settings.RequireLogin() && !LoggedInUser.IsUserLoggedIn)
                return BadRequest("Invalid authorisation.");

            try
            {
                var role = (Role)Enum.Parse(typeof(Role), roleName);
                return Ok(role == Role.Admin);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}