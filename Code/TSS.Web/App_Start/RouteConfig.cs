using Microsoft.AspNet.FriendlyUrls;
using System.Web.Routing;

namespace TSS.Web
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //default value : 
            //settings.AutoRedirectMode = RedirectMode.Permanent;
            //changed to :
            var settings = new FriendlyUrlSettings()
            {
                AutoRedirectMode = RedirectMode.Off
            };            

            routes.EnableFriendlyUrls(settings);
        }
    }
}
