using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.ImportedObjects
{
	internal class Connection : IIdeaObject
	{
		public string Id => "$connection-" + Node.Name;

		public string Name => Node.Name;

		public IIdeaNode Node { get; }

		public ISet<IIdeaMember1D> Members { get; }

		public Connection(IIdeaNode node, ISet<IIdeaMember1D> members)
		{
			Node = node;
			Members = members;
		}
	}
}