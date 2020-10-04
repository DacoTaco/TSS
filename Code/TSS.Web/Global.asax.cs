using System;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using TechnicalServiceSystem.Utilities;

namespace TSS.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            /*var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All;
            json.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;*/
            GlobalConfiguration.Configuration.Formatters
                .Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }

        private void Application_End(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //get the exception message, set it in the session variable and load the error page.
            //normally the exception is a HttpUnHandledException which has the exception as an innerexception
            var exception = Server.GetLastError();
            var message = exception?.InnerException?.Message ?? exception?.Message;
            Session["exceptionMessage"] = message;

            //server.transfer will redirect the page to be loaded so the url doesn't change
            //Response.Redirect on the other hand tells the browser to load a different page (302)
            Server.Transfer("~/DisplayError.aspx");
            Server.ClearError();
        }

        protected void Application_BeginRequest()
        {
            //NOTE: Stopping IE from being a caching whore
            HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now);
            Response.Cache.SetValidUntilExpires(true);
            new SessionHandler().BindSession();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            new SessionHandler().CloseSession();
        }
    }
}