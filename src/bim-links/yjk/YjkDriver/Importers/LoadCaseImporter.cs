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
				case TypeOfLoadCase.Dead:
					return (LoadCaseType.Permanent, LoadCaseSubType.PermanentStandard);
				case TypeOfLoadCase.Live:
					return (LoadCaseType.Variable, LoadCaseSubType.VariableStatic);
				case TypeOfLoadCase.Wind:
					return (LoadCaseType.Variable, LoadCaseSubType.VariableDynamic);
				case TypeOfLoadCase.HorizontalSeismic:
					return (LoadCaseType.Accidental, LoadCaseSubType.VariableDynamic);
				case TypeOfLoadCase.VerticalSeismic:
					return (LoadCaseType.Accidental, LoadCaseSubType.VariableDynamic);
				case TypeOfLoadCase.CivilDefence:
					return (LoadCaseType.Accidental, LoadCaseSubType.VariableStatic);
				case TypeOfLoadCase.Crane:
					return (LoadCaseType.Variable, LoadCaseSubType.VariableStatic);
				case TypeOfLoadCase.Temperature:
					return (LoadCaseType.Variable, LoadCaseSubType.VariableStatic);
				default:
					_logger.LogWarning($"LoadCase id={id}: unrecognised type {typeOfLoadCase}, defaulting to (Permanent, PermanentStandard)");
					return (LoadCaseType.Permanent, LoadCaseSubType.PermanentStandard);
			}
		}
	}
}
