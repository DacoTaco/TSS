using Newtonsoft.Json;
using System;
using System.Net;

namespace TSS.Web.Feature.Infrastructure
{
    public class ProblemDetail
    {
        public Uri Type { get; set; }
        public string Title { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Detail { get; set; }
        public string CorrelationID { get; set; }
        public Uri UpstreamUri { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented });
        }
    }
}