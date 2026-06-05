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

		private readonly Dictionary<IIdentifier, IIdeaObject> cachedObjects;

		// Maps node key "X;Y;Z" to the GUID set for that connection point.
		// Populated by GetCadUserSelection; read by ConnectionImporter.Create.
		private readonly Dictionary<string, HashSet<string>> _connectionGuidsByKey = new Dictionary<string, HashSet<string>>();

		// The GUID set for the connection currently being imported.
		private HashSet<string> _currentConnectionGuids;

		public ModelClient(TS.Model teklaModel, IPluginLogger plugInLogger)
		{
			this.teklaModel = teklaModel;
			this.plugInLogger = plugInLogger;
			cachedObjects = new Dictionary<IIdentifier, IIdeaObject>();
		}

		public void RegisterConnectionGuids(string nodeKey, HashSet<string> guids)
		{
			if (_connectionGuidsByKey.TryGetValue(nodeKey, out var existing))
			{
				existing.UnionWith(guids);
				plugInLogger?.LogDebug($"RegisterConnectionGuids: key={nodeKey} merged to {existing.Count} guids");
			}
			else
			{
				_connectionGuidsByKey[nodeKey] = new HashSet<string>(guids);
				plugInLogger?.LogDebug($"RegisterConnectionGuids: key={nodeKey} guids={guids.Count}");
			}
		}

		public void SetCurrentConnectionGuidsByKey(string nodeKey)
		{
			if (_connectionGuidsByKey.TryGetValue(nodeKey, out var guids))
			{
				_currentConnectionGuids = guids;
				plugInLogger?.LogDebug($"SetCurrentConnectionGuidsByKey: key={nodeKey} guids={guids.Count}");
			}
			else
			{
				_currentConnectionGuids = null;
				plugInLogger?.LogDebug($"SetCurrentConnectionGuidsByKey: key={nodeKey} not found, no filter");
			}
		}

		public void SetCurrentConnectionGuids(HashSet<string> guids)
		{
			_currentConnectionGuids = guids;
			plugInLogger?.LogDebug($"SetCurrentConnectionGuids: {guids.Count} guids set for current connection");
		}

		public bool IsInCurrentConnection(string guid)
		{
			return _currentConnectionGuids == null || _currentConnectionGuids.Contains(guid);
		}

		public bool IsRegisteredInAnyConnection(string guid)
		{
			return _connectionGuidsByKey.Values.Any(set => set.Contains(guid));
		}

		public bool IsInSameConnectionAs(string boltGridGuid, string partGuid)
		{
			if (_currentConnectionGuids == null)
			{
				return true;
			}

			if (_currentConnectionGuids.Contains(partGuid))
			{
				return true;
			}

			// Accept parts that are not registered in ANY connection — they were not picked up
			// by the sorter (e.g. gusset plates on diagonals whose centroid fell outside all
			// node BBs). They belong here because they belong nowhere else.
			bool registeredElsewhere = _connectionGuidsByKey.Values.Any(set => set.Contains(partGuid));
			return !registeredElsewhere;
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
					else if (part is TS.Beam b && !IdentifierHelper.WasherMemberFilter(part) && !IdentifierHelper.NutMemberFilter(part))
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
				plugInLogger.LogDebug($"Standard component add all child items");
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
		public List<(Point, List<TS.ModelObject>, List<TS.ModelObject>)> GetBulkSelection(bool selectWholeModel = false, IProgressMessaging progressMessaging = null, BIM.Common.SorterSettings sorterSettings = null)
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

				BIM.Common.SorterResult sortedJoints = BulkSelectionHelper.FindJoints(myModel, selectedItems, plugInLogger, sorterSettings);


				plugInLogger.LogInformation($"GetBulkSelection found joints {sortedJoints.Joints.Count}");
				foreach (var joint in sortedJoints.Joints)
				{
					plugInLogger.LogInformation($"GetBulkSelection joint {joint.Location.X} {joint.Location.Y} {joint.Location.Z}");
					List<TS.ModelObject> beams = new List<TS.ModelObject>();
					List<TS.ModelObject> parts = new List<TS.ModelObject>();


					var structuralMembers = joint.Members
					.Where(m => !IdentifierHelper.HaunchFilter(m.Parent as TS.Part))
					.Where(m => !IdentifierHelper.AnchorMemberFilter(m.Parent as TS.Part))
					.Where(m => !IdentifierHelper.WasherMemberFilter(m.Parent as TS.Part))
					.Where(m => !IdentifierHelper.NutMemberFilter(m.Parent as TS.Part))
					.Where(m => !IdentifierHelper.ConcreteBlocksFilter(m.Parent as TS.Part));

					structuralMembers.ToList().ForEach(sm => beams.Add(sm.Parent as TS.ModelObject));

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
					}

					plugInLogger.LogInformation($"GetBulkSelection joint number of fasteners {joint.Fasteners.Count}");

					// Build GUIDs of members and plates belonging to this joint for membership check.
					var jointParentGuids = joint.Members
						.Concat(joint.Plates.OfType<BIM.Common.Item>())
						.Concat(joint.StiffeningMembers.OfType<BIM.Common.Item>())
						.Select(item => (item.Parent as TS.ModelObject)?.Identifier.GUID.ToString())
						.Where(g => g != null)
						.ToHashSet();

					foreach (var jointFastener in joint.Fasteners)
					{
						if (jointFastener.Parent is TS.ModelObject tsObject)
						{
							if (sortedJoints.Joints.Count > 1 && tsObject is TS.BoltGroup boltGroup)
							{
								// Collect connected part GUIDs of this bolt.
								var connectedGuids = new[]
								{
									boltGroup.PartToBoltTo?.Identifier.GUID.ToString(),
									boltGroup.PartToBeBolted?.Identifier.GUID.ToString(),
								}
								.Concat(boltGroup.OtherPartsToBolt?.OfType<TS.Part>().Select(p => p.Identifier.GUID.ToString()) ?? Enumerable.Empty<string>())
								.Where(g => g != null)
								.ToHashSet();

										var fastenerOrigin = jointFastener.LCS.Origin;
								var distToThis = Math.Sqrt(
									Math.Pow(fastenerOrigin.X - joint.Location.X, 2) +
									Math.Pow(fastenerOrigin.Y - joint.Location.Y, 2) +
									Math.Pow(fastenerOrigin.Z - joint.Location.Z, 2));

								// Priority 1: bolt has a PLATE connected part that is exclusive to this joint
								// (not present in any other joint). ENDPLATE bolts are assigned this way.
								bool hasExclusivePlatePart = connectedGuids.Any(g =>
								{
									if (!jointParentGuids.Contains(g)) return false;
									// Must be a plate (not just a member) to qualify as exclusive anchor.
									var isPlate = joint.Plates.Any(p => (p.Parent as TS.ModelObject)?.Identifier.GUID.ToString() == g);
									if (!isPlate) return false;
									return sortedJoints.Joints.Where(j => j != joint).All(j =>
									{
										var otherPlateGuids = j.Plates
											.Select(p => (p.Parent as TS.ModelObject)?.Identifier.GUID.ToString())
											.Where(og => og != null);
										return !otherPlateGuids.Contains(g);
									});
								});

								if (hasExclusivePlatePart)
								{
									// Bolt is anchored to an exclusive plate of this joint — accept regardless of distance.
								}
								else
								{
									// Priority 2: closest joint by distance.
									var isClosest = sortedJoints.Joints.All(j =>
									{
										var d = Math.Sqrt(
											Math.Pow(fastenerOrigin.X - j.Location.X, 2) +
											Math.Pow(fastenerOrigin.Y - j.Location.Y, 2) +
											Math.Pow(fastenerOrigin.Z - j.Location.Z, 2));
										return distToThis <= d;
									});
									if (!isClosest)
									{
										plugInLogger.LogDebug($"GetBulkSelection joint [{joint.Location.X:F0},{joint.Location.Y:F0},{joint.Location.Z:F0}] SKIPPED fastener {tsObject.Identifier.GUID} (no exclusive plate part, closer to another joint)");
										continue;
									}
								}

								plugInLogger.LogDebug($"GetBulkSelection joint [{joint.Location.X:F0},{joint.Location.Y:F0},{joint.Location.Z:F0}] KEPT fastener {tsObject.Identifier.GUID}");
							}
							parts.Add(tsObject);
						}
					}

					plugInLogger.LogInformation($"GetBulkSelection joint number of welds {joint.Welds.Count}");
					foreach (var jointWeld in joint.Welds)
					{
						if (jointWeld.Parent is TS.ModelObject tsObject)
						{
							parts.Add(tsObject);
						}
					}

					// Skip orphan joints: no operations of their own (no plates, no welds)
					// and all their members are already covered by another joint that does have operations.
					if (joint.Plates.Count == 0 && joint.Welds.Count == 0 && sortedJoints.Joints.Count > 1)
					{
						var memberGuids = new HashSet<string>(
							joint.Members.Select(m => (m.Parent as TS.ModelObject)?.Identifier.GUID.ToString())
							.Where(g => g != null));

						var isSubsetOfRicherJoint = sortedJoints.Joints
							.Where(other => other != joint && (other.Plates.Count > 0 || other.Welds.Count > 0))
							.Any(other =>
							{
								var otherGuids = other.Members
									.Select(m => (m.Parent as TS.ModelObject)?.Identifier.GUID.ToString())
									.Where(g => g != null);
								return memberGuids.IsSubsetOf(otherGuids);
							});

						if (isSubsetOfRicherJoint)
						{
							plugInLogger.LogInformation($"GetBulkSelection SKIPPED orphan joint [{joint.Location.X:F0},{joint.Location.Y:F0},{joint.Location.Z:F0}] (no operations, members covered by richer joint)");
							continue;
						}
					}

					selections?.Add(
						(
							new Point(joint.Location.X, joint.Location.Y, joint.Location.Z),
							beams,
							parts
						)
					);
				}

				// Fallback: BoltGroups not assigned to any joint by the sorter (their origin fell
				// outside all node BBs) are reassigned to the joint whose member is their PartToBoltTo
				// or PartToBeBolted.
				if (sortedJoints.Joints.Count > 0)
				{
					var assignedFastenerGuids = new HashSet<string>(
						sortedJoints.Joints.SelectMany(j => j.Fasteners)
							.Select(f => (f.Parent as TS.ModelObject)?.Identifier.GUID.ToString())
							.Where(g => g != null));

					var unassignedBolts = selectedItems
						.OfType<TS.BoltGroup>()
						.Where(bg => !assignedFastenerGuids.Contains(bg.Identifier.GUID.ToString()))
						.ToList();
					plugInLogger.LogDebug($"GetBulkSelection fallback: {unassignedBolts.Count} unassigned BoltGroups from {selectedItems.Count(o => o is TS.BoltGroup)} total");

					foreach (var bolt in unassignedBolts)
					{
						var boltGuid = bolt.Identifier.GUID.ToString();
						var connectedGuids = new[]
						{
							bolt.PartToBoltTo?.Identifier.GUID.ToString(),
							bolt.PartToBeBolted?.Identifier.GUID.ToString(),
						}
						.Concat(bolt.OtherPartsToBolt?.OfType<TS.Part>().Select(p => p.Identifier.GUID.ToString()) ?? Enumerable.Empty<string>())
						.Where(g => g != null)
						.ToHashSet();

						// Find the selection whose PLATES (parts) overlap with the bolt's connected parts.
						// Prefer plate match over member match to avoid assigning shared-column bolts
						// to the wrong joint.
						var targetIdx = selections?.FindIndex(sel =>
							sel.Item3.Any(p => connectedGuids.Contains(p.Identifier.GUID.ToString())));

						// Fallback to member match only if no plate match found.
						if (!targetIdx.HasValue || targetIdx.Value < 0)
						{
							targetIdx = selections?.FindIndex(sel =>
								sel.Item2.Any(b => connectedGuids.Contains(b.Identifier.GUID.ToString())));
						}

						if (targetIdx.HasValue && targetIdx.Value >= 0)
						{
							var pt = selections[targetIdx.Value].Item1;
							selections[targetIdx.Value].Item3.Add(bolt);
							plugInLogger.LogDebug($"GetBulkSelection fallback: unassigned BoltGroup {boltGuid} added to joint [{pt.X:F0},{pt.Y:F0},{pt.Z:F0}]");
						}
						else
						{
							plugInLogger.LogDebug($"GetBulkSelection fallback: unassigned BoltGroup {boltGuid} NOT matched to any joint (connectedGuids={string.Join(",", connectedGuids.Select(g => g.Length > 8 ? g.Substring(0, 8) : g))})");
						}
					}
				}
			}
			return selections;
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
			_currentConnectionGuids = null;
			_connectionGuidsByKey.Clear();
		}
	}
}
