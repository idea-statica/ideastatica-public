using CommunityToolkit.Mvvm.Input;
using ConnectionIomGenerator.Model;
using ConnectionIomGenerator.Service;
using ConnectionIomGenerator.UI.Tools;
using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;

namespace ConnectionIomGenerator.UI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly IPluginLogger _logger;
		
		public MainWindowViewModel(IPluginLogger pluginLogger)
		{
			this._logger = pluginLogger;
			GenerateIomCommand = new AsyncRelayCommand(GenerateIomAsync, CanGenerateIomAsync);
			SaveIomCommand = new AsyncRelayCommand(SaveIomAsync, CanSaveIomAsync);

			ConnectionDefinitionJson = JsonTools.GetJsonText<ConnectionInput>(ConnectionInput.GetDefaultECEN());
		}

		public AsyncRelayCommand GenerateIomCommand { get; }
		public AsyncRelayCommand SaveIomCommand { get; }

		private bool CanGenerateIomAsync()
		{
			return true;
		}

		private bool CanSaveIomAsync()
		{
			return IomContainer != null;
		}

		private async Task GenerateIomAsync()
		{
			// get input
			if(string.IsNullOrEmpty(ConnectionDefinitionJson))
			{
				_logger.LogWarning("GenerateIomAsync : Leaving - no input data");
				return;
			}

			var input = JsonTools.DeserializeJson<ConnectionInput>(ConnectionDefinitionJson);

			if (input == null)
			{
				_logger.LogWarning("GenerateIomAsync : Leaving - null instance of ConnectionInput");
				return;
			}

			// Pass logger to IomGenerator
			var generator = new IomGenerator(_logger);
			IomContainer = await generator.GenerateIomAsync(input);

			// Serialize OpenModel to formatted XML
			if (IomContainer?.OpenModel != null)
			{
				IomXml = IdeaRS.OpenModel.Tools.SerializeModel(IomContainer.OpenModel);
				_logger.LogInformation("IOM serialized to XML successfully");
			}
			else
			{
				_logger.LogWarning("Generated IOM or OpenModel is null");
				IomXml = null;
			}

			_logger.LogInformation($"IOM generated successfully");
		}

		private async Task SaveIomAsync()
		{
			if (IomContainer == null)
			{
				_logger.LogWarning("SaveIomAsync : IomContainer is null");
				return;
			}

			// Show save file dialog
			var saveFileDialog = new SaveFileDialog
			{
				Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
				DefaultExt = "xml",
				FileName = "Connection.xml",
				Title = "Save IOM to XML file"
			};

			if (saveFileDialog.ShowDialog() == true)
			{
				try
				{
					_logger.LogInformation($"Saving IOM to file: {saveFileDialog.FileName}");
					
					// Save OpenModelContainer to XML file
					await Task.Run(() => IdeaRS.OpenModel.Tools.OpenModelContainerToFile(IomContainer, saveFileDialog.FileName));
					
					_logger.LogInformation($"IOM saved successfully to: {saveFileDialog.FileName}");
					Status = $"IOM saved to: {saveFileDialog.FileName}";
				}
				catch (Exception ex)
				{
					_logger.LogError($"Failed to save IOM to file: {saveFileDialog.FileName}", ex);
					Status = $"Error saving IOM: {ex.Message}";
				}
			}
			else
			{
				_logger.LogDebug("SaveIomAsync : User cancelled save operation");
			}
		}

		private string? connectionDefinitionJson;
		public string? ConnectionDefinitionJson
		{
			get => connectionDefinitionJson;
			set => SetProperty(ref connectionDefinitionJson, value);
		}

		private string? iomXml;
		public string? IomXml
		{
			get => iomXml;
			set => SetProperty(ref iomXml, value);
		}

		private string? messageText;
		public string? MessageText
		{
			get => messageText;
			set => SetProperty(ref messageText, value);
		}

		private string? status;
		public string? Status
		{
			get => status;
			set => SetProperty(ref status, value);
		}

		private OpenModelContainer? iomContainer;
		public OpenModelContainer? IomContainer
		{
			get => iomContainer;
			set
			{
				if (SetProperty(ref iomContainer, value))
				{
					// Notify that CanExecute for SaveIomCommand has changed
					SaveIomCommand.NotifyCanExecuteChanged();
				}
			}
		}
	}
}
