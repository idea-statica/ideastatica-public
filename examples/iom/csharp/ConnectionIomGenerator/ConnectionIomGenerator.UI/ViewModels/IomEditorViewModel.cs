using CommunityToolkit.Mvvm.Input;
using ConnectionIomGenerator.Model;
using ConnectionIomGenerator.UI.Models;
using ConnectionIomGenerator.UI.Services;
using ConnectionIomGenerator.UI.Tools;
using IdeaStatiCa.Plugin;
using System;
using System.Windows.Input;

namespace ConnectionIomGenerator.UI.ViewModels
{
	/// <summary>
	/// ViewModel for the IOM Editor dialog window.
	/// Wraps MainWindowViewModel and adds dialog-specific functionality.
	/// </summary>
	public class IomEditorViewModel : ViewModelBase
	{
		private readonly MainWindowViewModel _mainViewModel;
		private string? _validationMessage;

		/// <summary>
		/// Initializes a new instance of the <see cref="IomEditorViewModel"/> class.
		/// </summary>
		/// <param name="logger">Logger for tracking operations.</param>
		/// <param name="iomService">Service for IOM operations.</param>
		/// <param name="fileDialogService">Service for file dialogs.</param>
		public IomEditorViewModel(
			IPluginLogger logger,
			IIomService iomService,
			IFileDialogService fileDialogService)
		{
			// MainWindowViewModel constructor already calls ConnectionInput.GetDefaultECEN()
			_mainViewModel = new MainWindowViewModel(logger, iomService, fileDialogService);
			
			OkCommand = new RelayCommand(ExecuteOk, CanExecuteOk);
			CancelCommand = new RelayCommand(ExecuteCancel);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IomEditorViewModel"/> class with initial model.
		/// </summary>
		/// <param name="model">Initial model data.</param>
		/// <param name="logger">Logger for tracking operations.</param>
		/// <param name="iomService">Service for IOM operations.</param>
		/// <param name="fileDialogService">Service for file dialogs.</param>
		public IomEditorViewModel(
			IomGeneratorModel model,
			IPluginLogger logger,
			IIomService iomService,
			IFileDialogService fileDialogService)
		{
			if (model == null)
				throw new ArgumentNullException(nameof(model));

			_mainViewModel = new MainWindowViewModel(logger, iomService, fileDialogService);
			
			// Load the initial model data into MainViewModel
			if (model.ConnectionInput != null)
			{
				_mainViewModel.ConnectionDefinitionJson = JsonTools.GetJsonText(model.ConnectionInput);
			}
			
			if (model.Loading != null)
			{
				_mainViewModel.LoadingDefinitionJson = JsonTools.GetJsonText(model.Loading);
			}
			
			OkCommand = new RelayCommand(ExecuteOk, CanExecuteOk);
			CancelCommand = new RelayCommand(ExecuteCancel);
		}

		/// <summary>
		/// Gets the wrapped MainWindowViewModel.
		/// This is bound to the ConnectionInputView DataContext.
		/// </summary>
		public MainWindowViewModel MainViewModel => _mainViewModel;

		/// <summary>
		/// Gets the validation message.
		/// </summary>
		public string? ValidationMessage
		{
			get => _validationMessage;
			set => SetProperty(ref _validationMessage, value);
		}

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
		public IomGeneratorModel GetResultModel()
		{
			// Update the model's ConnectionInput from current JSON
			var connectionInput = JsonTools.DeserializeJson<ConnectionInput>(
				_mainViewModel.ConnectionDefinitionJson ?? string.Empty);
			
			_mainViewModel.Model.ConnectionInput = connectionInput;
			
			// Update the model's Loading from current JSON
			if (!string.IsNullOrEmpty(_mainViewModel.LoadingDefinitionJson))
			{
				var loadingInput = JsonTools.DeserializeJson<LoadingInput>(
					_mainViewModel.LoadingDefinitionJson);
				_mainViewModel.Model.Loading = loadingInput;
			}
			else
			{
				_mainViewModel.Model.Loading = null;
			}

			return _mainViewModel.Model;
		}

		private bool CanExecuteOk()
		{
			return true;
		}

		private void ExecuteOk()
		{
			// Validate the JSON input
			if (string.IsNullOrWhiteSpace(_mainViewModel.ConnectionDefinitionJson))
			{
				ValidationMessage = "Connection definition is required.";
				return;
			}

			// Try to deserialize to validate JSON format
			var connectionInput = JsonTools.DeserializeJson<ConnectionInput>(
				_mainViewModel.ConnectionDefinitionJson);
			
			if (connectionInput == null)
			{
				ValidationMessage = "Invalid connection definition JSON.";
				return;
			}

			// Validate connection input
			if (string.IsNullOrWhiteSpace(connectionInput.Material))
			{
				ValidationMessage = "Material is required.";
				return;
			}

			if (connectionInput.Members == null || connectionInput.Members.Count == 0)
			{
				ValidationMessage = "At least one member is required.";
				return;
			}

			foreach (var member in connectionInput.Members)
			{
				if (string.IsNullOrWhiteSpace(member.Name))
				{
					ValidationMessage = $"Member {member.Id}: Name is required.";
					return;
				}

				if (string.IsNullOrWhiteSpace(member.CrossSection))
				{
					ValidationMessage = $"Member {member.Id}: Cross-section is required.";
					return;
				}
			}

			ValidationMessage = null;

			// Update the model from JSON before closing
			GetResultModel();

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
