using yjk.FeaApis;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.Plugin;
using yjk.ViewModels;

namespace yjk.Importers
{
	internal class LoadCaseImporter : IntIdentifierImporter<IIdeaLoadCase>
	{
		private readonly IFeaLoadsApi loadsApi;
		private readonly IPluginLogger _logger = AppLogger.Instance;

		public LoadCaseImporter(IFeaLoadsApi loadsApi)
		{
			this.loadsApi = loadsApi;
		}

		public override IIdeaLoadCase Create(int id)
		{
			IFeaLoadCase loadCase = loadsApi.GetLoadCase(id);
			var loadType = GetLoadType(loadCase.LoadCaseType, id);
			_logger.LogInformation($"LoadCase created: id={id}, name={loadCase.Name}, type={loadType.Item1}, subType={loadType.Item2}");
			return new IdeaLoadCase(id)
			{
				Name = loadCase.Name,
				LoadGroup = Get<IIdeaLoadGroup>(loadCase.LoadGroupId),
				LoadType = loadType.Item1,
				Type = loadType.Item2,
				Variable = VariableType.Standard
			};
		}

		private (LoadCaseType, LoadCaseSubType) GetLoadType(TypeOfLoadCase typeOfLoadCase, int id)
		{
			switch (typeOfLoadCase)
			{
				case TypeOfLoadCase.Selfweight:
					return (LoadCaseType.Permanent, LoadCaseSubType.PermanentSelfweight);
				case TypeOfLoadCase.DeadLoad:
					return (LoadCaseType.Permanent, LoadCaseSubType.PermanentStandard);
				case TypeOfLoadCase.Snow:
					return (LoadCaseType.Variable, LoadCaseSubType.VariableStatic);
				default:
					_logger.LogWarning($"LoadCase id={id}: unrecognised type {typeOfLoadCase}, defaulting to (Permanent, PermanentStandard)");
					return (LoadCaseType.Permanent, LoadCaseSubType.PermanentStandard);
			}
		}
	}
}
