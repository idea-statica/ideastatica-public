
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represents named and identifiable generic member of that connects to up to two end nodes and might go through other nodes.
	/// <para>
	/// Member is used to represent beam, column, truss, rib or beamslab
	/// </para>
	/// <para>
	/// Members consist from one or more elements, each defining cros-section and materials.</para>
	/// </summary>
	public interface IIdeaMember1D : IIdeaObject
	{
		/// <summary>
		/// Type of member: beam, column, truss, rib or beamslab.
		/// </summary>
		IdeaRS.OpenModel.Model.Member1DType Type { get; }

		/// <summary>
		/// Elements this member consists of. Always returns a valid list with at least one element.
		/// <para>
		/// The following guarantees are valid for the elements:
		/// <list type="bullet">
		///		<item>There is always at least one element in each member.</item>
		///		<item>First element's <see cref="IIdeaElement1D.StartNode"/> is linked to the same node as <see cref="StartNode"/>.</item>
		///		<item>N-th element's <see cref="IIdeaElement1D.EndNode"/> is linked to the same node as (n+1)-th element's <see cref="IIdeaElement1D.StartNode"/>.</item>
		///		<item>Last element's <see cref="IIdeaElement1D.EndNode"/> is linked to the same node as <see cref="EndNode"/>.</item>
		/// </list>
		/// </para>
		/// </summary>
		HashSet<IIdeaElement1D> Elements { get; }

		/// <summary>
		/// Identifies the start node of the member or null, if the member's start is not connected to any node.
		/// <para>
		/// Following guarantees are valid for the returned node:
		/// <list type="bullet">
		///		<item>It is not equal to the <see cref="EndNode"/>.</item>
		///		<item>The node returned from this property is guaranteed to list this member in the set returned from <see cref="IIdeaNode.GetConnectedMembers"/> call</item>
		///		<item>The first of the elements is connected to the same node via <see cref="IIdeaElement1D.StartNode"/>.</item>
		/// </list>
		/// </para>
		/// </summary>
		IIdeaNode StartNode { get; }

		/// <summary>
		/// Identifies the end node of the member or null, if the member's end is not connected to any node.
		/// <para>
		/// Following guarantees are valid for the returned node:
		/// <list type="bullet">
		///		<item>It is not equal to the <see cref="StartNode"/>.</item>
		///		<item>The node returned from this property is guaranteed to list this member in the set returned from <see cref="IIdeaNode.GetConnectedMembers"/> call</item>
		///		<item>The last of the elements is connected to the same node via <see cref="IIdeaElement1D.EndNode"/>.</item>
		/// </list>
		/// </summary>
		IIdeaNode EndNode { get; }

		// IdeaReleases startReleases;

		// IdeaReleases endReleases;

		// void midPoint;

	}
}