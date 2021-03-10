using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using System;

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

		protected override ReferenceElement ImportInternal(ImportContext ctx, IIdeaElement1D element)
		{
			if (element.StartNode.IsSimilarTo(element.EndNode))
			{
				throw new Exception(); // TODO: text
			}

			Element1D iomElement = new Element1D
			{
				Name = element.Name,
				CrossSectionBegin = _cssImporter.Import(ctx, element.StartCrossSection),
				CrossSectionEnd = _cssImporter.Import(ctx, element.EndCrossSection),
				Segment = _segmentImporter.Import(ctx, element.Segment)
			};

			ctx.Add(iomElement);

			return new ReferenceElement(iomElement);
		}
	}
}