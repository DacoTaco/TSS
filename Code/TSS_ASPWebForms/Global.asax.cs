using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Optimization;
using System.Web.Routing;
using TechnicalServiceSystem.Utilities;

namespace TSS_ASPWebForms
{
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            //NOTE: Stopping IE from being a caching whore
            HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now);
            Response.Cache.SetValidUntilExpires(true);

            //this creates and sets the session for this connection
            SessionHandler.GetSession();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //free session for this connection
            SessionHandler.UnbindSession();
        }
    }
}