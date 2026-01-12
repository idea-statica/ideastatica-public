using CommunityToolkit.Mvvm.Input;
using ConnectionIomGenerator.Model;
using ConnectionIomGenerator.Service;
using ConnectionIomGenerator.UI.Tools;
using IdeaStatiCa.Plugin;
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

			ConnectionDefinitionJson = JsonTools.GetJsonText<ConnectionInput>(ConnectionInput.GetDefaultECEN());
		}

		public AsyncRelayCommand GenerateIomCommand { get; }

		private bool CanGenerateIomAsync()
		{
			return true;
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
			var iom = await generator.GenerateIomAsync(input);

			// Serialize OpenModel to formatted XML
			if (iom?.OpenModel != null)
			{
				IomXml = IdeaRS.OpenModel.Tools.SerializeModel(iom.OpenModel);
				_logger.LogInformation("IOM serialized to XML successfully");
			}
			else
			{
				_logger.LogWarning("Generated IOM or OpenModel is null");
				IomXml = null;
			}

			_logger.LogInformation($"IOM generated successfully");
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
	}
}
