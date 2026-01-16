using Castle.Core.Internal;
using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using System.Text.RegularExpressions;

namespace ConnectionIomGenerator.Service
{
	public class IomGenerator : IIomGenerator
	{
		private readonly IPluginLogger _logger;

		public IomGenerator(IPluginLogger logger)
		{
			_logger = logger;
		}

		public async Task<OpenModelContainer> GenerateIomAsync(ConnectionInput input, LoadingInput? loadingInput)
		{
			_logger.LogInformation("Starting IOM generation");

			var res = new OpenModelContainer();

			try
			{
				// Generate FEA model
				_logger.LogDebug("Generating FEA model");
				var generator = new FeaGenerator();
				var feaModel = generator.Generate(input, loadingInput);

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

				// add loading if exists
				if(loadingInput != null)
				{
					IdeaRS.OpenModel.Result.OpenModelResult openModelResult = new IdeaRS.OpenModel.Result.OpenModelResult();

					///openModelResult.
					var resOnMembers = new IdeaRS.OpenModel.Result.ResultOnMembers();
					
					foreach (var lc in res.OpenModel.LoadCase)
					{
						resOnMembers.Loading = new IdeaRS.OpenModel.Result.Loading(IdeaRS.OpenModel.Result.LoadingType.LoadCase, lc.Id);


						foreach (var mem1D in res.OpenModel.Member1D)
						{
							var memBimId = project.GetBimApiId(mem1D.Id);
							int feaMemMemId = ParseBimId(memBimId);
							int elInx = 0;
							input.Members.First(m => m.Id == feaMemMemId);
							foreach (var refEl in mem1D.Elements1D)
							{
								var element1D = refEl.Element;
								var elBimId = project.GetBimApiId(refEl.Id);

								int feaElMemId = ParseBimId(elBimId);

								var resOnMember = new ResultOnMember();
								resOnMember.ResultType = ResultType.InternalForces;
								resOnMember.Member = new IdeaRS.OpenModel.Result.Member(MemberType.Element1D, element1D.Id);

								ResultOfInternalForces intForce1 = new ResultOfInternalForces() { N = 1}; // Do not generate a zero impulse; it will be omitted when the connection is created.
								List<SectionResultBase> forcesList = new List<SectionResultBase>() { intForce1 };

								ResultOnSection resInSection0 = new ResultOnSection() { AbsoluteRelative = AbsoluteRelative.Relative, Position = 0, Results = forcesList };
								ResultOnSection resInSection1 = new ResultOnSection() { AbsoluteRelative = AbsoluteRelative.Relative, Position = 1, Results = forcesList };
								//forcesList.Add(resInSection0);
								//resOnMember.Results = new ResultOnSection
								resOnMember.Results.Add(resInSection0);
								resOnMember.Results.Add(resInSection1);
								resOnMembers.Members.Add(resOnMember);
								elInx++;
							}
						}
					}

							//resOnMember.
						

					openModelResult.ResultOnMembers.Add(resOnMembers);
					res.OpenModelResult = openModelResult;
				}

				_logger.LogInformation($"IOM generation completed successfully with {res.OpenModel?.Member1D?.Count ?? 0} members");
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to generate IOM", ex);
				throw;
			}

			return await Task.FromResult(res);
		}

		static int ParseBimId(string bimApiId)
		{
			int inx = bimApiId.IndexOf("-");
			string parsedId = bimApiId.Substring(inx+1, bimApiId.Length - inx - 1);
			return Int32.Parse(parsedId);
		}
	}
}
