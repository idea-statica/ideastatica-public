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

		///// <inheritdoc cref="IRcsApiClient.ClientId"/>/>
		//public string ClientId { get; private set; }

		/// <inheritdoc cref="IRcsApiClient.ProjectId"/>/>
		public Guid ProjectId
		{
			get => this.Project == null ? this.Project.ProjectId : Guid.Empty;
		}



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
			//if(Project != null && ProjectId != Guid.Empty)
			//{
			//	await Project.CloseProjectAsync(ProjectId);
			//}

			//this.Calculation = null;
			//this.Connection = null;
			//this.Export = null;
			//this.LoadEffect = null;
			//this.Material = null;
			//this.Member = null;
			//this.Operation = null;
			//this.Parameter = null;
			//this.Presentation = null;
			//this.Project = null;
			//this.Report = null;
			//this.Template = null;
			//this.ClientApi = null;
			//this.ClientId = string.Empty;

		}

		private async Task<string> CreateClientAsync()
		{
			Configuration configuration = new Configuration();
			configuration.BasePath = BasePath.AbsoluteUri;

			//var clientApi = new ClientApi(configuration);
			//string clientId = await clientApi.ConnectClientAsync();
			//configuration.DefaultHeaders.Add("ClientId", clientId);

			//this.Calculation = new CalculationApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.Connection = new IdeaStatiCa.ConnectionApi.Api.ConnectionApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.Export = new ExportApiExt(clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.LoadEffect = new LoadEffectApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.Material = new MaterialApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.Member = new MemberApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.Operation = new OperationApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.Parameter = new ParameterApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.Presentation = new PresentationApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.Project = new ProjectApiExt(this, clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.Report = new ReportApiExt(clientApi.Client, clientApi.AsynchronousClient, configuration);
			//this.Template = new TemplateApiExt(clientApi.Client, clientApi.AsynchronousClient, configuration);

			//this.ClientApi = clientApi;
			//this.ClientId = clientId;
			return string.Empty;
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
