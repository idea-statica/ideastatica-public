using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.ImportedObjects;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Tests.Helpers
{
	internal class BímItemEqualityComparer : IEqualityComparer<IBimItem>
	{
		private static readonly IIdeaObjectComparer _ideaObjectComparer = new IIdeaObjectComparer();

		public bool Equals(IBimItem x, IBimItem y)
		{
			if (x is Connection connA && y is Connection connB)
			{
				return ConnectionEqual(connA, connB);
			}

			return false;
		}

		public int GetHashCode(IBimItem obj)
		{
			return obj.ReferencedObject.GetHashCode() + obj.Type.GetHashCode() * 23;
		}

		private bool ConnectionEqual(Connection a, Connection b)
		{
			ConnectionPoint connA = (ConnectionPoint)a.ReferencedObject;
			ConnectionPoint connB = (ConnectionPoint)b.ReferencedObject;

			return _ideaObjectComparer.Equals(connA.Node, connB.Node) &&
				Enumerable.SequenceEqual(connA.Members, connB.Members);
		}
	}
}