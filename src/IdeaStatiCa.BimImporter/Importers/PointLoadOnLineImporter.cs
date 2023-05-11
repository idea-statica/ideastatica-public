using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class PointLoadOnLineImporter : AbstractImporter<IIdeaPointLoadOnLine>
	{
		public PointLoadOnLineImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaPointLoadOnLine pointLoadOnLine)
		{
			return new PointLoadOnLine()
			{
				Direction = pointLoadOnLine.Direction,
				Fx = pointLoadOnLine.Fx,
				Fy = pointLoadOnLine.Fy,
				Fz = pointLoadOnLine.Fz,
				Mx = pointLoadOnLine.Mx,
				My = pointLoadOnLine.My,
				Mz = pointLoadOnLine.Mz,
				Ey = pointLoadOnLine.Ey,
				Ez = pointLoadOnLine.Ez,
				Geometry = ctx.Import(pointLoadOnLine.Geometry),
				RelativePosition = pointLoadOnLine.RelativePosition
			};
		}
	}
}
