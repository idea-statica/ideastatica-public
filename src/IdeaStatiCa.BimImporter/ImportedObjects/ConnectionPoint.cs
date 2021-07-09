using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.ImportedObjects
{
	internal class ConnectionPoint : IIdeaObject
	{
		public string Id => "$connection-" + Node.Id;

		public string Name => Node.Name;

		public IIdeaNode Node { get; }

		public IEnumerable<IIdeaMember1D> Members { get; }

		public ConnectionPoint(IIdeaNode node, IEnumerable<IIdeaMember1D> members)
		{
			Node = node;
			Members = members;
		}
	}
}