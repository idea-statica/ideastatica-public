using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class SpanImporter : AbstractImporter<IIdeaSpan>
	{
		public SpanImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaSpan obj)
		{
			return new Span
			{
				StartCrossSection = ctx.Import(obj.StartCrossSection),
				EndCrossSection = ctx.Import(obj.EndCrossSection ?? obj.StartCrossSection),
				StartPosition = obj.StartPosition,
				EndPosition = obj.EndPosition,
			};
		}
	}
}