using BimApiLinkFeaExample.FeaExampleApi;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace BimApiLinkFeaExample
{
	internal class Model : IFeaModel
	{
		private readonly IFeaGeometryApi geometry;
		private readonly IProgressMessaging messagingService;

		public Model(IFeaGeometryApi geometry, IProgressMessaging messagingService)
		{
			this.geometry = geometry;
			this.messagingService = messagingService;
		}

		public OriginSettings GetOriginSettings()
		{
			return new OriginSettings()
			{
				ProjectName = "TestProject",
			};
		}

		/// <summary>
		/// Returns all members in the model to be able find all related (connected) members, when select node only.
		/// </summary>
		/// <returns>All members in the model to be able find all related (connected) members, when select node only.</returns>
		public IEnumerable<Identifier<IIdeaMember1D>> GetAllMembers()
		{
			return geometry.GetMembersIdentifiers()
				.Select(x => new IntIdentifier<IIdeaMember1D>(x));
		}

		/// <summary>
		/// Returns user selection...nodes, members, ...
		/// When nodes and members are selected, no more members are added from <see cref="GetAllMembers"/>.
		/// From all selected nodes (that containes at least one member) are created connections.
		/// When only members are selected, connection nodes are recognized automatically. From this nodes are created connections
		/// </summary>
		/// <returns></returns>
		public FeaUserSelection GetUserSelection()
		{
			List<Identifier<IIdeaNode>> nodes = geometry.GetNodesIdentifiers()
				.Select(x => new IntIdentifier<IIdeaNode>(x))
				.Cast<Identifier<IIdeaNode>>()
				.ToList();

			List<Identifier<IIdeaMember1D>> members = geometry.GetMembersIdentifiers()
				.Select(x => new IntIdentifier<IIdeaMember1D>(x))
				.Cast<Identifier<IIdeaMember1D>>()
				.ToList();

			return new FeaUserSelection()
			{
				Members = members,
				Nodes = nodes
			};
		}

		/// <inheritdoc cref="SelectUserSelection"/>
		public void SelectUserSelection(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members) 
		{
			//feaApi.SelectNodes(nodes, members);
		}
	}
}