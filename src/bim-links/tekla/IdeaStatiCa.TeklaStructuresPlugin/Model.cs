using CI;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Geometry3d;
using TS = Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin
{
	internal class Model : ICadModel
	{
		private readonly IModelClient _model;

		public Model(IModelClient modelClient)
		{
			_model = modelClient;
		}

		/// <summary>
		/// Get All Members
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Identifier<IIdeaMember1D>> GetAllMembers() =>
			_model.GetAllMembers().Select(x => new StringIdentifier<IIdeaMember1D>(x.Identifier.GUID.ToString()));

		/// <summary>
		/// Get Origin Settings
		/// </summary>
		/// <returns></returns>
		public OriginSettings GetOriginSettings()
		{
			return new OriginSettings
			{
				ProjectName = _model.GetProjectName(),
				CheckEquilibrium = true,
				DateOfCreate = DateTime.Now,
			};
		}

		/// <summary>
		/// Get User Selection - single connection
		/// </summary>
		/// <returns></returns>
		public CadUserSelection GetUserSelection()
		{
			try
			{
				var point = _model.GetConnectionPoint();
				var members = _model.GetSelectBeams();
				var selectedObject = _model.GetSelectObjects().Distinct();
				return GetCadUserSelection(point, members, selectedObject);
			}
			catch
			{
				return null;
			}
		}

		private CadUserSelection GetCadUserSelection(Point point, IEnumerable<TS.ModelObject> members, IEnumerable<TS.ModelObject> selectedObject)
		{
			CadUserSelection selection = new CadUserSelection()
			{
				ConnectionPoint = new ConnectionIdentifier<IIdeaConnectionPoint>(point.X, point.Y, point.Z),
				Members = members
								.Select(x => new ConnectedMemberIdentifier<IIdeaConnectedMember>(x.Identifier.GUID.ToString()))
								.Cast<Identifier<IIdeaConnectedMember>>()
								.ToList(),
				Objects = new List<IIdentifier>(),
			};

			List<IIdentifier> identifiers = new List<IIdentifier>();

			selectedObject.ForEach(teklaObject => identifiers = IdentifierHelper.GetIdentifier(teklaObject, ref identifiers, connectionPoint: point));

			members.ForEach(teklaObject =>
			{
				if (teklaObject is TS.Beam beam)
				{
					identifiers = IdentifierHelper.GetIdentifier(beam, ref identifiers, false, point);
				}
			});

			identifiers.ForEach(id => (selection.Objects as List<IIdentifier>).Add(id));
			return selection;
		}



		/// <summary>
		/// Get User Selections - bulk
		/// </summary>
		/// <returns></returns>
		public IEnumerable<CadUserSelection> GetUserSelections()
		{
			var selections = new List<CadUserSelection>();

			foreach (var joint in _model.GetBulkSelection())
			{
				selections.Add(GetCadUserSelection(joint.Item1, joint.Item2, joint.Item3));
			}

			return selections;
		}

		/// <summary>
		/// Get Selections - bulk on whole model
		/// </summary>
		/// <returns></returns>
		public IEnumerable<CadUserSelection> GetSelectionOfWholeModel()
		{
			var selections = new List<CadUserSelection>();

			foreach (var joint in _model.GetBulkSelection(true))
			{
				selections.Add(GetCadUserSelection(joint.Item1, joint.Item2, joint.Item3));
			}

			return selections;
		}
	}
}
