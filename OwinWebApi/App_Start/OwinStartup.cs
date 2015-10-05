// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinStartup.cs">
// Copyright (c) 2014-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under <a href="https://github.com/logjam2/examples/blob/master/LICENSE">The MIT License (MIT)</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Tracing;
using LogJam.Trace;
using LogJam.WebApi;
using Microsoft.Owin;
using Owin;

[assembly: Microsoft.Owin.OwinStartup(typeof (LogJam.Examples.OwinWebApi.OwinStartup))]

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

			// Use LogJam for OWIN tracing, and HTTP request logging
			owinAppBuilder.UseOwinTracerLogging();
			owinAppBuilder.LogHttpRequests();

			// Trace OWIN exceptions
			owinAppBuilder.TraceExceptions(logFirstChance: false, logUnhandled: true);
		}

		private void ConfigureWebApiTracing(ITracerFactory tracerFactory, HttpConfiguration webApiConfig)
		{
			webApiConfig.Services.Replace(typeof(ITraceWriter), new LogJamWebApiTraceWriter(tracerFactory));
			//webApiConfig.Services.Add(typeof(IExceptionLogger), new LogJamExceptionLogger(tracerFactory));
		}

	}
}