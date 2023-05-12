using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class LoadCaseImporter : AbstractImporter<IIdeaLoadCase>
	{
		public LoadCaseImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaLoadCase lc)
		{
			LoadCase lcRet = new LoadCase()
			{
				Name = lc.Name,
				Description = lc.Description,
				LoadType = lc.LoadType,
				Type = lc.Type,
				Variable = lc.Variable,
				LoadGroup = ctx.Import(lc.LoadGroup),
				LoadsOnLine = lc.LoadsOnLine.Select(l => ctx.Import(l)).ToList(),
				PointLoadsOnLine = lc.PointLoadsOnLine.Select(l => ctx.Import(l)).ToList()
			};

			return lcRet;
		}
	}
}
