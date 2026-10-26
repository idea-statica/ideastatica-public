using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaRstabPlugin.BimApi
{
	internal abstract class RstabLoadCaseBase : IIdeaLoadCase
	{
		public LoadCaseType LoadType { get; set; }

		public LoadCaseSubType Type { get; set; }

		public VariableType Variable { get; } = VariableType.Standard;

		public IIdeaLoadGroup LoadGroup { get; set; }

		public IEnumerable<IIdeaLoadOnSurface> LoadsOnSurface { get; set; }

		public string Description { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		protected int GetLoadGroupInx(bool cyclic)
		{
			if (LoadType == LoadCaseType.Permanent || LoadType == LoadCaseType.Nonlinear)
			{
				return 2;   // "Permanent base"
			}
			if (cyclic)
			{
				return 4;   // "Variable cyclic base"
			}
			else
			{
				return 3;   // "Variable base"
			}
		}
	}
}