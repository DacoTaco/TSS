using System;
using System.Web;
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

            //this creates and sets the session for the application
            //TODO : to be used once we use nhibernate completely instead of 'binding' a new session for every request
            //SessionHandler.BindSession();
        }

        private void Application_End(object sender, EventArgs e)
        {
            //unbind the session for this application
            SessionHandler.UnbindSession();
        }

        protected void Application_BeginRequest()
        {
            //NOTE: Stopping IE from being a caching whore
            HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now);
            Response.Cache.SetValidUntilExpires(true);
            SessionHandler.BindSession();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            SessionHandler.UnbindSession();
        }
    }
}