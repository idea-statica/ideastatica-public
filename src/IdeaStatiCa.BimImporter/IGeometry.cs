using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// Provides a way to search members by nodes and nodes by members.
	/// </summary>
	public interface IGeometry
	{
		/// <summary>
		/// Returns all member connected to the <paramref name="node"/>.
		/// </summary>
		/// <param name="node">Node to get members for.</param>
		/// <returns>Connected members.</returns>
		/// <exception cref="ArgumentException">If the <paramref name="node"/> is not known.</exception>
		IEnumerable<IIdeaMember1D> GetConnectedMembers(IIdeaNode node);

		/// <summary>
		/// Returns all nodes that the <paramref name="member"/> is connected to.
		/// </summary>
		/// <param name="member">Member to get nodes for.</param>
		/// <returns>Connecting node.</returns>
		/// <exception cref="ArgumentException">If the <paramref name="member"/> is not known.</exception>
		IEnumerable<IIdeaNode> GetNodesOnMember(IIdeaMember1D member);
	}
}