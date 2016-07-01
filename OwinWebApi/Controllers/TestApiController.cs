using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiTracing = System.Web.Http.Tracing;
using LogJam.Trace;
using Microsoft.Owin;

namespace LogJam.Examples.OwinWebApi.Controllers
{

    [RoutePrefix("webapi")]
    public class TestApiController : ApiController
    {

        // GET: webapi/logjam-trace
        [HttpGet, Route("logjam-trace")]
        public string LogJamTrace(string message = null, TraceLevel severity = TraceLevel.Info, int count = 1)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "Message not specified.";
            }

            Tracer tracer = Request.GetOwinContext().GetTracerFactory().TracerFor(this);
            for (int i = 0; i < count; ++i)
            {
                tracer.Trace(severity, message);
            }

            return string.Format("Traced '{0}' {1} times to LogJam Tracer with severity: {2}", message, count, severity);
        }

        // GET: webapi/webapi-trace
        [HttpGet, Route("webapi-trace")]
        public string WebApiTrace(string message = null, WebApiTracing.TraceLevel webApiTraceLevel = WebApiTracing.TraceLevel.Info, int count = 1)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "Message not specified.";
            }

            WebApiTracing.ITraceWriter webApiTraceWriter = Configuration.Services.GetTraceWriter();
            for (int i = 0; i < count; ++i)
            {
                webApiTraceWriter.Trace(Request, GetType().FullName, webApiTraceLevel, record => record.Message = message);
            }

            return string.Format("Traced '{0}' {1} times to Web-Api ITraceWriter with severity: {2}", message, count, webApiTraceLevel);
        }

        // GET: webapi/exception
        [HttpGet, Route("exception")]
        public void Exception()
        {
            throw new Exception("Exception thrown b/c ~/webapi/exception API was called.");
        }

        // GET: webapi/delay
        [HttpGet, Route("delay")]
        public async Task<string> Delay(int millisecondsDelay = 500)
        {
            await Task.Delay(millisecondsDelay);

            return string.Format("Response delayed by {0} ms.", millisecondsDelay);
        }

    }
}
