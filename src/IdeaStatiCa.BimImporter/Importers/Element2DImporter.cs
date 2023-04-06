using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class Element2DImporter : AbstractImporter<IIdeaElement2D>
	{
		public Element2DImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaElement2D element2D)
		{
			return new Element2D()
			{
				Name = element2D.Name,
				Material = ctx.Import(element2D.Material),
				GeometricRegion = ctx.Import(element2D.GeometricRegion),
				Thickness = element2D.Thickness,
				EccentricityZ = element2D.EccentricityZ,
				InnerLines = element2D.InnerLines.ConvertAll(l => ctx.Import(l)),
				InnerPoints = element2D.InnerPoints.ConvertAll(p => ctx.Import(p)),
				ElementType = element2D.ElementType
			};
		}
	}
}
