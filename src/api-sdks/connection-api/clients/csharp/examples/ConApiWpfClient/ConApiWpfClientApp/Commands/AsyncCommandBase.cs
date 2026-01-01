using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConApiWpfClientApp.Commands
{
	public abstract class AsyncCommandBase : ICommand
	{
		protected readonly MainWindowViewModel _viewModel;
		protected readonly IPluginLogger _logger;

		public event EventHandler? CanExecuteChanged;

		protected AsyncCommandBase(MainWindowViewModel viewModel, IPluginLogger logger)
		{
			_viewModel = viewModel;
			_logger = logger;
		}

		protected IConnectionApiClient? ConApiClient => _viewModel.ConApiClient;

		public abstract bool CanExecute(object? parameter);

		public async void Execute(object? parameter)
		{
			try
			{
				await ExecuteAsync(parameter);
			}
			catch (Exception ex)
			{
				_logger.LogWarning($"{GetType().Name} failed", ex);
				_viewModel.OutputText = ex.Message;
			}
		}

		protected abstract Task ExecuteAsync(object? parameter);

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
