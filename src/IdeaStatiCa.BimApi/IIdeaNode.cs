
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represents named and identifiable node that several members can connect to.
	/// </summary>
	public interface IIdeaNode : IIdeaObject {

		/// <summary>
		/// X-axis coordinate of the node position
		/// </summary>
		double X { get; }

		/// <summary>
		/// Y-axis coordinate of the node position
		/// </summary>
		double Y { get; }

		/// <summary>
		/// Z-axis coordinate of the node position
		/// </summary>
		double Z { get; }

		/// <summary>
		/// Returns the list of members that are connected to this node (either by their start or end)
		/// @return returns the set of the members. It never returns a null
		/// <para>
		/// The following is guaranteed:
		/// <list type="bullet">
		///		<item>Any member returned, links to this node by either <see cref="IIdeaMember1D.StartNode"/> or <see cref="IIdeaMember1D.EndNode"/> property.</item>
		///		<item>Returned value is never null.</item>
		/// </list>
		/// </para>
		/// </summary>
		HashSet<IIdeaMember1D> GetConnectedMembers();

	}
}