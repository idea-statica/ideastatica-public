using ConnectionIomGenerator.Model;
using ConnectionIomGenerator.Service;
using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;
using System;
using System.Threading.Tasks;

namespace ConnectionIomGenerator.UI.Services
{
	/// <summary>
	/// Implementation of IOM service
	/// </summary>
	public class IomService : IIomService
	{
		private readonly IPluginLogger _logger;

		public IomService(IPluginLogger logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<OpenModelContainer> GenerateIomAsync(ConnectionInput input, LoadingInput? loadingInput)
		{
			var generator = new IomGenerator(_logger);
			return await generator.GenerateIomAsync(input, loadingInput);
		}

		public string SerializeToXml(OpenModel openModel)
		{
			return IdeaRS.OpenModel.Tools.SerializeModel(openModel);
		}

		public async Task SaveToFileAsync(OpenModelContainer container, string filePath)
		{
			await Task.Run(() => IdeaRS.OpenModel.Tools.OpenModelContainerToFile(container, filePath));
		}
	}
}