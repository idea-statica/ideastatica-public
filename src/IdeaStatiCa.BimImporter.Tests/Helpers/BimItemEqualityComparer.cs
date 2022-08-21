using FluentAssertions;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.ImportedObjects;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Tests.Helpers
{
	internal class BimItemEqualityComparer : IEqualityComparer<IBimItem>
	{
		private static readonly IIdeaObjectComparer _ideaObjectComparer = new IIdeaObjectComparer();

		public bool Equals(IBimItem x, IBimItem y)
		{
			if (x.Type != y.Type)
			{
				return false;
			}

			if (x is Connection connA && y is Connection connB)
			{
				return ConnectionsEqual(connA, connB);
			}

			if (x is Member memberA && y is Member memberB)
			{
				return MembersEqual(memberA, memberB);
			}

			return false;
		}

		public int GetHashCode(IBimItem obj)
		{
			return obj.ReferencedObject.GetHashCode() + obj.Type.GetHashCode() * 23;
		}

		private bool ConnectionsEqual(Connection a, Connection b)
		{
			ConnectionPoint connA = (ConnectionPoint)a.ReferencedObject;
			ConnectionPoint connB = (ConnectionPoint)b.ReferencedObject;

			connA.ConnectedMembers.Should().BeEquivalentTo(connB.ConnectedMembers, options => options
			.AllowingInfiniteRecursion()
			);
			return _ideaObjectComparer.Equals(connA.Node, connB.Node);

		}

		private bool MembersEqual(Member a, Member b)
		{
			IIdeaMember1D memberA = (IIdeaMember1D)a.ReferencedObject;
			IIdeaMember1D memberB = (IIdeaMember1D)b.ReferencedObject;

			return _ideaObjectComparer.Equals(memberA, memberB);
		}
	}
}