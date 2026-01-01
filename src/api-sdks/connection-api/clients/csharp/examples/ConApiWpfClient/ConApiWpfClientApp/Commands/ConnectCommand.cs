using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Commands
{
	public class ConnectCommand : AsyncCommandBase
	{
		private readonly IConfiguration _configuration;

		public ConnectCommand(MainWindowViewModel viewModel, IPluginLogger logger, IConfiguration configuration)
			: base(viewModel, logger)
		{
			_configuration = configuration;
		}

		public override bool CanExecute(object? parameter) => ConApiClient == null;

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
