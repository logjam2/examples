// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinHandlers.cs">
// Copyright (c) 2014-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under <a href="https://github.com/logjam2/examples/blob/master/LICENSE">The MIT License (MIT)</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Owin;

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
		}

		/// <summary>
		/// A sample OWIN handler, to test OWIN tracing
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
			string traceCountParam = owinContext.Request.Query.Get("traceCount");
			if (! int.TryParse(traceCountParam, out traceCount))
			{
				traceCount = 1;
			}

			for (int i = 0; i < traceCount; ++i)
			{
				_owinLogger.WriteInformation(message);
			}

			return owinContext.Response.WriteAsync(string.Format("Traced '{0}' {1} times to OWIN Tracer with severity: Information", message, traceCount));
		}


	}
}