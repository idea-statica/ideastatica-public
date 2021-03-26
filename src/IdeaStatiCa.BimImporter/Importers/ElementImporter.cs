using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class ElementImporter : AbstractImporter<IIdeaElement1D>
	{
		private readonly IImporter<IIdeaCrossSection> _cssImporter;
		private readonly IImporter<IIdeaSegment3D> _segmentImporter;

		public ElementImporter(IImporter<IIdeaCrossSection> cssImporter, IImporter<IIdeaSegment3D> segmentImporter)
		{
			_cssImporter = cssImporter;
			_segmentImporter = segmentImporter;
		}

		protected override OpenElementId ImportInternal(ImportContext ctx, IIdeaElement1D element)
		{
			Element1D iomElement = new Element1D
			{
				Name = element.Name,
				CrossSectionBegin = _cssImporter.Import(ctx, element.StartCrossSection),
				CrossSectionEnd = _cssImporter.Import(ctx, element.EndCrossSection),
				Segment = _segmentImporter.Import(ctx, element.Segment),
				EccentricityBeginX = element.EccentricityBegin.X,
				EccentricityBeginY = element.EccentricityBegin.Y,
				EccentricityBeginZ = element.EccentricityBegin.Z,
				EccentricityEndX = element.EccentricityEnd.X,
				EccentricityEndY = element.EccentricityEnd.Y,
				EccentricityEndZ = element.EccentricityEnd.Z,
				RotationRx = element.RotationRx
			};

			return iomElement;
		}
	}
}