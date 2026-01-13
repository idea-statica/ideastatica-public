using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Command to connect to the Connection API service by either starting a new service or attaching to an existing one.
	/// </summary>
	public class ConnectCommand : AsyncCommandBase
	{
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectCommand"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		/// <param name="configuration">Application configuration settings.</param>
		public ConnectCommand(MainWindowViewModel viewModel, IPluginLogger logger, IConfiguration configuration)
			: base(viewModel, logger)
		{
			_configuration = configuration;
		}

		/// <inheritdoc/>
		public override bool CanExecute(object? parameter) => ConApiClient == null;

		/// <inheritdoc/>
		protected override async Task ExecuteAsync(object? parameter)
		{
			_logger.LogInformation("ConnectAsync");

			if (ConApiClient != null)
			{
				throw new Exception("IConnectionApiController is already connected");
			}

			_viewModel.IsBusy = true;

			try
			{
				_viewModel.OutputText = "Attaching to the ConnectionRestApi";
				if (_viewModel.RunApiServer)
				{
					_viewModel.ConnectionApiClientFactory = new ConnectionApiServiceRunner(_configuration["IdeaStatiCaSetupPath"]);
					_viewModel.ConApiClient = await _viewModel.ConnectionApiClientFactory.CreateApiClient();
				}
				else
				{
					if (_viewModel.ApiUri == null)
					{
						throw new Exception("ApiUri is not set");
					}

					_viewModel.ConnectionApiClientFactory = new ConnectionApiServiceAttacher(_configuration["CONNECTION_API_ENDPOINT"]!);
					_viewModel.ConApiClient = await _viewModel.ConnectionApiClientFactory.CreateApiClient();
				}

				if (_viewModel.ConApiClient == null)
				{
					throw new Exception("Can not create ConApiClient");
				}

				_viewModel.OutputText = $"Service Url = {_viewModel.ConApiClient!.ClientApi!.Configuration!.BasePath}\nConnected. ClientId = {_viewModel.ConApiClient.ClientId}";
			}
			catch (Exception ex)
			{
				_logger.LogWarning("ConnectAsync failed", ex);
				_viewModel.OutputText = ex.Message;
			}
			finally
			{
				_viewModel.IsBusy = false;
				_viewModel.RefreshCommands();
			}
		}
	}
}
