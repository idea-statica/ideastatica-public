using IdeaRstabPlugin.Factories;
using IdeaRstabPlugin.Providers;
using IdeaRS.OpenModel.Loading;

namespace IdeaRstabPlugin.BimApi
{
	internal class RstabLoadCaseNonLinear : RstabLoadCaseBase
	{
		public RstabLoadCaseNonLinear(IObjectFactory objectFactory, ILoadsProvider loadsProvider, int no)
		{
			LoadType = LoadCaseType.Nonlinear;
			Type = LoadCaseSubType.PermanentStandard;

			Dlubal.RSTAB8.LoadCombination data = loadsProvider.GetLoadCombination(no);

			LoadGroup = objectFactory.GetLoadGroup(GetLoadGroupInx(false));

			Id = $"loadcase-nl-{data.Loading.No}";
			Name = $"LC NL {data.Loading.No} {data.Description}";
			Description = data.Definition;
		}
	}
}