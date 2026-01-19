using CommunityToolkit.Mvvm.Input;
using ConnectionIomGenerator.UI.Models;
using ConnectionIomGenerator.UI.Services;
using IdeaStatiCa.Plugin;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConnectionIomGenerator.UI.ViewModels
{
	/// <summary>
	/// ViewModel for the IOM Editor dialog window.
	/// Wraps MainWindowViewModel and adds dialog-specific functionality.
	/// </summary>
	public class IomEditorWindowViewModel : ViewModelBase
	{
		private readonly ConnectionIomGenerator.UI.ViewModels.IomGeneratorViewModel _iomEditorViewModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="IomEditorWindowViewModel"/> class with initial model.
		/// </summary>
		/// <param name="model">Initial model data.</param>
		/// <param name="logger">Logger for tracking operations.</param>
		/// <param name="iomService">Service for IOM operations.</param>
		/// <param name="fileDialogService">Service for file dialogs.</param>
		public IomEditorWindowViewModel(
			IomGeneratorModel model,
			IPluginLogger logger,
			IIomService iomService,
			IFileDialogService fileDialogService)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			_iomEditorViewModel = new IomGeneratorViewModel(model, logger, iomService, fileDialogService);
			
			OkCommand = new RelayCommand(ExecuteOk, CanExecuteOk);
			CancelCommand = new RelayCommand(ExecuteCancel);
		}

		/// <summary>
		/// Gets the wrapped MainWindowViewModel.
		/// This is bound to the ConnectionInputView DataContext.
		/// </summary>
		public IomGeneratorViewModel IomEditorViewModel => _iomEditorViewModel;

		/// <summary>
		/// Gets the command for the OK button.
		/// </summary>
		public ICommand OkCommand { get; }

		/// <summary>
		/// Gets the command for the Cancel button.
		/// </summary>
		public ICommand CancelCommand { get; }

		/// <summary>
		/// Event raised when the dialog should be closed.
		/// </summary>
		public event EventHandler<DialogCloseRequestedEventArgs>? CloseRequested;

		/// <summary>
		/// Gets the model with the current data.
		/// Only call this when user clicks OK - it updates the model from current JSON.
		/// </summary>
		public async Task<IomGeneratorModel> GetResultModelAsync()
		{
			await _iomEditorViewModel.GenerateIomCommand.ExecuteAsync(null);
			return _iomEditorViewModel.Model;
		}

		private bool CanExecuteOk()
		{
			return true;
		}

		private void ExecuteOk()
		{
			// Request dialog close with OK result
			CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));

		}

		private void ExecuteCancel()
		{
			// Request dialog close with Cancel result
			CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
		}
	}

	/// <summary>
	/// Event args for dialog close request.
	/// </summary>
	public class DialogCloseRequestedEventArgs : EventArgs
	{
		public DialogCloseRequestedEventArgs(bool dialogResult)
		{
			DialogResult = dialogResult;
		}

		/// <summary>
		/// Gets the dialog result (true for OK, false for Cancel).
		/// </summary>
		public bool DialogResult { get; }
	}
}
