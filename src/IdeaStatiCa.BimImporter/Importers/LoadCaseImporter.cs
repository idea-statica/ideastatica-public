using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

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
			};

			return lcRet;
		}
	}
}
