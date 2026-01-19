using CommunityToolkit.Mvvm.Input;
using ConnectionIomGenerator.Model;
using ConnectionIomGenerator.Service;
using ConnectionIomGenerator.UI.Models;
using ConnectionIomGenerator.UI.Services;
using ConnectionIomGenerator.UI.Tools;
using IdeaStatiCa.Plugin;
using System;
using System.Threading.Tasks;

namespace ConnectionIomGenerator.UI.ViewModels
{
	/// <summary>
	/// Main window view model for IOM generation
	/// </summary>
	public class IomGeneratorViewModel : ViewModelBase
	{
		private readonly IPluginLogger _logger;
		private readonly IIomService _iomService;
		private readonly IFileDialogService _fileDialogService;
		private readonly IomGeneratorModel? _model;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="IomGeneratorViewModel"/> class.
		/// </summary>
		/// <remarks>This constructor initializes the commands for generating and saving IOM data, and sets up the
		/// initial state of the view model. If the <paramref name="model"/> contains a non-<see langword="null"/>
		/// <c>ConnectionInput</c>, its JSON representation is assigned to the <c>ConnectionDefinitionJson</c>
		/// property.</remarks>
		/// <param name="model">The data model containing the connection input and other relevant data for the view model.</param>
		/// <param name="logger">The logger instance used for logging application events. This parameter cannot be <see langword="null"/>.</param>
		/// <param name="iomService">The service responsible for handling IOM (Input/Output Model) operations. This parameter cannot be <see
		/// langword="null"/>.</param>
		/// <param name="fileDialogService">The service used for handling file dialog operations. This parameter cannot be <see langword="null"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/>, <paramref name="iomService"/>, or <paramref name="fileDialogService"/> is
		/// <see langword="null"/>.</exception>
		public IomGeneratorViewModel(
			IomGeneratorModel model,
			IPluginLogger logger,
			IIomService iomService,
			IFileDialogService fileDialogService)
		{
			_model = model ?? throw new ArgumentNullException(nameof(model));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_iomService = iomService ?? throw new ArgumentNullException(nameof(iomService));
			_fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));

			GenerateIomCommand = new AsyncRelayCommand(GenerateIomAsync, CanGenerateIomAsync);
			GenerateLoadingCommand = new AsyncRelayCommand(GenerateLoadingAsync, CanGenerateLoadingAsync);
			SaveIomCommand = new AsyncRelayCommand(SaveIomAsync, CanSaveIomAsync);

			if (_model?.ConnectionInput != null)
			{
				ConnectionDefinitionJson = JsonTools.GetJsonText<ConnectionInput>(_model.ConnectionInput);
			}
		}

		public AsyncRelayCommand GenerateIomCommand { get; }
		public AsyncRelayCommand GenerateLoadingCommand { get; }
		public AsyncRelayCommand SaveIomCommand { get; }

		/// <summary>
		/// Gets the underlying model. This is updated when JSON is deserialized.
		/// </summary>
		public IomGeneratorModel Model => _model!;

		private bool CanGenerateIomAsync()
		{
			return !string.IsNullOrEmpty(ConnectionDefinitionJson);
		}

		private bool CanGenerateLoadingAsync()
		{
			return _model!.ConnectionInput != null;
		}

		private bool CanSaveIomAsync()
		{
			return _model!.IomContainer != null;
		}

		private async Task GenerateIomAsync()
		{
			try
			{
				var input = DeserializeInput();
				if (input == null)
				{
					return;
				}


				LoadingInput? loadingInput = null;
				if(!string.IsNullOrEmpty(LoadingDefinitionJson))
				{
					loadingInput = JsonTools.DeserializeJson<LoadingInput>(LoadingDefinitionJson);
				}

				_model!.IomContainer = null;
				_model.ConnectionInput = input;
				_model.IomContainer = await _iomService.GenerateIomAsync(input, loadingInput);

				if (_model.IomContainer?.OpenModel != null)
				{
					IomXml = _iomService.SerializeToXml(_model.IomContainer.OpenModel);
					_logger.LogInformation("IOM generated and serialized successfully");
					Status = $"IOM generated with {_model.IomContainer.OpenModel.Member1D?.Count ?? 0} members";
				}
				else
				{
					_logger.LogWarning("Generated IOM or OpenModel is null");
					IomXml = null;
					Status = "Generation failed";
				}

				SaveIomCommand.NotifyCanExecuteChanged();
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to generate IOM", ex);
				Status = $"Error: {ex.Message}";
				MessageText = ex.ToString();
			}
		}

		private async Task GenerateLoadingAsync()
		{
			var input = DeserializeInput();
			if (input == null)
			{
				return;
			}

			var feaGenerator = new FeaGenerator();
			var loading = feaGenerator.CreateLoadingForConnection(input);

			if(loading == null)
			{
				return;
			}

			var loadingJson = JsonTools.GetJsonText<LoadingInput>(loading);
			this.LoadingDefinitionJson = loadingJson;
			await Task.CompletedTask;
		}

		private async Task SaveIomAsync()
		{
			if (_model!.IomContainer == null)
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
				await _iomService.SaveToFileAsync(_model.IomContainer, filePath);
				
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

		private string? loadingDefinitionJson;
		public string? LoadingDefinitionJson
		{
			get => loadingDefinitionJson;
			set
			{
				if (SetProperty(ref loadingDefinitionJson, value))
				{
					GenerateLoadingCommand.NotifyCanExecuteChanged();
				}
			}
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
	}
}
