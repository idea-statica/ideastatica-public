using CadExampleApi;
using System.Collections.Generic;

namespace BimApiLinkCadExample.BulkSelection
{
	/// <summary>
	/// A joint found by <see cref="JointFinder"/>: where it is, the members meeting there, and the plates and bolt
	/// grids around them.
	/// </summary>
	internal class FoundJoint
	{
		public FoundJoint(CadPoint3D location, IReadOnlyList<CadMember> members, IReadOnlyList<CadObject> connectionObjects)
		{
			Location = location;
			Members = members;
			ConnectionObjects = connectionObjects;
		}

		public CadPoint3D Location { get; }

		public IReadOnlyList<CadMember> Members { get; }

		public IReadOnlyList<CadObject> ConnectionObjects { get; }
	}
}
