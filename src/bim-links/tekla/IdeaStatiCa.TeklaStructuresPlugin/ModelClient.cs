using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.Utilities;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Picker = Tekla.Structures.Model.UI.Picker;
using TS = Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin
{
	public class ModelClient : IModelClient
	{
		private readonly TS.Model teklaModel;
		private readonly IPluginLogger plugInLogger;
		private readonly BIM.Common.SorterSettings sorterSettings;

		private readonly Dictionary<IIdentifier, IIdeaObject> cachedObjects;

		public ModelClient(TS.Model teklaModel, IPluginLogger plugInLogger, BIM.Common.SorterSettings sorterSettings)
		{
			this.teklaModel = teklaModel;
			this.plugInLogger = plugInLogger;
			this.sorterSettings = sorterSettings;
			cachedObjects = new Dictionary<IIdentifier, IIdeaObject>();
		}

		private TS.Model GetTeklaModel()
		{
			if (!teklaModel.GetConnectionStatus())
			{
				plugInLogger.LogInformation("GetTeklaModel Tekla is not running.");
				throw new ArgumentException("Tekla is not running. Press Enter.");
			}

			return teklaModel;
		}

		/// <summary>
		/// Get all members from Advance steel project
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TS.ModelObject> GetAllMembers()
		{
			plugInLogger.LogInformation("GetAllMembers");
			var model = GetTeklaModel();
			var beamsEnumerator = model.GetModelObjectSelector().GetAllObjectsWithType(TS.ModelObject.ModelObjectEnum.BEAM);
			var polyBeamsEnumerator = model.GetModelObjectSelector().GetAllObjectsWithType(TS.ModelObject.ModelObjectEnum.POLYBEAM);

			List<TS.ModelObject> members = new List<TS.ModelObject>();
			while (beamsEnumerator.MoveNext())
			{
				members.Add(beamsEnumerator.Current);
			}

			while (polyBeamsEnumerator.MoveNext())
			{
				members.Add(polyBeamsEnumerator.Current);
			}
			return members;
		}

		/// <summary>
		/// Get Item By Handler
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		public TS.Object GetItemByHandler(string handle)
		{
			plugInLogger.LogInformation($"GetItemByHandler handle {handle}.");
			var model = GetTeklaModel();

			var handlers = handle.Split(';');
			//skip construction 
			var itemHandle = handlers[0];
			if (handlers.Length > 1)
			{
				itemHandle = handlers[1];
			}

			var identifier = new Tekla.Structures.Identifier(itemHandle);
			plugInLogger.LogDebug($"GetItemByHandler readItem by identifier {identifier.ToString()}.");

			return model.SelectModelObject(identifier);
		}

		/// <summary>
		/// Get Parent item by Handler
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		public TS.Object GetParentItemByHandler(string handle)
		{
			plugInLogger.LogInformation($"GetParentItemByHandler by handle {handle}.");
			var handlers = handle.Split(';');

			if (handlers.Length > 1)
			{
				return GetItemByHandler(handlers[0]);
			}
			else
			{
				plugInLogger.LogDebug($"GetParentItemByHandler unknown handle {handle}.");
				return null;
			}
		}

		/// <summary>
		/// Get Project Path
		/// </summary>
		/// <returns></returns>
		public string GetProjectPath()
		{
			plugInLogger.LogInformation($"GetProjectPath.");
			var model = GetTeklaModel();
			var modelInfo = model.GetInfo();
			var projectPath = Path.Combine(modelInfo.ModelPath, modelInfo.ModelName);
			plugInLogger.LogDebug($"GetProjectPath found path {projectPath}.");
			if (HasWritePermissionOnDir(projectPath))
			{
				return projectPath;
			}
			else
			{
				plugInLogger.LogDebug($"GetProjectPath found path {projectPath} has not write permission.");
				return GetBackUpProjectPath(projectPath);
			}
		}

		/// <summary>
		/// Get Connection Point by user action
		/// </summary>
		/// <returns></returns>
		public Point GetConnectionPoint()
		{
			plugInLogger.LogInformation($"GetConnectionPoint.");
			GetTeklaModel();

			var picker = new Picker();
			var connectionPoint = picker.PickPoint(TeklaStructuresResources.Properties.Resources.SelectCoonecntionPoint);
			if (connectionPoint == null)
			{
				plugInLogger.LogDebug($"GetConnectionPoint - not selected point");
				throw new InvalidOperationException("Invalid Point - not selected point");
			}
			plugInLogger.LogDebug($"GetConnectionPoint - selected point {connectionPoint.X} {connectionPoint.Y} {connectionPoint.Z}");
			return connectionPoint;
		}

		/// <summary>
		/// User select beams
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TS.ModelObject> GetSelectBeams()
		{
			plugInLogger.LogInformation($"GetSelectBeams.");
			GetTeklaModel();
			var picker = new Picker();

			var selectedItems = new List<TS.ModelObject>();

			TS.ModelObjectEnumerator partsEnumerator;
			try
			{
				partsEnumerator = picker.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, TeklaStructuresResources.Properties.Resources.SelectBeams);
			}
			catch (ApplicationException ex)
			{
				plugInLogger.LogInformation($"GetSelectBeams - selection failed", ex);
				return selectedItems;
			}


			while (partsEnumerator.MoveNext())
			{
				if (partsEnumerator.Current is TS.Beam beam)
				{
					plugInLogger.LogDebug($"GetSelectBeams. selected beam id {beam.Identifier} name {beam.Name}");
					selectedItems.Add(partsEnumerator.Current);
				}
			}

			return selectedItems;
		}

		/// <summary>
		/// User select connection objects
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TS.ModelObject> GetSelectObjects()
		{
			plugInLogger.LogInformation($"GetSelectObjects - ask user for selection");
			ModelObjectEnumerator partsEnumerator = UserObjectsSelection(TeklaStructuresResources.Properties.Resources.SelectParts);

			plugInLogger.LogInformation($"GetSelectObjects - process user selection");
			var selectedItems = ProcessUserSelection(partsEnumerator);
			return selectedItems;
		}

		private ModelObjectEnumerator UserObjectsSelection(string prompt)
		{
			plugInLogger.LogInformation($"UserObjectsSelection");
			GetTeklaModel();
			var picker = new Picker();
			ModelObjectEnumerator partsEnumerator = null;
			try
			{
				partsEnumerator = picker.PickObjects(Picker.PickObjectsEnum.PICK_N_OBJECTS, prompt);
			}
			catch (ApplicationException e)
			{
				plugInLogger.LogDebug($"UserObjectsSelection failed {e.Message}", e);
				return partsEnumerator;
			}

			return partsEnumerator;
		}

		private List<ModelObject> ProcessUserSelection(ModelObjectEnumerator partsEnumerator)
		{
			plugInLogger.LogInformation($"ProcessUserSelection");
			List<ModelObject> selectedItems = new List<ModelObject>();
			if (partsEnumerator == null)
			{
				plugInLogger.LogInformation($"ProcessUserSelection - partsEnumerator is null");
				return selectedItems;
			}

			List<Tekla.Structures.Identifier> proceseedDetails = new List<Tekla.Structures.Identifier>();
			while (partsEnumerator.MoveNext())
			{
				var partOfEnumerator = partsEnumerator.Current;
				if (partOfEnumerator is TS.Part tsPart)
				{
					if (IdentifierHelper.AnchorMemberFilter(tsPart) && tsPart.GetFatherComponent() is TS.Detail detail)
					{
						ProcessDetailPart(selectedItems, proceseedDetails, detail);

					}
					else
					{
						selectedItems.Add(tsPart);
					}
				}
				else if (partOfEnumerator is TS.Detail detail)
				{
					ProcessDetailPart(selectedItems, proceseedDetails, detail);


				}
				else if (partOfEnumerator is TS.BaseComponent baseComponent)
				{
					plugInLogger.LogDebug($"Component {baseComponent.Name} add child parts");
					foreach (var componentItem in baseComponent.GetChildren())
					{
						if (componentItem is TS.Part part)
						{
							selectedItems.Add(part);
						}
					}
				}
			}
			return selectedItems;
		}

		private void ProcessDetailPart(List<ModelObject> selectedItems, List<Tekla.Structures.Identifier> proceseedDetails, Detail detail)
		{
			if (proceseedDetails.Any(id => id.Equals(detail.Identifier)))
			{
				plugInLogger.LogDebug($"ProcessUserSelection - skip {detail.Identifier} name:{detail.Name}");
				//skip duplicity
				return;
			}
			else
			{
				proceseedDetails.Add(detail.Identifier);
			}

			var detailItems = new List<TS.ModelObject>();
			var anchorItems = new List<TS.ModelObject>();
			bool notFoundAnchor = true;
			// Find anchors from the detail
			foreach (var detailItem in detail.GetChildren())
			{
				if (detailItem is TS.Part part)
				{
					detailItems.Add(part);

					if (IdentifierHelper.AnchorMemberFilter(part)) //add only one anchor from group anchor importer than takes all anchor positions
					{
						if (notFoundAnchor)
						{
							anchorItems.Add(part);
							notFoundAnchor = false;
						}
					}
					else if (part is TS.Beam b && (!IdentifierHelper.WasherMemberFilter(part) && !IdentifierHelper.NutMemberFilter(part)))
					{
						anchorItems.Add(part);
					}
					else if (part is ContourPlate)
					{
						if (!IdentifierHelper.GroutFilter(part) && !IdentifierHelper.CastPlateFilter(part))
						{
							anchorItems.Add(part);
						}
					}
				}
			}

			if (notFoundAnchor)
			{
				plugInLogger.LogInformation($"ProcessUserSelection detail '{detail.Name}' number {detail.Number}: no anchor part among its {detailItems.Count} parts");
				selectedItems.AddRange(detailItems);
			}
			else
			{
				plugInLogger.LogDebug($"Component with anchor add filtered subset of child parts");
				selectedItems.AddRange(anchorItems);
			}
		}

		/// <summary>
		/// User select bulk selection
		/// </summary>
		/// <returns></returns>
		public List<(Point, List<TS.ModelObject>, List<TS.ModelObject>)> GetBulkSelection(bool selectWholeModel = false, IProgressMessaging progressMessaging = null)
		{
			plugInLogger.LogInformation("GetBulkSelection");
			List<(Point, List<TS.ModelObject>, List<TS.ModelObject>)> selections = new List<(Point, List<TS.ModelObject>, List<TS.ModelObject>)>();
			{
				var myModel = GetTeklaModel();

				TS.ModelObjectEnumerator partsEnumerator = null;


				if (selectWholeModel)
				{
					plugInLogger.LogInformation($"GetBulkSelection - select Whole Model");
					partsEnumerator = myModel.GetModelObjectSelector().GetAllObjects();
				}
				else
				{
					plugInLogger.LogInformation($"GetSelectObjects - ask user for selection");
					partsEnumerator = UserObjectsSelection(TeklaStructuresResources.Properties.Resources.CreateBulkSelection);
				}

				progressMessaging?.SetStageLocalised(1, 0, LocalisedMessage.ModelPostProcess, string.Empty);
				plugInLogger.LogInformation($"GetSelectObjects - process user selection");
				var selectedItems = ProcessUserSelection(partsEnumerator);

				BIM.Common.SorterResult sortedJoints = BulkSelectionHelper.FindJoints(myModel, selectedItems, sorterSettings, plugInLogger);


				plugInLogger.LogInformation($"GetBulkSelection found joints {sortedJoints.Joints.Count}");
				var partsAnyJointHolds = PartsAnyJointHolds(sortedJoints);
				var adoptedPlates = new HashSet<Guid>();
				var exports = new List<JointExport>();
				foreach (var joint in sortedJoints.Joints)
				{
					plugInLogger.LogInformation($"GetBulkSelection joint {joint.Location.X} {joint.Location.Y} {joint.Location.Z}");
					var export = new JointExport(joint);
					exports.Add(export);
					List<TS.ModelObject> beams = export.Beams;
					List<TS.ModelObject> parts = export.Parts;


					var candidates = joint.Members
					.Where(m => !IdentifierHelper.HaunchFilter(m.Parent as TS.Part))
					.Where(m => !IdentifierHelper.AnchorMemberFilter(m.Parent as TS.Part))
					.Where(m => !IdentifierHelper.WasherMemberFilter(m.Parent as TS.Part))
					.Where(m => !IdentifierHelper.NutMemberFilter(m.Parent as TS.Part))
					.Where(m => !IdentifierHelper.ConcreteBlocksFilter(m.Parent as TS.Part))
					.ToList();
					var readings = candidates.Select(m => (Member: m, Reading: ReadPlateProfileMember(m, joint, candidates))).ToList();
					var structuralMembers = readings.Where(r => r.Reading != PlateReading.Plate).Select(r => r.Member).ToList();
					foreach (var reading in readings)
					{
						if (reading.Reading == PlateReading.Plate)
						{
							export.ExportedPlates.Add(((TS.Part)reading.Member.Parent).Identifier.GUID);
						}
						else if (reading.Reading == PlateReading.Member)
						{
							export.MemberPlates.Add((TS.Part)reading.Member.Parent);
						}
					}

					structuralMembers.ForEach(sm => beams.Add(sm.Parent as TS.ModelObject));

					plugInLogger.LogInformation($"GetBulkSelection joint number of members {beams.Count}");

					var stiffenigMembers = joint.Members
					.Where(m => !structuralMembers.Contains(m));

					stiffenigMembers.ToList().ForEach(sm => parts.Add(sm.Parent as TS.ModelObject));

					plugInLogger.LogInformation($"GetBulkSelection joint number of plates {joint.Plates.Count}");
					foreach (var plate in joint.Plates)
					{
						if (plate.Parent is TS.ModelObject tsObject)
						{
							parts.Add(tsObject);
						}
					}

					plugInLogger.LogInformation($"GetBulkSelection joint number of stiffening members {joint.StiffeningMembers.Count}");
					foreach (var stiffeningmember in joint.StiffeningMembers)
					{
						if (stiffeningmember.Parent is TS.ModelObject tsObject)
						{
							parts.Add(tsObject);
						}

						plugInLogger.LogInformation($"GetBulkSelection stiffening member {DescribeMemberShape(stiffeningmember)}");
					}

					plugInLogger.LogInformation($"GetBulkSelection joint number of fasteners {joint.Fasteners.Count}");
					foreach (var jointFastener in joint.Fasteners)
					{
						if (jointFastener.Parent is TS.ModelObject tsObject)
						{
							parts.Add(tsObject);
						}
					}

					parts.AddRange(PlatesAnchorsFastenNoJointHolds(joint, partsAnyJointHolds, adoptedPlates));

					plugInLogger.LogInformation($"GetBulkSelection joint number of welds {joint.Welds.Count}");
					foreach (var jointWeld in joint.Welds)
					{
						if (jointWeld.Parent is TS.ModelObject tsObject)
						{
							parts.Add(tsObject);
						}
					}
				}

				FoldSecondSightingsOfColumnBases(exports);

				foreach (var export in exports.Where(e => !e.FoldedAway))
				{
					selections?.Add(
						(
							new Point(export.Joint.Location.X, export.Joint.Location.Y, export.Joint.Location.Z),
							export.Beams,
							export.Parts
						)
					);
				}
			}
			return selections;
		}

		/// <summary>Applies <see cref="AnchorBoltGroupRule.PlanColumnBaseFolds"/> to the collected exports.</summary>
		private void FoldSecondSightingsOfColumnBases(List<JointExport> exports)
		{
			var plan = AnchorBoltGroupRule.PlanColumnBaseFolds(exports
				.Select(export => new JointPlateReadings(
					export.Beams.Count,
					export.ExportedPlates.ToList(),
					export.MemberPlates.Select(plate => plate.Identifier.GUID).ToList()))
				.ToList());

			foreach (var fold in plan.FoldInto)
			{
				var second = exports[fold.Key];
				var first = exports[fold.Value];
				var known = new HashSet<Guid>(first.Parts.Select(part => part.Identifier.GUID));
				foreach (var part in second.Parts.Concat(second.MemberPlates))
				{
					if (known.Add(part.Identifier.GUID))
					{
						first.Parts.Add(part);
					}
				}

				second.FoldedAway = true;
				plugInLogger.LogInformation($"GetBulkSelection joint at {DescribeLocation(second.Joint)} folded into the joint at {DescribeLocation(first.Joint)}: its structural members, plates {string.Join(", ", second.MemberPlates.Select(plate => plate.Identifier.GUID))}, go out with that joint");
			}

			foreach (var stay in plan.StayMembers)
			{
				var home = exports[stay.Home];
				var plate = home.Parts.First(part => part.Identifier.GUID == stay.Plate);
				home.Parts.RemoveAll(part => part.Identifier.GUID == stay.Plate);
				home.Beams.Add(plate);
				plugInLogger.LogInformation($"GetBulkSelection base plate {stay.Plate} stays a member at {DescribeLocation(home.Joint)}: another joint holds it as a structural member and cannot be folded");
			}
		}

		private static string DescribeLocation(BIM.Common.Joint joint)
			=> $"({joint.Location.X:F0}; {joint.Location.Y:F0}; {joint.Location.Z:F0})";

		/// <summary>What one joint sends to the export, collected before any joint is emitted so a second sighting can be folded.</summary>
		private sealed class JointExport
		{
			public JointExport(BIM.Common.Joint joint)
			{
				Joint = joint;
			}

			public BIM.Common.Joint Joint { get; }

			public List<TS.ModelObject> Beams { get; } = new List<TS.ModelObject>();

			public List<TS.ModelObject> Parts { get; } = new List<TS.ModelObject>();

			/// <summary>Plate-profile members this joint exports as plates.</summary>
			public HashSet<Guid> ExportedPlates { get; } = new HashSet<Guid>();

			/// <summary>Plate-profile members this joint exports as members.</summary>
			public List<TS.Part> MemberPlates { get; } = new List<TS.Part>();

			public bool FoldedAway { get; set; }
		}

		private enum PlateReading
		{
			NotPlateProfile,
			Member,
			Plate,
		}

		/// <summary>How a plate-profile member of the joint goes out, see <see cref="AnchorBoltGroupRule.IsAnchoredPlate"/>.</summary>
		private PlateReading ReadPlateProfileMember(BIM.Common.Member member, BIM.Common.Joint joint, IReadOnlyCollection<BIM.Common.Member> candidates)
		{
			if (!(member.Parent is TS.Beam plate) || !BulkSelectionHelper.IsRectangularCssBeam(plate))
			{
				return PlateReading.NotPlateProfile;
			}

			var facts = new AnchoredPlateFacts
			{
				OtherStructuralMemberLeft = candidates.Any(other => other != member
					&& !(other.Parent is TS.Beam otherBeam && BulkSelectionHelper.IsRectangularCssBeam(otherBeam))),
			};
			string anchor = null;
			(TS.Part Rod, TS.BoltGroup Group)? rod = null;
			var bolts = plate.GetBolts();
			while (bolts.MoveNext())
			{
				if (!(bolts.Current is TS.BoltGroup group) || !IdentifierHelper.FastensOnly(group, plate))
				{
					continue;
				}

				if (IdentifierHelper.AnchorBoltGroupFilter(group))
				{
					facts.BoltGroupAnchor = true;
					anchor = $"bolt group {group.Identifier.GUID}{(Fastens(joint, group) ? string.Empty : " (not among this joint's fasteners)")}";
					break;
				}

				rod = rod ?? IdentifierHelper.RodAnchorParts(group.GetFatherComponent()) ?? IdentifierHelper.RodAnchorParts(plate.GetFatherComponent());
			}

			if (!facts.BoltGroupAnchor && rod.HasValue)
			{
				facts.RodAnchorIsBeam = rod.Value.Rod is TS.Beam;
				facts.RodAnchorInJoint = Holds(joint, rod.Value.Rod);
				facts.RodDetailFirstGroupOnPlate = rod.Value.Group != null && IdentifierHelper.FastensOnly(rod.Value.Group, plate);
				anchor = $"rod {rod.Value.Rod.Identifier.GUID}";
			}

			var reading = AnchorBoltGroupRule.IsAnchoredPlate(facts) ? PlateReading.Plate : PlateReading.Member;
			if (anchor != null)
			{
				plugInLogger.LogInformation($"GetBulkSelection plate-profile member {plate.Identifier.GUID} anchored by {anchor}: {(reading == PlateReading.Plate ? "exported as a plate" : "kept a member")} (rodIsBeam {facts.RodAnchorIsBeam}, rodInJoint {facts.RodAnchorInJoint}, firstGroupOnPlate {facts.RodDetailFirstGroupOnPlate}, otherStructuralMember {facts.OtherStructuralMemberLeft})");
			}

			return reading;
		}

		/// <summary>The plates this joint's anchors fasten that no joint holds, each given to the first joint that asks.</summary>
		private IEnumerable<TS.Part> PlatesAnchorsFastenNoJointHolds(BIM.Common.Joint joint, HashSet<Guid> partsAnyJointHolds, HashSet<Guid> adopted)
		{
			var fastened = new List<TS.Part>();
			foreach (var member in joint.Members.Concat(joint.StiffeningMembers))
			{
				if (member.Parent is TS.Beam rod
					&& IdentifierHelper.AnchorMemberFilter(rod)
					&& IdentifierHelper.RodAnchorParts(rod.GetFatherComponent())?.Group?.PartToBoltTo is TS.Part rodPlate)
				{
					fastened.Add(rodPlate);
				}
			}

			foreach (var fastener in joint.Fasteners)
			{
				if (fastener.Parent is TS.BoltGroup group && IdentifierHelper.AnchorBoltGroupFilter(group) && group.PartToBoltTo is TS.Part groupPlate)
				{
					fastened.Add(groupPlate);
				}
			}

			foreach (var plate in fastened)
			{
				var guid = plate.Identifier.GUID;
				if (partsAnyJointHolds.Contains(guid) || !adopted.Add(guid))
				{
					continue;
				}

				plugInLogger.LogInformation($"GetBulkSelection joint takes the plate its anchor fastens, which no joint reached: {BulkSelectionHelper.Describe(plate)}");
				yield return plate;
			}
		}

		private static HashSet<Guid> PartsAnyJointHolds(BIM.Common.SorterResult sorted)
			=> new HashSet<Guid>(sorted.Joints
				.SelectMany(joint => joint.Members.Cast<BIM.Common.Item>().Concat(joint.StiffeningMembers).Concat(joint.Plates))
				.Select(item => item.Parent as TS.ModelObject)
				.Where(source => source != null)
				.Select(source => source.Identifier.GUID));

		private static bool Holds(BIM.Common.Joint joint, TS.ModelObject part)
			=> joint.Members.Concat(joint.StiffeningMembers)
				.Any(member => (member.Parent as TS.ModelObject)?.Identifier.Equals(part.Identifier) == true);

		private static bool Fastens(BIM.Common.Joint joint, TS.BoltGroup group)
			=> joint.Fasteners.Any(fastener => (fastener.Parent as TS.ModelObject)?.Identifier.Equals(group.Identifier) == true);

		/// <summary>
		/// Which rule made a part detailing. A part reaches <c>Joint.StiffeningMembers</c> either by sitting inside
		/// the node box or by being shorter than it is wide, and the joint alone does not say which - only the second
		/// can be wrong, on a frame stub shorter than its own section depth.
		/// <para>
		/// The verdict is asked of the sorter rather than recomputed here, so the log cannot drift away from the rule.
		/// </para>
		/// </summary>
		private static string DescribeMemberShape(BIM.Common.Member member)
		{
			var profile = (member.Parent as TS.Part)?.Profile?.ProfileString ?? "unknown";
			var span = CI.Geometry3D.GeomOperation.Distance(member.Begin, member.End);

			return $"profile '{profile}' span {span:F0} byShape {BIM.Common.ItemsSorter.IsDetailingByShape(member)}";
		}

		/// <summary>
		/// Get point id
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public string GetPointId(Point point)
		{
			plugInLogger.LogInformation($"GetPointId {point.X.ToString("G", CultureInfo.InvariantCulture)}; {point.Y.ToString("G", CultureInfo.InvariantCulture)}; {point.Z.ToString("G", CultureInfo.InvariantCulture)}");

			return $"{point.X.ToString("G", CultureInfo.InvariantCulture)};{point.Y.ToString("G", CultureInfo.InvariantCulture)};{point.Z.ToString("G", CultureInfo.InvariantCulture)}";
		}

		/// <summary>
		/// get point from id
		/// </summary>
		/// <param name="nodeNo"></param>
		/// <returns></returns>
		public Point GetPoint3D(string nodeNo)
		{
			plugInLogger.LogDebug($"GetPoint3D for id {nodeNo}");

			var coords = nodeNo.Split(';');
			if (coords.Length == 3)
			{
				if (!double.TryParse(coords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
				{
					plugInLogger.LogInformation($"Not unknown coord X {coords[0]}");
					return null;
				}

				if (!double.TryParse(coords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
				{
					plugInLogger.LogInformation($"Not unknown coord Y {coords[1]}");
					return null;
				}

				if (!double.TryParse(coords[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double z))
				{
					plugInLogger.LogInformation($"Not unknown coord Z {coords[2]}");
					return null;
				}

				return new Point(x, y, z);
			}
			else
			{
				plugInLogger.LogInformation($"Not unknown node X {nodeNo}");
				return null;
			}
		}

		/// <summary>
		/// Get Material from database
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Tekla.Structures.Catalogs.MaterialItem GetMaterial(string name)
		{
			plugInLogger.LogInformation($"GetMaterial {name}");
			var mat = new Tekla.Structures.Catalogs.MaterialItem();
			if (mat.Select(name))
			{

				return mat;
			}

			plugInLogger.LogInformation($"GetMaterial not found material {name}");
			return null;
		}

		/// <summary>
		/// Get CrossSection from database
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ProfileItem GetCrossSection(string name)
		{
			plugInLogger.LogInformation($"GetCrossSection {name}");
			var libItem = new LibraryProfileItem();
			if (!libItem.Select(name) || libItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
			{
				ParametricProfileItem paramProfileItem = new ParametricProfileItem();
				if (paramProfileItem.Select(name))
				{
					return paramProfileItem;
				}
				else
				{
					plugInLogger.LogInformation($"GetCrossSection not found {name}");
					return null;
				}
			}
			else
			{
				return libItem;
			}
		}

		/// <summary>
		/// Get Project Name
		/// </summary>
		/// <returns></returns>
		public string GetProjectName()
		{
			plugInLogger.LogDebug($"GetProjectName");
			return Path.GetFileName(GetProjectPath());
		}

		/// <summary>
		/// Check if the current user has write permission on the specified file or directory.
		/// </summary>
		/// <param name="filePath">The path to the file or directory to check.</param>
		/// <returns>True if the user has write permission, false otherwise.</returns>
		public static bool HasWritePermissionOnDir(string filePath)
		{
			try
			{
				AuthorizationRuleCollection rules;

				// Check if the provided path points to a file
				if (File.Exists(filePath))
				{
					// Get the access control list for the file
					FileInfo fileInfo = new FileInfo(filePath);
					FileSecurity fileSecurity = fileInfo.GetAccessControl();

					// Get the access rules for the file
					rules = fileSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
				}
				else // Assume the path points to a directory
				{
					// Get the access control list for the directory
					DirectoryInfo dirInfo = new DirectoryInfo(filePath);
					DirectorySecurity dirSecurity = dirInfo.GetAccessControl();

					// Get the access rules for the directory
					rules = dirSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
				}

				// Get the current user
				var currentUser = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				bool result = false;

				// Iterate through the access rules
				foreach (FileSystemAccessRule rule in rules)
				{
					// Check if the rule grants write permission
					if (0 == (rule.FileSystemRights & (FileSystemRights.WriteData | FileSystemRights.Write)))
					{
						continue; // Skip if the rule does not grant write permission
					}

					// Check if the current user is in the role specified by the rule
					if (rule.IdentityReference.Value.StartsWith("S-1-"))
					{
						// Create a SecurityIdentifier from the rule's identity reference
						var sid = new SecurityIdentifier(rule.IdentityReference.Value);

						// Skip if the current user is not in the role
						if (!currentUser.IsInRole(sid))
						{
							continue;
						}
					}
					else
					{
						// Skip if the current user is not in the role
						if (!currentUser.IsInRole(rule.IdentityReference.Value))
						{
							continue;
						}
					}

					// Determine if the rule grants or denies access
					if (rule.AccessControlType == AccessControlType.Deny)
					{
						return false; // Access is denied
					}
					else if (rule.AccessControlType == AccessControlType.Allow)
					{
						result = true; // Access is allowed
					}
				}

				// Return the final result
				return result;
			}
			catch
			{
				// An exception occurred, return false
				return false;
			}
		}

		/// <summary>
		/// Get path to the user document folder for hashed project
		/// </summary>
		/// <param name="originalPath"></param>
		public string GetBackUpProjectPath(string originalPath)
		{
			plugInLogger.LogDebug("GetBackUpProjectPath");
			var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var backupPath = Path.Combine(docPath, "IdeaStatiCa Projects", GetHashString(originalPath), Path.GetFileName(originalPath));
			plugInLogger.LogInformation($"GetBackUpProjectPath path {backupPath}");
			return backupPath;
		}

		/// <summary>
		/// Calculate hash from string
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		protected static byte[] GetHash(string inputString)
		{
			using (HashAlgorithm algorithm = SHA256.Create())
				return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
		}

		/// <summary>
		/// Calculate hash from string
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		protected static string GetHashString(string inputString)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in GetHash(inputString).Take(8))// Take only the first 8 bytes
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}

		public void CacheCreatedObject(IIdentifier identifier, IIdeaObject createdObject)
		{
			if (GetCachedObject(identifier) == null)
			{
				cachedObjects.Add(identifier, createdObject);
			}
		}

		public IIdeaObject GetCachedObject(IIdentifier identifier)
		{
			if (cachedObjects.TryGetValue(identifier, out IIdeaObject ideaObject))
			{
				return ideaObject;
			}
			else
			{
				return null;
			}
		}

		public void ClearCache()
		{
			cachedObjects.Clear();
		}
	}
}
