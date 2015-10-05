// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiConfig.cs">
// Copyright (c) 2014-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under <a href="https://github.com/logjam2/examples/blob/master/LICENSE">The MIT License (MIT)</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Http;

namespace LogJam.Examples.OwinWebApi
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();
		}
	}
}