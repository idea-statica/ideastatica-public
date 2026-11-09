using BimApiLinkCadExample.BulkSelection;
using CadExampleApi;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BimApiLinkCadExample
{
	/// <summary>
	/// Tells Checkbot which connections the user selected in the CAD application. A connection is given by
	/// identifiers only; Checkbot then asks the importers for each object.
	/// </summary>
	internal class Model : ICadModel
	{
		private readonly ICadGeometryApi _cad;

		public Model(ICadGeometryApi cad)
		{
			_cad = cad;
		}

		/// <summary>
		/// Information about the source model that Checkbot keeps with the import. Only the project name is set here.
		/// </summary>
		public OriginSettings GetOriginSettings() => new OriginSettings { ProjectName = "CAD example" };

		/// <summary>
		/// Every member of the model. BimApiLink builds a graph of nodes and members from them, to find which members
		/// meet at each node.
		/// </summary>
		public IEnumerable<Identifier<IIdeaMember1D>> GetAllMembers()
			=> _cad.GetAllMembers().Select(member => new IntIdentifier<IIdeaMember1D>(member.Id));

		/// <summary>
		/// Answers Checkbot's <b>One</b> import: one connection, at the point the user picks, made of what the user
		/// selected. Returns null when the user cancels the pick, and Checkbot then imports nothing.
		/// </summary>
		public CadUserSelection GetUserSelection()
		{
			CadPoint3D point = _cad.PickConnectionPoint();
			if (point == null)
			{
				return null;
			}

			return CreateSelection(point, _cad.GetSelectedMembers(), _cad.GetSelectedConnectionObjects());
		}

		/// <summary>
		/// Answers Checkbot's <b>Bulk</b> import: every connection found among the objects the user selected.
		/// </summary>
		public IEnumerable<CadUserSelection> GetUserSelections()
			=> FindConnections(_cad.GetSelectedMembers().Concat(_cad.GetSelectedConnectionObjects()));

		/// <summary>
		/// Answers an import of the whole model: every connection in the model.
		/// </summary>
		public IEnumerable<CadUserSelection> GetSelectionOfWholeModel()
			=> FindConnections(_cad.GetAllObjects());

		/// <summary>
		/// Finds the connections among the objects. The joint finder does not look at cuts, so each connection gets the
		/// cuts of its members and plates. A cut at the far end of a member is included as well; a real link would keep
		/// only the cuts near the connection.
		/// </summary>
		private IEnumerable<CadUserSelection> FindConnections(IEnumerable<CadObject> objects)
		{
			return JointFinder.FindJoints(objects, _cad)
				.Select(joint =>
				{
					IEnumerable<CadCut> cuts = joint.Members.Cast<ICadPart>()
						.Concat(joint.ConnectionObjects.OfType<CadPlate>())
						.SelectMany(part => _cad.GetCutsOf(part.Id));

					return CreateSelection(joint.Location, joint.Members, joint.ConnectionObjects.Concat(cuts));
				})
				.ToList();
		}

		/// <summary>
		/// Describes one connection by identifiers: its point, its members, and its plates, bolt grids and cuts.
		/// </summary>
		private static CadUserSelection CreateSelection(CadPoint3D point, IEnumerable<CadMember> members, IEnumerable<CadObject> connectionObjects)
		{
			return new CadUserSelection
			{
				ConnectionPoint = new ConnectionIdentifier<IIdeaConnectionPoint>(point.X, point.Y, point.Z),
				Members = members
					.Select(member => new ConnectedMemberIdentifier<IIdeaConnectedMember>(member.Id.ToString()))
					.ToList<Identifier<IIdeaConnectedMember>>(),
				Objects = connectionObjects.Select(GetIdentifier).ToList(),
			};
		}

		/// <summary>
		/// The identifier Checkbot passes to the importer of that kind of object.
		/// </summary>
		private static IIdentifier GetIdentifier(CadObject connectionObject)
		{
			switch (connectionObject)
			{
				case CadPlate plate:
					return new IntIdentifier<IIdeaPlate>(plate.Id);
				case CadBoltGrid boltGrid:
					return new IntIdentifier<IIdeaBoltGrid>(boltGrid.Id);
				case CadCut cut:
					return new IntIdentifier<IIdeaCut>(cut.Id);
				default:
					throw new NotSupportedException($"A {connectionObject.GetType().Name} cannot be part of a connection.");
			}
		}
	}
}
