using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;

namespace UnitTests.Web
{
    public abstract class ControllerTestFixture
    {
        //run this code block in a transaction that will get rollbacked later
        private TransactionScope _transactionScope;

        [TearDown]
        protected void TearDownSession()
        {
            _transactionScope.Dispose();
        }

        [SetUp]
        protected void SetUpSession()
        {
            _transactionScope = new TransactionScope();
        }

        protected async Task<HttpResponseMessage> GetAsync(string url, HttpStatusCode expectedStatus = HttpStatusCode.OK)
        {
            using (var server = new HttpServer(StartUpFixture.WebConfig))
            using (var client = new HttpClient(server))
            {
                var response = await client.GetAsync($"http://ThisPartDoesntMatter/{url}").ConfigureAwait(false);
                if (response.StatusCode == expectedStatus)
                    return response;

                var errorMessage = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Assert.Fail($"Received unexpected HTTP Status code {(int)response.StatusCode} while expecting {(int)expectedStatus} : {Environment.NewLine}{errorMessage}");
                return response;
            }
        }

        protected async Task<HttpResponseMessage> PostAsync(string url, StringContent data, HttpStatusCode expectedStatus = HttpStatusCode.OK)
        {
            using (var server = new HttpServer(StartUpFixture.WebConfig))
            using (var client = new HttpClient(server))
            {
                var response = await client.PostAsync($"http://ThisPartDoesntMatter/{url}", data).ConfigureAwait(false);
                if (response.StatusCode == expectedStatus)
                    return response;

                var errorMessage = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Assert.Fail($"Received unexpected HTTP Status code {(int)response.StatusCode} while expecting {(int)expectedStatus} : {Environment.NewLine}{errorMessage}");
                return response;
            }
        }
    }
}
