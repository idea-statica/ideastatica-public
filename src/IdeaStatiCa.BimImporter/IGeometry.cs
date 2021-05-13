using IdeaStatiCa.BimApi;
using System.Collections.Generic;
using System;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// Provides a way to search members by nodes and nodes by members.
	/// </summary>
	public interface IGeometry
	{
		/// <summary>
		/// Builds member/node data from <paramref name="model"/>.
		/// </summary>
		/// <param name="model"></param>
		void Build(IIdeaModel model);

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

		/// <summary>
		/// Returns all members in the model.
		/// </summary>
		/// <returns>Enumerable of all members.</returns>
		IEnumerable<IIdeaMember1D> GetMembers();

		/// <summary>
		/// Returns all nodes in the model.
		/// </summary>
		/// <returns>Enumerable of all nodes.</returns>
		IEnumerable<IIdeaNode> GetNodes();
	}
}