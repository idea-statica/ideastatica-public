using IdeaStatica.BimApiLink.BimApi;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using System;

namespace BimApiExample.Plugin.Importers
{
	internal class NodeImporter : IntIdentifierImporter<IIdeaNode>
	{
		public NodeImporter(/*TODO pass API using DI*/)
		{
		}

		public override IIdeaNode Create(int id)
		{
			var v = GetLocation(id);
			return new IdeaNode(id)
			{
				Vector = v,
			};
		}

		private static IdeaVector3D GetLocation(int id)
		{
			return id switch
			{
				1 => new IdeaVector3D(0, 0, 0),
				2 => new IdeaVector3D(0, 0, 3),
				3 => new IdeaVector3D(5, 0, 3),
				_ => throw new NotImplementedException(),
			};
		}
	}
}