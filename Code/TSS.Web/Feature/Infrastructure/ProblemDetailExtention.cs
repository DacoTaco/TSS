using System;
using System.Net;

namespace TSS.Web.Feature.Infrastructure
{
    public static class ProblemDetailExtention
    {
        public static ProblemDetail ToProblemDetail(this Exception exception, Uri baseUri, string correlationID)
        {
            var problemDetail = new ProblemDetail()
            {
                CorrelationID = correlationID,
                Detail = exception.Message
            };

            //add different exception handling here
            switch(exception)
            {
                default:
                    problemDetail.Type = baseUri.AppendType("general-problem");
                    problemDetail.Title = "General Error";
                    problemDetail.StatusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            return problemDetail;
        }

        public static Uri AppendType(this Uri baseUri, string type)
        {
            var uriBuilder = new UriBuilder(baseUri);

            uriBuilder.Path += $"{(uriBuilder.Path.EndsWith("/")?"":"/")}problems/{type}";

            return uriBuilder.Uri;
        }
    }
}