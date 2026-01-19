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
		private readonly IIomGenerator _iomGenerator;

		public IomService(IIomGenerator iomGenerator, IPluginLogger logger)
		{
			_iomGenerator = iomGenerator;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<OpenModelContainer> GenerateIomAsync(ConnectionInput input, LoadingInput? loadingInput)
		{
			return await _iomGenerator.GenerateIomAsync(input, loadingInput);
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