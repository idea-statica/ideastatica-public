using BimApiLinkCadExample.CadExampleApi;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System;

namespace BimApiLinkCadExample
{
	internal class Model : ICadModel
	{
		private readonly ICadGeometryApi _model;
		private readonly IProgressMessaging _messagingService;

		public Model(ICadGeometryApi geometry, IProgressMessaging messagingService)
		{
			_model = geometry;
			_messagingService = messagingService;
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
			return _model.GetAllMembers()
				.Select(x => new IntIdentifier<IIdeaMember1D>(x.Id));
		}

		/// <summary>
		/// Get User Selection - Single connection
		/// </summary>
		public CadUserSelection GetUserSelection()
		{
			try
			{
				//The below can be implemented if the Authoring Software has a interface for Connection Design.
				CadPoint3D point = _model.GetConnectionPoint();
				var members = _model.GetSelectedMembers();
				var selectedObjects = _model.GetSelectPartObjects();
				return GetCadUserSelection(point, members, selectedObjects);
			}
			catch 
			{
				return null;
			}
		}

		/// <summary>
		/// Get User Selection - bulk
		/// TODO Bulk Selection Not Currently Implemented - return the result of the single selection.
		/// </summary>
		public IEnumerable<CadUserSelection> GetUserSelections()
		{
			return new List<CadUserSelection>() { GetUserSelection() };
			
			//var selections = new List<CadUserSelection>();

			//foreach (var joint in _model.GetBulkSelection())
			//{
			//	selections.Add(GetCadUserSelection(joint.Item1, joint.Item2, joint.Item3));
			//}

			//return selections;
		}

		public IEnumerable<CadUserSelection> GetSelectionOfWholeModel()
		{
			throw new System.NotImplementedException();
		}

		private CadUserSelection GetCadUserSelection(CadPoint3D point, IEnumerable<CadMember> members, IEnumerable<CadObject> selectedObject)
		{
			CadUserSelection selection = new CadUserSelection()
			{
				ConnectionPoint = new ConnectionIdentifier<IIdeaConnectionPoint>(point.X, point.Y, point.Z),
				Members = members
								.Select(x => new ConnectedMemberIdentifier<IIdeaConnectedMember>(x.Id.ToString()))
								.Cast<Identifier<IIdeaConnectedMember>>()
								.ToList(),
				Objects = new List<IIdentifier>(),
			};

			List<IIdentifier> identifiers = new List<IIdentifier>();

			foreach(var member in members) 
			{
				identifiers.Add(GetIdentifier(member));
			}

			foreach(var part in selectedObject) 
			{
				identifiers.Add(GetIdentifier(part));
			}

			selection.Objects = identifiers;
			
			return selection;
		}

		private IIdentifier GetIdentifier(CadObject item)
		{
			if(item is CadMember)
				return new IntIdentifier<IIdeaMember1D>(item.Id);
			else if (item is CadPlate)
				return new IntIdentifier<IIdeaPlate>(item.Id);
			else if (item is CadCutByPart)
				return new IntIdentifier<IIdeaCut>(item.Id);
			else if (item is CadWeld)
				return new IntIdentifier<IIdeaWeld>(item.Id);
			else if (item is CadBoltGrid)
				return new IntIdentifier<IIdeaBoltGrid>(item.Id);
			else
				throw new NotImplementedException(item.GetType().Name);
		}
	}
}