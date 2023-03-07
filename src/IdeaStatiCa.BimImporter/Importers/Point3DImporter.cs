using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class Point3DImporter : AbstractImporter<IIdeaPoint3D>
	{
		public Point3DImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaPoint3D point3D)
		{
			return new Point3D()
			{
				Name= point3D.Name,
				X = point3D.X,
				Y = point3D.Y,
				Z = point3D.Z
			};
		}
	}
}
