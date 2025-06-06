﻿using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// Connection API client. A simplified API client which combines all REST API endpoints into a single client for use in .NET applications.
	/// </summary>
	public class ConnectionApiClient : IConnectionApiClient
	{
		private bool disposedValue;
		
		/// <summary>
		/// Base uri path of the client.
		/// </summary>
		public Uri BasePath { get; private set; }

		/// <inheritdoc cref="IConnectionApiClient.ActiveProjectId"/>/>
		public Guid ActiveProjectId
		{
			get => this.Project.ProjectId;
		}

		/// <summary>
		/// Client API
		/// </summary>
		public IClientApi ClientApi { get; private set; }

		/// <inheritdoc cref="IConnectionApiClient.Calculation"/>
		public ICalculationApiAsync Calculation { get; private set; }
		/// <inheritdoc cref="IConnectionApiClient.Connection"/>
		public IConnectionApiAsync Connection { get; private set; }
		/// <inheritdoc cref="IConnectionApiClient.Export"/>
		public IExportApiExtAsync Export { get; private set; }
		/// <inheritdoc cref="IConnectionApiClient.LoadEffect"/>
		public ILoadEffectApiAsync LoadEffect { get; private set; }
		
		/// <inheritdoc cref="IConnectionApiClient.Material"/>
		public IMaterialApiAsync Material { get; private set; }
		
		/// <inheritdoc cref="IConnectionApiClient.Member"/>
		public IMemberApiAsync Member { get; private set; }
		
		/// <inheritdoc cref="IConnectionApiClient.Operation"/>
		public IOperationApiAsync Operation { get; private set; }
		
		/// <inheritdoc cref="IConnectionApiClient.Parameter"/>
		public IParameterApiAsync Parameter { get; private set; }
		
		/// <inheritdoc cref="IConnectionApiClient.Presentation"/>
		public IPresentationApiAsync Presentation { get; private set; }
		
		/// <inheritdoc cref="IConnectionApiClient.Project"/>
		public IProjectApiExtAsync Project { get; private set; }
		
		/// <inheritdoc cref="IConnectionApiClient.Report"/>
		public IReportApiExtAsync Report { get; private set; }
		
		/// <inheritdoc cref="IConnectionApiClient.Template"/>
		public ITemplateApiExtAsync Template { get; private set; }

		/// <inheritdoc cref="IConnectionApiClient.Conversion"/>
		public IConversionApiAsync Conversion { get; private set; }

		/// <inheritdoc cref="IConnectionApiClient.ClientId"/>
		public string ClientId { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="basePath"></param>
		public ConnectionApiClient(string basePath)
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
			if (ClientApi != null)
			{
				throw new Exception("Client is already connected");
			}

			await CreateClientAsync();
		}

		private async Task CloseAsync()
		{
			if(Project != null && ActiveProjectId != Guid.Empty)
			{
				await Project.CloseProjectAsync(ActiveProjectId);
			}

			this.Calculation = null;
			this.Connection = null;
			this.Export = null;
			this.LoadEffect = null;
			this.Material = null;
			this.Member = null;
			this.Operation = null;
			this.Parameter = null;
			this.Presentation = null;
			this.Project = null;
			this.Report = null;
			this.Template = null;
			this.Conversion = null;
			this.ClientApi = null;

		}

		private async Task<string> CreateClientAsync()
		{
			Configuration configuration = new Configuration();
			configuration.Timeout = Timeout.Infinite;
			configuration.BasePath = BasePath.AbsoluteUri;

			var clientApi = new ClientApi(configuration);
			ClientId = await clientApi.ConnectClientAsync();
			configuration.DefaultHeaders.Add("ClientId", ClientId);

			this.Calculation = new CalculationApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Connection = new IdeaStatiCa.ConnectionApi.Api.ConnectionApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Export = new ExportApiExt(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.LoadEffect = new LoadEffectApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Material = new MaterialApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Member = new MemberApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Operation = new OperationApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Parameter = new ParameterApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Presentation = new PresentationApi(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Project = new ProjectApiExt(this, clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Report = new ReportApiExt(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Template = new TemplateApiExt(clientApi.Client, clientApi.AsynchronousClient, configuration);
			this.Conversion = new ConversionApi(clientApi.Client, clientApi.AsynchronousClient, configuration);

			this.ClientApi = clientApi;
			return ClientId;
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
