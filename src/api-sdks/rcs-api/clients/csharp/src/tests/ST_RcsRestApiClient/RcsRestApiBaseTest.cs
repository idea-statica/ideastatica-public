﻿using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.RcsApi;
using Microsoft.Extensions.Configuration;

namespace ST_ConnectionRestApi
{
	public class RcsRestApiBaseTest
	{

		protected Uri? ApiUri { get; set; }

		protected IRcsApiClient? RcsApiClient { get; set; }

		protected IApiServiceFactory<IRcsApiClient> ApiFactory { get; set; }

		protected bool RunServer { get; set; }

		protected string ProjectPath { get; set; }

		protected RcsProject? Project { get; set; } = null;

		protected Guid ActiveProjectId { get; set; } = Guid.Empty;

		[OneTimeSetUp]
		public async Task OneTimeSetUp()
		{
			var configBuilder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables();

			// Build the configuration
			var configuration = configBuilder.Build();
			if (configuration == null)
			{
				throw new Exception("No configuration found");
			}

			var currentDir = AppDomain.CurrentDomain.BaseDirectory;
			ProjectPath = Path.Combine(currentDir, "Projects");

			var setupDir = configuration["IdeaStatiCaSetupPath"];
			this.RunServer = string.IsNullOrEmpty(configuration["CONNECTION_API_RUNSERVER"]) ? true : configuration["CONNECTION_API_RUNSERVER"]! == "true";
			this.ApiUri = string.IsNullOrEmpty(configuration["CONNECTION_API_ENDPOINT"]) ? null : new Uri(configuration["CONNECTION_API_ENDPOINT"]!);

			if(ApiUri == null)
			{
				throw new NotImplementedException("Implement the use case of starting ConnectionRestApi service");
			}
			this.ApiFactory = new RcsApiServiceAttacher(ApiUri.AbsoluteUri);

			await Task.CompletedTask;

			//IConnectionApiClientFactory? connectionApiClientFactory = null;
			//if (this.RunServer == false)
			//{
			//	// wait till the service is running otherwise docker-compose is not stable
			//	if (ApiUri == null)
			//	{
			//		throw new Exception("ApiUri is not set");
			//	}

			//	int attempt = 3;

			//	while (attempt > 0)
			//	{
			//		try
			//		{
			//			// wait till the server is ready
			//			connectionApiClientFactory = await ApiFactory.CreateConnectionApiClient();
			//			break;
			//		}
			//		catch (Exception)
			//		{
			//			attempt--;
			//			await Task.Delay(1000);
			//		}
			//	}

			//	if (attempt == 0 || connectionApiClientFactory == null)
			//	{
			//		throw new Exception("Service is not ready");
			//	}
			//	else
			//	{
			//		//connectionApiClientFactory.Dispose();
			//	}
			//}

		}

	}
}
