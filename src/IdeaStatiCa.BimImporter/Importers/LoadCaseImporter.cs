using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class LoadCaseImporter : AbstractImporter<IIdeaLoadCase>
	{
		public LoadCaseImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaLoadCase lc)
		{
			List<ReferenceElement> loadsOnSurface = new List<ReferenceElement>();

			if (lc.LoadsOnSurface != null)
			{
				foreach (var los in lc.LoadsOnSurface)
				{
					ReferenceElement refLoadOnSurface = ctx.Import(los);
					loadsOnSurface.Add(refLoadOnSurface);
				}
			}

			LoadCase lcRet = new LoadCase()
			{
				Name = lc.Name,
				Description = lc.Description,
				LoadType = lc.LoadType,
				Type = lc.Type,
				Variable = lc.Variable,
				LoadsOnSurface = loadsOnSurface
			};

			ReferenceElement refElement = ctx.Import(lc.LoadGroup);
			lcRet.LoadGroup = refElement;

			return lcRet;
		}
	}
}
