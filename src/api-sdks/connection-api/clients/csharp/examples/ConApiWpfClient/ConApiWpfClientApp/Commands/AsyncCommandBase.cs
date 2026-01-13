using ConApiWpfClientApp.ViewModels;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConApiWpfClientApp.Commands
{
	/// <summary>
	/// Base class for asynchronous WPF commands that provides common functionality
	/// for command execution, error handling, and state management.
	/// </summary>
	public abstract class AsyncCommandBase : ICommand
	{
		/// <summary>
		/// The view model that owns this command.
		/// </summary>
		protected readonly MainWindowViewModel _viewModel;
		
		/// <summary>
		/// Logger for tracking command execution and errors.
		/// </summary>
		protected readonly IPluginLogger _logger;

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler? CanExecuteChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCommandBase"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that owns this command.</param>
		/// <param name="logger">Logger for tracking command execution.</param>
		protected AsyncCommandBase(MainWindowViewModel viewModel, IPluginLogger logger)
		{
			if(viewModel == null)
			{
				throw new ArgumentNullException(nameof(viewModel));
			}

			_viewModel = viewModel;
			_logger = logger;
		}

		/// <summary>
		/// Gets the Connection API client from the view model.
		/// </summary>
		protected IConnectionApiClient? ConApiClient => _viewModel.ConApiClient;

		/// <summary>
		/// Determines whether this command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command. Can be null.</param>
		/// <returns>true if this command can be executed; otherwise, false.</returns>
		public abstract bool CanExecute(object? parameter);

		/// <summary>
		/// Executes the command asynchronously.
		/// </summary>
		/// <param name="parameter">Data used by the command. Can be null.</param>
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

		/// <summary>
		/// When implemented in a derived class, executes the command's logic asynchronously.
		/// </summary>
		/// <param name="parameter">Data used by the command. Can be null.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		protected abstract Task ExecuteAsync(object? parameter);

		/// <summary>
		/// Raises the <see cref="CanExecuteChanged"/> event to notify that the command's execution state has changed.
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
