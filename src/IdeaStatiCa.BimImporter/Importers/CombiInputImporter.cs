using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class CombiInputImporter : AbstractImporter<IIdeaCombiInput>
	{

		public CombiInputImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaCombiInput com)
		{

			CombiInputEC combi = new CombiInputEC()
			{

			};

			foreach (var item in com.CombiItems)
			{
				combi.Items.Add(new CombiItem() { Coeff = item.Coeff, LoadCase = ctx.Import(item.LoadCase) });
			}
			return combi;
		}
	}
}
