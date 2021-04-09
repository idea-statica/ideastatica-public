using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class ElementImporter : AbstractImporter<IIdeaElement1D>
	{
		protected override OpenElementId ImportInternal(ImportContext ctx, IIdeaElement1D element)
		{
			Element1D iomElement = new Element1D
			{
				Name = element.Name,
				CrossSectionBegin = ctx.Import(element.StartCrossSection),
				CrossSectionEnd = ctx.Import(element.EndCrossSection),
				Segment = ctx.Import(element.Segment),
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