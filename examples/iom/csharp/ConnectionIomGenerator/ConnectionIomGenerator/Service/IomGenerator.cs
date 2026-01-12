using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;

namespace ConnectionIomGenerator.Service
{
	public class IomGenerator : IIomGenerator
	{
		private readonly IPluginLogger _logger;

		public IomGenerator(IPluginLogger logger)
		{
			_logger = logger;
		}

		public async Task<OpenModelContainer> GenerateIomAsync(ConnectionInput input)
		{
			_logger.LogInformation("Starting IOM generation");

			var res = new OpenModelContainer();

			try
			{
				// Generate FEA model
				_logger.LogDebug("Generating FEA model");
				var generator = new FeaGenerator();
				var feaModel = generator.Generate(input);

				// Wrap FeaModel to implement IIdeaModel
				_logger.LogDebug("Creating FEA model wrapper");
				var feaModelWrapper = new FeaModelWrapper(feaModel);

				// Create a simple in-memory project for ID mapping
				var project = new InMemoryProject();

				// Create BimImporter
				_logger.LogDebug("Creating BIM importer");
				var bimImporter = BimImporter.Create(
					ideaModel: feaModelWrapper,
					project: project,
					logger: _logger,
					geometryProvider: null, // Will use DefaultGeometryProvider
					configuration: new BimImporterConfiguration()
				);

				// Import connections and populate OpenModel
				_logger.LogDebug("Importing connections");
				var modelBim = bimImporter.ImportConnections(CountryCode.ECEN);

				// Assign the imported OpenModel to the container
				res.OpenModel = modelBim.Model;

				_logger.LogInformation($"IOM generation completed successfully with {res.OpenModel?.Member1D?.Count ?? 0} members");
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to generate IOM", ex);
				throw;
			}

			return await Task.FromResult(res);
		}
	}
}
