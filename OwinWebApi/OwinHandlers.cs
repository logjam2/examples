// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinHandlers.cs">
// Copyright (c) 2014-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under <a href="https://github.com/logjam2/examples/blob/master/LICENSE">The MIT License (MIT)</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;
using TraceLevel = LogJam.Trace.TraceLevel;

namespace LogJam.Examples.OwinWebApi
{
	public class OwinHandlers
	{
		private ILogger _owinLogger;

		/// <summary>
		/// Configure handlers for various OWIN urls.
		/// </summary>
		/// <param name="owinAppBuilder"></param>
		public void Configure(IAppBuilder owinAppBuilder)
		{
			_owinLogger = owinAppBuilder.CreateLogger<OwinHandlers>();

            owinAppBuilder.Map("/owin/trace", (appBuilder => appBuilder.Run(OwinTrace)));
			owinAppBuilder.Map("/owin/exception", (appBuilder => appBuilder.Run(OwinException)));
		}

		/// <summary>
		/// An OWIN handler, to test OWIN tracing
		/// </summary>
		/// <param name="owinContext"></param>
		/// <returns></returns>
		private Task OwinTrace(IOwinContext owinContext)
		{
			string message = owinContext.Request.Query.Get("message");
			if (string.IsNullOrEmpty(message))
			{
				message = "Message not specified.";
			}

			int traceCount;
			string traceCountParam = owinContext.Request.Query.Get("count");
			if (! int.TryParse(traceCountParam, out traceCount))
			{
				traceCount = 1;
			}

            TraceEventType traceEventType;
            string severityParam = owinContext.Request.Query.Get("eventType");
            if (! Enum.TryParse(severityParam, true, out traceEventType))
            {
                traceEventType = TraceEventType.Information;
            }

            for (int i = 0; i < traceCount; ++i)
			{
				_owinLogger.WriteCore(traceEventType, 0, this, null, (obj, exception) => message);
			}

			return owinContext.Response.WriteAsync(string.Format("Traced '{0}' {1} times to OWIN Tracer with TraceEventType: {2}", message, traceCount, traceEventType));
		}

		/// <summary>
		/// An OWIN handler, which throws an <see cref="Exception"/>.
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		private Task OwinException(IOwinContext arg)
		{
			throw new Exception("Exception thrown b/c ~/owin/exception URL was called.");
		}

	}
}