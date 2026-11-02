using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
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
				RotationRx = element.RotationRx
			};

			return iomElement;
		}
	}
}