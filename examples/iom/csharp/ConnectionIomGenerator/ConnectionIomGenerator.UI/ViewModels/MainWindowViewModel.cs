using CommunityToolkit.Mvvm.Input;
using ConnectionIomGenerator.Model;
using ConnectionIomGenerator.UI.Services;
using ConnectionIomGenerator.UI.Tools;
using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;
using System;
using System.Threading.Tasks;

namespace ConnectionIomGenerator.UI.ViewModels
{
	/// <summary>
	/// Main window view model for IOM generation
	/// </summary>
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly IPluginLogger _logger;
		private readonly IIomService _iomService;
		private readonly IFileDialogService _fileDialogService;
		
		public MainWindowViewModel(
			IPluginLogger logger,
			IIomService iomService,
			IFileDialogService fileDialogService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_iomService = iomService ?? throw new ArgumentNullException(nameof(iomService));
			_fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));

			GenerateIomCommand = new AsyncRelayCommand(GenerateIomAsync, CanGenerateIomAsync);
			SaveIomCommand = new AsyncRelayCommand(SaveIomAsync, CanSaveIomAsync);

			ConnectionDefinitionJson = JsonTools.GetJsonText<ConnectionInput>(ConnectionInput.GetDefaultECEN());
		}

		public AsyncRelayCommand GenerateIomCommand { get; }
		public AsyncRelayCommand SaveIomCommand { get; }

		private bool CanGenerateIomAsync()
		{
			return !string.IsNullOrEmpty(ConnectionDefinitionJson);
		}

		private bool CanSaveIomAsync()
		{
			return IomContainer != null;
		}

		private async Task GenerateIomAsync()
		{
			try
			{
				var input = DeserializeInput();
				if (input == null) return;

				IomContainer = await _iomService.GenerateIomAsync(input);

				if (IomContainer?.OpenModel != null)
				{
					IomXml = _iomService.SerializeToXml(IomContainer.OpenModel);
					_logger.LogInformation("IOM generated and serialized successfully");
					Status = $"IOM generated with {IomContainer.OpenModel.Member1D?.Count ?? 0} members";
				}
				else
				{
					_logger.LogWarning("Generated IOM or OpenModel is null");
					IomXml = null;
					Status = "Generation failed";
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to generate IOM", ex);
				Status = $"Error: {ex.Message}";
				MessageText = ex.ToString();
			}
		}

		private async Task SaveIomAsync()
		{
			if (IomContainer == null)
			{
				_logger.LogWarning("SaveIomAsync: IomContainer is null");
				return;
			}

			try
			{
				var filePath = _fileDialogService.ShowSaveFileDialog(
					filter: "XML files (*.xml)|*.xml|All files (*.*)|*.*",
					defaultExt: "xml",
					defaultFileName: "Connection.xml",
					title: "Save IOM to XML file");

				if (string.IsNullOrEmpty(filePath))
				{
					_logger.LogDebug("SaveIomAsync: User cancelled save operation");
					return;
				}

				_logger.LogInformation($"Saving IOM to file: {filePath}");
				await _iomService.SaveToFileAsync(IomContainer, filePath);
				
				_logger.LogInformation($"IOM saved successfully to: {filePath}");
				Status = $"IOM saved to: {filePath}";
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to save IOM to file", ex);
				Status = $"Error saving IOM: {ex.Message}";
				MessageText = ex.ToString();
			}
		}

		private ConnectionInput? DeserializeInput()
		{
			if (string.IsNullOrEmpty(ConnectionDefinitionJson))
			{
				_logger.LogWarning("No input data provided");
				Status = "No input data";
				return null;
			}

			var input = JsonTools.DeserializeJson<ConnectionInput>(ConnectionDefinitionJson);
			if (input == null)
			{
				_logger.LogWarning("Failed to deserialize ConnectionInput");
				Status = "Invalid input format";
			}

			return input;
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
					SaveIomCommand.NotifyCanExecuteChanged();
				}
			}
		}
	}
}
