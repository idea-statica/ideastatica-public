using IdeaStatiCa.RcsApi.Api;
using IdeaStatiCa.RcsApi.Client;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.RcsApi
{
	public class RcsApiClient : IRcsApiClient
	{
		private bool disposedValue;
		
		/// <summary>
		/// 
		/// </summary>
		public Uri BasePath { get; private set; }

		/// <inheritdoc cref="IRcsApiClient.Calculation"/>
		public ICalculationApiAsync Calculation { get; private set; }

		/// <ihneritdoc cref="IRcsApiClient.CrossSection"/>
		public ICrossSectionApiAsync CrossSection { get; private set; }

		/// <inheritdoc cref="IRcsApiClient.DesignMember"/>
		public IDesignMemberApiAsync DesignMember { get; private set; }

		/// <inheritdoc cref="IRcsApiClient.InternalForces"/>
		public IInternalForcesApiAsync InternalForces { get; private set; }

		/// <inheritdoc cref="IRcsApiClient.Project"/>
		public IProjectApiExtAsync Project { get; private set; }

		/// <inheritdoc cref="IRcsApiClient.Section"/>/
		public ISectionApiAsync Section { get; private set; }


		/// <summary>
		/// 
		/// </summary>
		/// <param name="basePath"></param>
		public RcsApiClient(string basePath)
		{
			BasePath = new Uri(basePath);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task CreateAsync()
		{
			if (Project != null)
			{
				throw new Exception("Client is already connected");
			}

			await CreateClientAsync();
		}

		private async Task CloseAsync()
		{
			if (Project != null && Project.ProjectId != Guid.Empty)
			{
				await Project.CloseProjectAsync(Project.ProjectId);
			}

			this.Calculation = null;
			this.CrossSection = null;
			this.DesignMember = null;
			this.InternalForces = null;
			this.Project = null;
			this.Section = null;
		}

		private async Task<string> CreateClientAsync()
		{
			Configuration configuration = new Configuration();
			configuration.BasePath = BasePath.AbsoluteUri;

			//var clientApi = new ClientApi(configuration);
			//string clientId = await clientApi.ConnectClientAsync();
			//configuration.DefaultHeaders.Add("ClientId", clientId);

			var calculationApi = new CalculationApi(configuration);

			this.Calculation = calculationApi;
			this.CrossSection = new CrossSectionApi(calculationApi.Client, calculationApi.AsynchronousClient, configuration);
			this.DesignMember = new DesignMemberApi(calculationApi.Client, calculationApi.AsynchronousClient, configuration);
			this.InternalForces = new InternalForcesApi(calculationApi.Client, calculationApi.AsynchronousClient, configuration);
			this.Project = new ProjectApiExt(this, calculationApi.Client, calculationApi.AsynchronousClient, configuration);
			this.Section = new SectionApi(calculationApi.Client, calculationApi.AsynchronousClient, configuration);

			return await Task.FromResult(string.Empty);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					CloseAsync().Wait();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		protected virtual async Task DisposeAsync(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					await CloseAsync();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		/// <summary>
		/// Dispose for oll .Net Frameworks
		/// C# 8.0 and higher should use DisposeAsync
		/// <see href="https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync"/>
		/// </summary>
		[Obsolete("Use DisposeAsync")]
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public async ValueTask DisposeAsync()
		{
			await DisposeAsync(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
