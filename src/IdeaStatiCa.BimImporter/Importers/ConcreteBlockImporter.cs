using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class ConcreteBlockImporter : AbstractImporter<IIdeaConcreteBlock>
	{
		public ConcreteBlockImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaConcreteBlock concreteBlock, ConnectionData connectionData)
		{

			var concreteBlockIOM = new ConcreteBlock()
			{
				Height = concreteBlock.Height,
				Lenght = concreteBlock.Lenght,
				Material = concreteBlock.Material.Name,
				Width = concreteBlock.Width,
			};

			return concreteBlockIOM;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaConcreteBlock concreteBlock)
		{
			throw new System.NotImplementedException();
		}
	}
}