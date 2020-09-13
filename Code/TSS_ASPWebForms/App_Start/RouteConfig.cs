using Microsoft.AspNet.FriendlyUrls;
using System.Web.Routing;

namespace TSS_ASPWebForms
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings();
            //default value : 
            //settings.AutoRedirectMode = RedirectMode.Permanent;
            //changed to :
            settings.AutoRedirectMode = RedirectMode.Off;
            routes.EnableFriendlyUrls(settings);
        }
    }
}
