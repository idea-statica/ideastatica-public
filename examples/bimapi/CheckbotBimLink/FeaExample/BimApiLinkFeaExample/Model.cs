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
		private readonly IFeaLoadsApi loads;
		private readonly IProgressMessaging messagingService;

		public Model(IFeaGeometryApi geometry, IFeaLoadsApi loads, IProgressMessaging messagingService)
		{
			this.geometry = geometry;
			this.loads = loads;
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

			List<Identifier<IIdeaCombiInput>> loadCombinations = loads.GetLoadCombinationsIds()
				.Select(x => new IntIdentifier<IIdeaCombiInput>(x))
				.Cast<Identifier<IIdeaCombiInput>>()
				.ToList();

			return new FeaUserSelection()
			{
				Members = members,
				Nodes = nodes,
				Combinations = loadCombinations				
			};
		}

		/// <inheritdoc cref="SelectUserSelection"/>
		public void SelectUserSelection(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members) 
		{
			//feaApi.SelectNodes(nodes, members);
		}
	}
}