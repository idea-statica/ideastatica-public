using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class Region3DImporter : AbstractImporter<IIdeaRegion3D>
	{
		public Region3DImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaRegion3D region3D)
		{
			return new Region3D()
			{
				Outline = ctx.Import(region3D.Outline),
				Openings = region3D.Openings.ConvertAll(o => ctx.Import(o)),
				LocalCoordinateSystem = region3D.LocalCoordinateSystem,
			};
		}
	}
}
