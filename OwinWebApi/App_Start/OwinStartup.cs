// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinStartup.cs">
// Copyright (c) 2014-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under <a href="https://github.com/logjam2/examples/blob/master/LICENSE">The MIT License (MIT)</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Tracing;
using LogJam.Config;
using LogJam.Examples.OwinWebApi;
using LogJam.Trace;
using LogJam.Trace.Config;
using LogJam.Trace.Format;
using LogJam.WebApi;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof (OwinStartup))]

namespace LogJam.Examples.OwinWebApi
{
	public class OwinStartup
	{
		public void Configuration(IAppBuilder owinAppBuilder)
		{
			// OWIN logging should be configured first, so everything gets logged
			ConfigureWebAppLogging(owinAppBuilder);

			// OWIN error page should be next in the pipeline
			owinAppBuilder.UseErrorPage();

			// Some OWIN Handlers for testing
			new OwinHandlers().Configure(owinAppBuilder);

			// Web API
			var webApiConfig = new HttpConfiguration();
			ConfigureWebApiTracing(owinAppBuilder.GetTracerFactory(), webApiConfig);
			WebApiConfig.Register(webApiConfig);
			owinAppBuilder.UseWebApi(webApiConfig);
		}

		/// <summary>
		/// Main LogJam initialization, and configure LogJam for OWIN tracing, and HTTP request logging
		/// </summary>
		/// <param name="owinAppBuilder"></param>
		private void ConfigureWebAppLogging(IAppBuilder owinAppBuilder)
		{
			// Configure LogWriters
		    if (ShouldLogToConsole(owinAppBuilder))
		    {
                // TODO: Support UseConsoleIfAvailable()
		        owinAppBuilder.GetLogManagerConfig().UseConsole();
		        Console.WriteLine("Console logging configured");
		    }
		    else
		    {
                Console.WriteLine("Console logging NOT configured");
            }

            // Normally you don't need to do this - debug output is enabled if a debugger is attached during LogJam initialization.
            // HOWEVER, in this case (using OwinHost.exe), the debugger isn't attached during initialization, so this is enabled explicitly.
		    owinAppBuilder.GetLogManagerConfig().UseDebugger();

            // Use LogJam for OWIN tracing, and HTTP request logging
            // TODO: Support owinAppBuilder.GetTraceManager().TraceToAll(traceFormatter: new DefaultTraceFormatter() { IncludeTimestamp = true });
		    owinAppBuilder.GetTraceManagerConfig().TraceTo(owinAppBuilder.GetLogManagerConfig().Writers, 
                traceFormatter: new DefaultTraceFormatter() { IncludeTimestamp = true });
            owinAppBuilder.UseOwinTracerLogging();
		    owinAppBuilder.LogHttpRequestsToAll();

			// Trace OWIN exceptions
			owinAppBuilder.TraceExceptions(logFirstChance: false, logUnhandled: true);
		}

		/// <summary>
		/// Returns <c>true</c> if logging to the console (aka stdout) should be enabled.
		/// </summary>
		/// <param name="owinAppBuilder"></param>
		/// <returns></returns>
		protected bool ShouldLogToConsole(IAppBuilder owinAppBuilder)
		{
			// This is a test we use to determine whether it's valid to write to the console - eg it's valid in OwinHost, but not in IIS
			object value;
			return owinAppBuilder.Properties.TryGetValue("host.TraceOutput", out value)
			       && (value is TextWriter);
		}

		private void ConfigureWebApiTracing(ITracerFactory tracerFactory, HttpConfiguration webApiConfig)
		{
			webApiConfig.Services.Replace(typeof(ITraceWriter), new LogJamWebApiTraceWriter(tracerFactory));
			webApiConfig.Services.Add(typeof(IExceptionLogger), new LogJamExceptionLogger(tracerFactory));
		}

	}
}