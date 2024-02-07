using BimApiLinkCadExample.CadBulkSelection;
using BimApiLinkCadExample.CadExampleApi;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.BIM;
using IdeaStatiCa.BIM.Common;
using System;
using System.Collections.Generic;
using System.Linq;

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
		/// </summary>
		public IEnumerable<CadUserSelection> GetUserSelections()
		{
			var selections = new List<CadUserSelection>();

			foreach (var joint in GetBulkSelection())
			{
				selections.Add(GetCadUserSelection(joint.Item1, joint.Item2, joint.Item3));
			}

			return selections;
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

			foreach (var member in members)
			{
				identifiers.Add(GetIdentifier(member));
			}

			foreach (var part in selectedObject)
			{
				identifiers.Add(GetIdentifier(part));
			}

			selection.Objects = identifiers;

			return selection;
		}

		private IIdentifier GetIdentifier(CadObject item)
		{
			if (item is CadMember)
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



		/// <summary>
		/// User select bulk selection
		/// </summary>
		/// <returns></returns>
		public List<(CadPoint3D, List<CadMember>, List<CadObject>)> GetBulkSelection(bool selectWholeModel = false)
		{
			//_plugInLogger.LogInformation("GetBulkSelection");

			List<(CadPoint3D, List<CadMember>, List<CadObject>)> selections = new List<(CadPoint3D, List<CadMember>, List<CadObject>)>();
			{
				//Here we will get all the objects in the Geometry API.
				//This would be where you would call your program to get the selection from the UI of your program.

				List<int> allitems = _model.GetAllGeometricalItems().ConvertAll(x => x.Id);

				SorterResult sortedJoints = BulkSelectionHelper.FindJoints(allitems, _model);

				//_plugInLogger.LogInformation($"GetBulkSelection found joints {sortedJoints.Joints.Count}");

				foreach (var joint in sortedJoints.Joints)
				{
					//_plugInLogger.LogInformation($"GetBulkSelection joint {joint.Location.X} {joint.Location.Y} {joint.Location.Z}");

					List<CadMember> beams = new List<CadMember>();
					List<CadObject> parts = new List<CadObject>();

					var structuralMembers = joint.Members;
					//.Where(m => (m.Parent as TS.Part)?.Name != BulkSelectionHelper.HaunchMemberName)
					//.Where(m => (m.Parent as TS.Part)?.Name != BulkSelectionHelper.TeklaAnchorRodName)
					//.Where(m => (m.Parent as TS.Part)?.Name != BulkSelectionHelper.TeklaAnchorWasherName)
					//.Where(m => (m.Parent as TS.Part)?.Name != BulkSelectionHelper.TeklaAnchorNutName);

					structuralMembers.ToList().ForEach(sm => beams.Add(sm.Parent as CadMember));

					//_plugInLogger.LogInformation($"GetBulkSelection joint number of members {beams.Count}");

					//_plugInLogger.LogInformation($"GetBulkSelection joint number of plates {joint.Plates.Count}");
					foreach (var plate in joint.Plates)
					{
						if (plate.Parent is CadPlate pl)
						{
							parts.Add(pl);
						}
					}

					//EXAMPLE IS NOT CURRENTLY SET UP FOR STIFFENING MEMBERS
					//_plugInLogger.LogInformation($"GetBulkSelection joint number of stiffening members {joint.StiffeningMembers.Count}");
					//foreach (var stiffeningmember in joint.StiffeningMembers)
					//{
					//	if (stiffeningmember.Parent is TS.ModelObject tsObject)
					//	{
					//		parts.Add(tsObject);
					//	}
					//}

					//_plugInLogger.LogInformation($"GetBulkSelection joint number of fasteners {joint.Fasteners.Count}");
					foreach (var jointFastener in joint.Fasteners)
					{
						if (jointFastener.Parent is CadBoltGrid bg)
						{
							parts.Add(bg);
						}
					}

					//_plugInLogger.LogInformation($"GetBulkSelection joint number of welds {joint.Welds.Count}");
					foreach (var jointWeld in joint.Welds)
					{
						if (jointWeld.Parent is CadWeld weld)
						{
							parts.Add(weld);
						}
					}

					selections?.Add((new CadPoint3D(joint.Location.X, joint.Location.Y, joint.Location.Z), beams, parts));
				}
			}
			return selections;
		}
	}
}