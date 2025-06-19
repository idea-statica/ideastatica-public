using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class LoadOnSurfaceImporter : AbstractImporter<IIdeaLoadOnSurface>
	{
		public LoadOnSurfaceImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaLoadOnSurface los)
		{
			LoadOnSurface loadOnSurface = new LoadOnSurface()
			{
				Direction = los.Direction,
				Fx = los.Fx,
				Fy = los.Fy,
				Fz = los.Fz,								
			};

			ReferenceElement refElement = ctx.Import(los.ReferencedGeometry);
			loadOnSurface.ReferencedGeometry = refElement;

			return loadOnSurface;
		}
	}
}
