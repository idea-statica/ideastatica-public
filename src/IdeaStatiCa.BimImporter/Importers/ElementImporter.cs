using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Extensions;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class ElementImporter : AbstractImporter<IIdeaElement1D>
	{
		public ElementImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaElement1D element)
		{
			Element1D iomElement = new Element1D
			{
				Name = element.Name,
				Segment = ctx.Import(element.Segment),
				EccentricityBegin = element.EccentricityBegin.ToIOMVector(),
				EccentricityEnd = element.EccentricityEnd.ToIOMVector(),
				CardinalPoint = element.CardinalPoint,
				EccentricityReference = element.EccentricityReference,
				RotationRx = element.RotationRx
			};

			return iomElement;
		}
	}
}