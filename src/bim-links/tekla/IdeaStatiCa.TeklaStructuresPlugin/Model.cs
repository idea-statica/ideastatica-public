using CI;
using IdeaRS.OpenModel;
using IdeaStatiCa.BIM.Common;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using IdeaStatiCa.TeklaStructuresPlugin.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Geometry3d;
using TS = Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin
{
	internal class Model : ICadModel, IProgressMessagingAware
	{
		private readonly IModelClient _model;
		private readonly SorterSettings _sorterSettings;
		private readonly IPluginLogger _logger;

		public IProgressMessaging ProgressMessaging { get; set; }

		public Model(IModelClient modelClient, SorterSettings sorterSettings, IPluginLogger logger = null)
		{
			_model = modelClient;
			_sorterSettings = sorterSettings;
			_logger = logger;
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
				_model.ClearCache();
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

		private CadUserSelection GetCadUserSelection(Point point, IEnumerable<TS.ModelObject> members, IEnumerable<TS.ModelObject> selectedObject, IReadOnlyList<Point> allConnectionPoints = null)
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

			selectedObject.Where(selObj => !members.Contains(selObj)).ForEach(teklaObject => identifiers = IdentifierHelper.GetIdentifier(teklaObject, ref identifiers, connectionPoint: point, allConnectionPoints: allConnectionPoints, logger: _logger));

			members.ForEach(teklaObject =>
			{
				if (teklaObject is TS.Beam beam)
				{
					identifiers = IdentifierHelper.GetIdentifier(beam, ref identifiers, false, point, allConnectionPoints, _logger);
				}
			});

			// For bolts in this selection, add any connected plates that were not picked up
			// by the sorter (their centroid fell outside all node BBs).  These are typically
			// gusset plates on diagonals.
			// Also find bolts on plates that are in this selection but whose bolt origin fell
			// outside all node BBs (bolt itself unregistered but its plates are here).
			var extraPlates = new List<IIdentifier>();
			var extraBolts = new List<IIdentifier>();

			// Plates that came from joint.Plates (sorter) via selectedObject but were not found
			// by IdentifierHelper (e.g. triangular ContourPlates, non-rectangular stiffeners).
			// Only add if not already registered in another CP (avoids cross-contamination).
			foreach (var selObj in selectedObject)
			{
				var guid = (selObj as TS.ModelObject)?.Identifier.GUID.ToString();
				if (guid == null) continue;
				if (identifiers.Any(id => id.GetId()?.ToString() == guid)) continue;
				if (_model.IsRegisteredInAnyConnection(guid)) continue;
				bool isPlate = selObj is TS.ContourPlate
					|| (selObj is TS.Beam bSel && BulkSelectionHelper.IsRectangularCssBeam(bSel));
				if (!isPlate) continue;
				extraPlates.Add(new StringIdentifier<IIdeaPlate>(guid));
				_logger?.LogDebug($"GetCadUserSelection: added selectedObject plate guid={guid} name='{(selObj as TS.Part)?.Name}' for CP [{point.X:F0},{point.Y:F0},{point.Z:F0}]");
			}

			// Collect unregistered plates referenced by welds in identifiers.
			// This catches plates attached only by welds (no bolts) that fall outside node BBs.
			foreach (var weldId in identifiers.OfType<StringIdentifier<IIdeaWeld>>())
			{
				if (!(_model.GetItemByHandler(weldId.GetId()?.ToString()) is TS.BaseWeld weld)) continue;
				foreach (var weldPart in new[] { weld.MainObject, weld.SecondaryObject }.Where(p => p != null))
				{
					var wGuid = weldPart.Identifier.GUID.ToString();
					bool isPlate = (weldPart is TS.Beam bw && BulkSelectionHelper.IsRectangularCssBeam(bw))
						|| weldPart is TS.ContourPlate;
					if (!isPlate) continue;
					if (identifiers.Any(id => id.GetId()?.ToString() == wGuid)) continue;
					if (_model.IsRegisteredInAnyConnection(wGuid)) continue;
					extraPlates.Add(new StringIdentifier<IIdeaPlate>(wGuid));
					_logger?.LogDebug($"GetCadUserSelection: added weld-referenced plate guid={wGuid} name='{(weldPart as TS.Part)?.Name}' for CP [{point.X:F0},{point.Y:F0},{point.Z:F0}]");
				}
			}

			// Collect bolts from plates in selectedObject that are not yet in identifiers.
			foreach (var plateObj in selectedObject.OfType<TS.Part>()
				.Where(p => BulkSelectionHelper.IsRectangularCssBeam(p) || p is TS.ContourPlate))
			{
				var boltEnum = plateObj.GetBolts();
				while (boltEnum.MoveNext())
				{
					if (boltEnum.Current is TS.BoltGroup bg)
					{
						var bgGuid = bg.Identifier.GUID.ToString();
						if (!identifiers.Any(id => id.GetId()?.ToString() == bgGuid)
							&& !_model.IsRegisteredInAnyConnection(bgGuid))
						{
							extraBolts.Add(new StringIdentifier<IIdeaBoltGrid>(bgGuid));
							_logger?.LogDebug($"GetCadUserSelection: added unregistered BoltGroup guid={bgGuid} from plate '{plateObj.Name}' for CP [{point.X:F0},{point.Y:F0},{point.Z:F0}]");
						}
					}
				}
			}
			extraBolts.ForEach(id => identifiers.Add(id));

			foreach (var boltObj in selectedObject.OfType<TS.BoltGroup>()
				.Concat(identifiers
					.OfType<StringIdentifier<IIdeaBoltGrid>>()
					.Select(id => _model.GetItemByHandler(id.GetId()?.ToString()) as TS.BoltGroup)
					.Where(bg => bg != null)))
			{
				var connectedParts = new[]
				{
					boltObj.PartToBoltTo,
					boltObj.PartToBeBolted,
				}
				.Concat(boltObj.OtherPartsToBolt?.OfType<TS.Part>() ?? Enumerable.Empty<TS.Part>())
				.Where(p => p != null);

				foreach (var part in connectedParts)
				{
					var guid = part.Identifier.GUID.ToString();
					// Only actual plates — not structural members like columns/beams.
					bool isPlate = (part is TS.Beam bPart && BulkSelectionHelper.IsRectangularCssBeam(bPart))
						|| part is TS.ContourPlate;
					if (!isPlate) continue;
					// Skip if already in identifiers or if registered in any other CP.
					if (identifiers.Any(id => id.GetId()?.ToString() == guid)) continue;
					if (_model.IsRegisteredInAnyConnection(guid)) continue;
					// Add as plate identifier so it gets imported for this CP.
					extraPlates.Add(new StringIdentifier<IIdeaPlate>(guid));
					_logger?.LogDebug($"GetCadUserSelection: added unregistered ConnectedPart plate guid={guid} name='{part.Name}' for CP [{point.X:F0},{point.Y:F0},{point.Z:F0}]");
				}
			}
			extraPlates.ForEach(id => identifiers.Add(id));

			identifiers.ForEach(id => (selection.Objects as List<IIdentifier>).Add(id));

			// Register the GUID set for this CP so ConnectionImporter.Create can activate it
			// at the right moment — after ProcessConnectionObjects has added BoltGrids/Welds.
			// Key format must match ConnectionIdentifier.GetPointId (format "G", InvariantCulture).
			var nodeKey = $"{point.X.ToString("G", System.Globalization.CultureInfo.InvariantCulture)};{point.Y.ToString("G", System.Globalization.CultureInfo.InvariantCulture)};{point.Z.ToString("G", System.Globalization.CultureInfo.InvariantCulture)}";
			var connectionGuids = members.Concat(selectedObject)
				.Select(o => o.Identifier.GUID.ToString())
				.Concat(identifiers.Select(id => id.GetId()?.ToString()).Where(g => g != null))
				.ToHashSet();
			_model.RegisterConnectionGuids(nodeKey, connectionGuids);

			return selection;
		}



		/// <summary>
		/// Get User Selections - bulk
		/// </summary>
		/// <returns></returns>
		public IEnumerable<CadUserSelection> GetUserSelections()
		{
			_model.ClearCache();
			var selections = new List<CadUserSelection>();
			var joints = _model.GetBulkSelection(progressMessaging: ProgressMessaging, sorterSettings: _sorterSettings);
			int total = joints.Count;

			var allConnectionPoints = joints.Select(j => j.Item1).ToList();

			for (int i = 0; i < total; i++)
			{
				ProgressMessaging?.SetStageLocalised(i + 1, total, LocalisedMessage.ProcessingConnection, string.Empty);
				selections.Add(GetCadUserSelection(joints[i].Item1, joints[i].Item2, joints[i].Item3, allConnectionPoints));
			}

			return selections;
		}

		/// <summary>
		/// Get Selections - bulk on whole model
		/// </summary>
		/// <returns></returns>
		public IEnumerable<CadUserSelection> GetSelectionOfWholeModel()
		{
			_model.ClearCache();
			var joints = _model.GetBulkSelection(true, sorterSettings: _sorterSettings).ToList();
			var allConnectionPoints = joints.Select(j => j.Item1).ToList();

			var selections = new List<CadUserSelection>();
			foreach (var joint in joints)
			{
				selections.Add(GetCadUserSelection(joint.Item1, joint.Item2, joint.Item3, allConnectionPoints));
			}

			return selections;
		}
	}
}
