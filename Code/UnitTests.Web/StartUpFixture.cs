using NUnit.Framework;
using System.Web.Http;
using TSS.Web;

namespace UnitTests.Web
{
    //SetUp the webserver in memory for testing
    [SetUpFixture]
    public static class StartUpFixture
    {
        public static HttpConfiguration WebConfig { get; private set; }

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            WebConfig = new HttpConfiguration();
            WebApiConfig.Register(WebConfig);
            WebConfig.EnsureInitialized();
        }

        //Should Only be run when all tests are done, however it seemed to run after the first testcase of a test too...
        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {            
            WebConfig?.Dispose();
        }
    }
}
