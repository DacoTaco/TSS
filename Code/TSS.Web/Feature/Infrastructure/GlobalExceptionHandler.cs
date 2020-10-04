using System;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace TSS.Web.Feature.Infrastructure
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        private static readonly ProblemDetailJsonMediaTypeFormatter MediaTypeFormatter = new ProblemDetailJsonMediaTypeFormatter();
        public GlobalExceptionHandler() { }

        public override void Handle(ExceptionHandlerContext context)
        {
            var baseUri = new Uri(context.Request.RequestUri.GetLeftPart(UriPartial.Authority) + context.Request.GetRequestContext().VirtualPathRoot);
            var correlationID = context.Request.GetCorrelationId().ToString();
            var problemDetail = context.Exception.ToProblemDetail(baseUri, correlationID);
            var content = new ObjectContent<ProblemDetail>(problemDetail, MediaTypeFormatter);

            context.Result = new ResponseMessageResult(new HttpResponseMessage(problemDetail.StatusCode)
            {
                Content = content
            });
        }
    }
}