using FeaExampleApi;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace BimApiLinkFeaExample
{
	/// <summary>
	/// Tells Checkbot what the user selected in the FEA application and which combinations the model has. It answers
	/// with identifiers only; Checkbot then asks the importers for each object.
	/// </summary>
	internal class Model : IFeaModel
	{
		private readonly IFeaGeometryApi _geometry;
		private readonly IFeaLoadsApi _loads;

		public Model(IFeaGeometryApi geometry, IFeaLoadsApi loads)
		{
			_geometry = geometry;
			_loads = loads;
		}

		/// <summary>
		/// Information about the source model that Checkbot keeps with the import. Only the project name is set here.
		/// </summary>
		public OriginSettings GetOriginSettings() => new OriginSettings { ProjectName = "FEA example" };

		/// <summary>
		/// Every member of the model. BimApiLink builds a graph of nodes and members from them, to find which members
		/// meet at each node.
		/// </summary>
		public IEnumerable<Identifier<IIdeaMember1D>> GetAllMembers()
			=> _geometry.GetAllMembers().Select(member => new IntIdentifier<IIdeaMember1D>(member.Id));

		/// <summary>
		/// Answers Checkbot's import: the nodes and members the user selected. Each selected node becomes a connection
		/// of the selected members that meet there, and so does each node where two or more selected members meet.
		/// </summary>
		public FeaUserSelection GetUserSelection()
		{
			return new FeaUserSelection
			{
				Nodes = _geometry.GetSelectedNodes()
					.Select(node => new IntIdentifier<IIdeaNode>(node.Id))
					.ToList<Identifier<IIdeaNode>>(),
				Members = _geometry.GetSelectedMembers()
					.Select(member => new IntIdentifier<IIdeaMember1D>(member.Id))
					.ToList<Identifier<IIdeaMember1D>>(),
			};
		}

		/// <summary>
		/// Every combination of the model. Checkbot imports them with the load cases and load groups they use, and
		/// the connections are checked for them.
		/// </summary>
		public IEnumerable<Identifier<IIdeaCombiInput>> GetAllCombinations()
			=> _loads.GetAllCombinations().Select(combination => new IntIdentifier<IIdeaCombiInput>(combination.Id));

		/// <summary>
		/// Would select the nodes and members in the FEA application. <see cref="IdeaStatiCa.BimApiLink.FeaBimLink"/>
		/// never calls it, so it does nothing here.
		/// </summary>
		public void SelectUserSelection(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members)
		{
		}
	}
}
