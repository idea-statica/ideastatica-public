using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class ConcreteBlockImporter : AbstractImporter<IIdeaConcreteBlock>
	{
		public ConcreteBlockImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaConcreteBlock concreteBlock, ConnectionData connectionData)
		{
			var concreteBlockIOM = new ConcreteBlockData()
			{
				Id = 0,
				Height = concreteBlock.Height,
				Length = concreteBlock.Lenght,
				Material = concreteBlock.Material == null ? null : ctx.Import(concreteBlock.Material),
				Width = concreteBlock.Width,
			};

			(connectionData.ConcreteBlocks ?? (connectionData.ConcreteBlocks = new List<ConcreteBlockData>())).Add(concreteBlockIOM);

			//set correct Id
			concreteBlockIOM.Id = connectionData.ConcreteBlocks.Max(c => c.Id) + 1;

			return concreteBlockIOM;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaConcreteBlock concreteBlock)
		{
			throw new System.NotImplementedException();
		}
	}
}