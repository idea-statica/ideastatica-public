using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class TaperImporter : AbstractImporter<IIdeaTaper>
	{
		public TaperImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaTaper taper)
		{
			return new Taper()
			{
				Spans = taper?.Spans
					.Select(x => ctx.Import(x))
					.ToList() ?? new List<ReferenceElement>()
			};
		}
	}
}