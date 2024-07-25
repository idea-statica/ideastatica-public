using BimApiLinkFeaExample.FeaExampleApi;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiLinkFeaExample.Importers
{
	internal class LoadCaseImporter : IntIdentifierImporter<IIdeaLoadCase>
	{
		private readonly IFeaLoadsApi loadsApi;

		public LoadCaseImporter(IFeaLoadsApi loadsApi)
		{
			this.loadsApi = loadsApi;
		}

		public override IIdeaLoadCase Create(int id)
		{
			IFeaLoadCase loadCase = loadsApi.GetLoadCase(id);
			var loadType = GetLoadType(loadCase.LoadCaseType);
			return new IdeaLoadCase(id)
			{
				Name = loadCase.Name,
				LoadGroup = Get<IIdeaLoadGroup>(loadCase.LoadGroupId),
				LoadType = loadType.Item1,				
				Type = loadType.Item2,
				Variable = VariableType.Standard
			};
		}

		private static (LoadCaseType, LoadCaseSubType) GetLoadType(TypeOfLoadCase typeOfLoadCase) 
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
					return (LoadCaseType.Permanent, LoadCaseSubType.PermanentStandard);
			}
		}
	}
}
