using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using IdeaRS.OpenModel.Geometry3D;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class PolyLine3DImporter : AbstractImporter<IIdeaPolyLine3D>
	{
		public PolyLine3DImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaPolyLine3D polyLine3D)
		{
			return new PolyLine3D()
			{
				Segments = polyLine3D.Segments.ConvertAll(s => ctx.Import(s))
			};
		}
	}
}
