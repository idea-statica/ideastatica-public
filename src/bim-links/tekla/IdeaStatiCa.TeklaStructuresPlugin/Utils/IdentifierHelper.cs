using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.TeklaStructuresPlugin.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using TS = Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utils
{
	internal static class IdentifierHelper
	{
		// rename parameter addToCollection
		internal static List<IIdentifier> GetIdentifier(ModelObject teklaObject, ref List<IIdentifier> identifiers, bool addToCollection = true, Point connectionPoint = null)
		{
			if (teklaObject is TS.Beam beamPart
				&& StiffeningMemberFilter(beamPart)
				&& !AnchorMemberFilter(beamPart) //in not anchor
				&& !BulkSelectionHelper.IsRectangularCssBeam(beamPart))
			{
				var result = identifiers.Exists(x => x is ConnectedMemberIdentifier<IIdeaConnectedMember> cmId && cmId.GetId().ToString() == beamPart.Identifier.GUID.ToString());
				if (!result)
				{
					if (addToCollection)
					{
						identifiers.Add(new ConnectedMemberIdentifier<IIdeaConnectedMember>(beamPart.Identifier.GUID.ToString()));
					}
				}
			}
			else if (teklaObject is TS.PolyBeam polybeamPart)
			{
				var result = identifiers.Exists(x => x is ConnectedMemberIdentifier<IIdeaConnectedMember> cmId && cmId.GetId().ToString() == polybeamPart.Identifier.GUID.ToString());
				if (!result)
				{
					if (addToCollection)
					{
						identifiers.Add(new ConnectedMemberIdentifier<IIdeaConnectedMember>(polybeamPart.Identifier.GUID.ToString()));
					}
				}
			}
			else if (teklaObject is TS.ContourPlate plate)
			{
				AddIdentifier<IIdeaPlate>(identifiers, teklaObject, plate.Identifier.GUID.ToString());
			}
			else if ((teklaObject is TS.Beam beamAsPlate))
			{

				if (AnchorMemberFilter(beamAsPlate))
				{
					AddIdentifier<IIdeaAnchorGrid>(identifiers, teklaObject, teklaObject.Identifier.GUID.ToString());
					AddConcreteBlockToAnchor(identifiers);
				}
				else if (ConcreteBlocksFilter(beamAsPlate))
				{
					AddIdentifier<IIdeaConcreteBlock>(identifiers, teklaObject, beamAsPlate.Identifier.GUID.ToString());
					AddConcreteBlockToAnchor(identifiers);
				}
				else if (StiffeningMemberFilter(beamAsPlate) && addToCollection && BulkSelectionHelper.IsRectangularCssBeam(beamAsPlate))
				{
					AddIdentifier<IIdeaPlate>(identifiers, teklaObject, beamAsPlate.Identifier.GUID.ToString());
				}


			}
			if (teklaObject is BoltGroup boltGroupObject)
			{
				if (AnchorBoltGroupFilter(boltGroupObject))
				{
					AddIdentifier<IIdeaAnchorGrid>(identifiers, teklaObject, teklaObject.Identifier.GUID.ToString());
					AddConcreteBlockToAnchor(identifiers);
				}
				else
				{
					AddIdentifier<IIdeaBoltGrid>(identifiers, teklaObject, teklaObject.Identifier.GUID.ToString());
				}
			}
			else if (teklaObject is BaseWeld)
			{
				AddIdentifier<IIdeaWeld>(identifiers, teklaObject, teklaObject.Identifier.GUID.ToString());
			}
			else if (teklaObject is TS.BentPlate bentPlate)
			{
				AddIdentifier<IIdeaFoldedPlate>(identifiers, teklaObject, bentPlate.Identifier.GUID.ToString());
			}
			else if (teklaObject is TS.CutPlane cutPlane)
			{
				AddIdentifier<IIdeaCut>(identifiers, teklaObject, cutPlane.Identifier.GUID.ToString());
			}
			else if (teklaObject is TS.Fitting fitting)
			{
				AddIdentifier<IIdeaCut>(identifiers, teklaObject, fitting.Identifier.GUID.ToString());
			}
			else if (teklaObject is TS.BooleanPart booleanPart && booleanPart.Type == TS.BooleanPart.BooleanTypeEnum.BOOLEAN_CUT)
			{
				AddIdentifier<IIdeaCut>(identifiers, teklaObject, booleanPart.Identifier.GUID.ToString());
			}


			if (teklaObject is Part teklaPart)
			{
				var beamChildrenEnumerator = teklaPart.GetChildren();
				while (beamChildrenEnumerator.MoveNext())
				{
					var teklaChildObject = beamChildrenEnumerator.Current;

					//check workplane

					if (teklaChildObject is CutPlane cutPlane)
					{
						if (!IsWorkPlaneInSphereOfConnection(connectionPoint, teklaPart, cutPlane.Plane))
						{
							//skip cut its on other side of member
							continue;
						}

						identifiers = GetIdentifier(teklaChildObject, ref identifiers, addToCollection, connectionPoint);
					}
					else if (teklaChildObject is Fitting fitting)
					{
						if (!IsWorkPlaneInSphereOfConnection(connectionPoint, teklaPart, fitting.Plane))
						{
							//skip cut its on other side of member
							continue;
						}

						identifiers = GetIdentifier(teklaChildObject, ref identifiers, addToCollection, connectionPoint);
					}
					else if (teklaChildObject is BooleanPart booleanPart)
					{
						if (booleanPart.OperativePart is Part boolPart)
						{
							var originalCenterline = boolPart.GetCenterLine(false);

							if (!IsPointInShereOfConnection(connectionPoint, teklaPart, originalCenterline[0] as Point)
								&& !IsPointInShereOfConnection(connectionPoint, teklaPart, originalCenterline[originalCenterline.Count - 1] as Point)
								)
							{
								//skip cut its on otherside of member
								continue;
							}

							identifiers = GetIdentifier(teklaChildObject, ref identifiers, addToCollection, connectionPoint);

						}
					}
				}

				//bolts
				var bolts = teklaPart.GetBolts();
				while (bolts.MoveNext())
				{
					var modelObj = bolts.Current;

					if (!(modelObj is TS.BoltGroup boltGroup))
					{
						continue;
					}

					var boltCs = boltGroup.GetCoordinateSystem();

					if (!IsPointNearOfConnection(connectionPoint, teklaPart, boltCs.Origin as Point))
					{
						//not skip for Stiffening Member as plate 
						if (!(StiffeningMemberFilter(teklaPart) && addToCollection && BulkSelectionHelper.IsRectangularCssBeam(teklaPart)))
						{
							//skip item too far from connection point
							continue;
						}
					}

					// A group that fastens this part alone is never walked: such a group enters an export only through
					// the joint's fasteners, so an anchor of that shape is never added twice.
					if (!FastensOnly(boltGroup, teklaPart))
					{
						identifiers = GetIdentifier(modelObj, ref identifiers, addToCollection, connectionPoint);
					}
				}

				//welds
				var welds = teklaPart.GetWelds();
				while (welds.MoveNext())
				{
					var modelObj = welds.Current;

					if (!(modelObj is TS.BaseWeld weld))
					{
						continue;
					}
					var weldCs = weld.GetCoordinateSystem();
					if (!IsPointNearOfConnection(connectionPoint, teklaPart, weldCs.Origin as Point))
					{
						//not skip for Stiffening Member as plate 
						if (!(StiffeningMemberFilter(teklaPart) && addToCollection && BulkSelectionHelper.IsRectangularCssBeam(teklaPart)))
						{
							//skip item too far from connection point
							continue;
						}
					}

					identifiers = GetIdentifier(modelObj, ref identifiers, addToCollection, connectionPoint);
				}
			}

			return identifiers;
		}

		/// <summary>
		/// CAD re-sync discovery. From the joint's known member parts, follows near-joint welds and bolts to the sibling
		/// parts they connect (an added stiffener plate is welded/bolted to a member, so it is reachable this way) and
		/// classifies every reached object with <see cref="GetIdentifier"/>. Transitive (a plate welded to a plate welded
		/// to a member is still reached) and bounded by the same near-joint proximity filter the per-member walk uses, so
		/// it stays local to this joint. Seed members are walked with addToCollection:false — their own cuts/bolts/welds
		/// are re-classified but the member itself is not re-added as a plate — mirroring the import member walk.
		/// </summary>
		internal static List<IIdentifier> GetConnectedIdentifiers(IModelClient model, IEnumerable<string> memberHandles, Point connectionPoint)
		{
			var identifiers = new List<IIdentifier>();
			if (model == null || memberHandles == null || connectionPoint == null)
			{
				return identifiers;
			}

			var visited = new HashSet<string>();
			var queue = new Queue<(Part part, bool isSeedMember)>();
			foreach (string handle in memberHandles)
			{
				if (string.IsNullOrEmpty(handle) || !visited.Add(handle))
				{
					continue;
				}

				if (model.GetItemByHandler(handle) is Part memberPart)
				{
					queue.Enqueue((memberPart, true));
				}
			}

			while (queue.Count > 0)
			{
				var (part, isSeedMember) = queue.Dequeue();

				// Seed members: classify their own children only (addToCollection:false → the member is not re-added as a
				// plate), like the import member walk. Discovered siblings: full classification (adds the stiffener plate).
				identifiers = GetIdentifier(part, ref identifiers, !isSeedMember, connectionPoint);

				foreach (Part sibling in GetConnectedSiblingParts(part, connectionPoint))
				{
					if (sibling != null && visited.Add(sibling.Identifier.GUID.ToString()))
					{
						queue.Enqueue((sibling, false));
					}
				}
			}

			return identifiers;
		}

		/// <summary>
		/// Sibling parts connected to <paramref name="part"/> through its near-joint welds and bolts. Only welds/bolts
		/// whose coordinate-system origin is near the connection (the same filter <see cref="GetIdentifier"/> applies to a
		/// member's own welds/bolts) are followed, so the connectivity walk does not leak into a neighbouring joint.
		/// </summary>
		private static IEnumerable<Part> GetConnectedSiblingParts(Part part, Point connectionPoint)
		{
			var welds = part.GetWelds();
			while (welds.MoveNext())
			{
				if (!(welds.Current is BaseWeld weld))
				{
					continue;
				}

				var origin = weld.GetCoordinateSystem()?.Origin as Point;
				if (origin == null || !IsPointNearOfConnection(connectionPoint, part, origin))
				{
					continue;
				}

				if (weld.MainObject is Part main && !main.Identifier.Equals(part.Identifier))
				{
					yield return main;
				}

				if (weld.SecondaryObject is Part secondary && !secondary.Identifier.Equals(part.Identifier))
				{
					yield return secondary;
				}
			}

			var bolts = part.GetBolts();
			while (bolts.MoveNext())
			{
				if (!(bolts.Current is BoltGroup boltGroup))
				{
					continue;
				}

				var origin = boltGroup.GetCoordinateSystem()?.Origin as Point;
				if (origin == null || !IsPointNearOfConnection(connectionPoint, part, origin))
				{
					continue;
				}

				foreach (Part boltedPart in ConnectedBoltParts(boltGroup))
				{
					if (!boltedPart.Identifier.Equals(part.Identifier))
					{
						yield return boltedPart;
					}
				}
			}
		}

		private static IEnumerable<Part> ConnectedBoltParts(BoltGroup boltGroup)
		{
			if (boltGroup.PartToBoltTo is Part boltTo)
			{
				yield return boltTo;
			}

			if (boltGroup.PartToBeBolted is Part beBolted)
			{
				yield return beBolted;
			}

			if (boltGroup.OtherPartsToBolt != null)
			{
				foreach (var other in boltGroup.OtherPartsToBolt)
				{
					if (other is Part otherPart)
					{
						yield return otherPart;
					}
				}
			}
		}

		private static void AddConcreteBlockToAnchor(List<IIdentifier> identifiers)
		{
			var concreteBlock = identifiers.Find(id => id is StringIdentifier<IIdeaConcreteBlock>) as StringIdentifier<IIdeaConcreteBlock>;

			if (concreteBlock == null)
			{
				return;
			}

			for (int i = 0; i < identifiers.Count; i++)
			{
				if (identifiers[i] is StringIdentifier<IIdeaAnchorGrid> anchorGridId)
				{
					var existingIds = anchorGridId.Id.Split(';');

					// Check if the concrete block ID is already registered
					if (!existingIds.Contains(concreteBlock.Id))
					{
						// Append the new ID, ensuring no duplicate delimiters
						identifiers[i] = new StringIdentifier<IIdeaAnchorGrid>(string.Join(";", existingIds.Append(concreteBlock.Id)));
					}
				}
			}
		}

		private static bool IsWorkPlaneInSphereOfConnection(Point connectionPoint, Part beam, Plane plane)
		{
			var originalCenterline = beam.GetCenterLine(false);
			var tsWorkplanePoint = plane.Origin;
			var tsWokplanePointHitByReferenceLine = Projection.LineToPlane(
				new Line(originalCenterline[0] as Point, originalCenterline[originalCenterline.Count - 1] as Point),
				new GeometricPlane(tsWorkplanePoint, plane.AxisX, plane.AxisY)
				);
			var potentialIntersecPoint = Projection.PointToLine(connectionPoint, tsWokplanePointHitByReferenceLine);


			// Check if any coordinate of the potential intersection point is NaN
			if (double.IsNaN(potentialIntersecPoint.X) ||
				double.IsNaN(potentialIntersecPoint.Y) ||
				double.IsNaN(potentialIntersecPoint.Z))
			{
				// Assume the plate is out of the sphere
				return IsIntersectPointInSphereOfConnection(connectionPoint, beam, tsWokplanePointHitByReferenceLine.Origin);
			}

			// Calculate lengths
			var partLen = Distance.PointToPoint(
				originalCenterline[0] as Point,
				originalCenterline[originalCenterline.Count - 1] as Point);
			var projectedLen = Distance.PointToPoint(potentialIntersecPoint, tsWorkplanePoint);

			var intersectionOrigin = (partLen / 2 < projectedLen)
				? tsWokplanePointHitByReferenceLine.Origin
				: potentialIntersecPoint;

			return IsIntersectPointInSphereOfConnection(connectionPoint, beam, intersectionOrigin);
		}

		private static bool IsPointInShereOfConnection(Point connectionPoint, Part beam, Point workPlanePoint)
		{
			var cuttedCenterline = beam.GetCenterLine(true);
			var originalCenterline = beam.GetCenterLine(false);

			var tsWorkplanePoint = Projection.PointToLine(workPlanePoint, new Line(originalCenterline[0] as Point, cuttedCenterline[cuttedCenterline.Count - 1] as Point));

			return IsIntersectPointInSphereOfConnection(connectionPoint, beam, tsWorkplanePoint);
		}

		private static bool IsPointNearOfConnection(Point connectionPoint, Part beam, Point nearPoint)
		{
			var distanceToNearPoint = Distance.PointToPoint(connectionPoint, nearPoint);
			var cuttedCenterline = beam.GetCenterLine(true);
			// 1/4 of len member
			var beamLen = Distance.PointToPoint(cuttedCenterline[0] as Point, cuttedCenterline[cuttedCenterline.Count - 1] as Point);


			if (distanceToNearPoint.IsLesserOrEqual(beamLen / 4))
			{
				//skip cut its on otherside of member
				return true;
			}
			else
			{
				return false;
			}
		}

		private static bool IsIntersectPointInSphereOfConnection(Point connectionPoint, Part beam, Point workPlanePoint)
		{
			var cuttedCenterline = beam.GetCenterLine(true);
			var originalCenterline = beam.GetCenterLine(false);

			//cuttedCenterline[0] as TSG.Point
			var tsCpPoint = connectionPoint;
			var tsWorkplanePoint = workPlanePoint;

			var distanceToStart = Distance.PointToPoint(cuttedCenterline[0] as Point, tsCpPoint);
			var distanceToEnd = Distance.PointToPoint(cuttedCenterline[cuttedCenterline.Count - 1] as Point, tsCpPoint);

			var distanceToWorkPlane = Distance.PointToPoint(tsWorkplanePoint, tsCpPoint);
			var minDistance = Math.Min(distanceToEnd, distanceToStart);

			string strProfName = beam.Profile.ProfileString;

			LibraryProfileItem profileItem = new LibraryProfileItem();
			profileItem.Select(strProfName);
			var cssProperties = CssFactoryHelper.GetCssProperties(profileItem);
			var cssSize = 0.0;
			if (cssProperties.ContainsKey(CssFactoryHelper.TubeDiameterKey))
			{
				cssSize = (double)cssProperties[CssFactoryHelper.TubeDiameterKey];
			}
			else if ((cssProperties.ContainsKey(CssFactoryHelper.HeightKey)) && (cssProperties.ContainsKey(CssFactoryHelper.WidthKey)))
			{
				cssSize = Math.Max((double)cssProperties[CssFactoryHelper.HeightKey], (double)cssProperties[CssFactoryHelper.WidthKey]);
			}

			//extend cssSize by 1/3 of len member
			var len = Distance.PointToPoint(cuttedCenterline[0] as Point, cuttedCenterline[cuttedCenterline.Count - 1] as Point);
			if (beam.Name == BulkSelectionHelper.HaunchMemberName)
			{
				minDistance += len;
			}
			else
			{
				minDistance += len / 3;
			}

			if (minDistance.IsLesserOrEqual(distanceToWorkPlane - cssSize))
			{
				//skip cut its on otherside of member
				return false;
			}
			else
			{
				return true;
			}
		}
		private static bool StiffeningMemberFilter(Part beam)
		{

			//skip anchor member
			if (NutMemberFilter(beam))
			{
				return false;
			}

			//skip concrete blocks
			if (ConcreteBlocksFilter(beam))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Concrete Blocks Filter
		/// </summary>
		/// <param name="beam"></param>
		/// <returns></returns>
		public static bool ConcreteBlocksFilter(Part beam)
		{
			//find concrete blocks
			return beam is TS.Beam b && (b.Type == TS.Beam.BeamTypeEnum.PAD_FOOTING || b.Type == TS.Beam.BeamTypeEnum.STRIP_FOOTING);
		}

		/// <summary>
		/// Haunch Filter
		/// </summary>
		/// <param name="beam"></param>
		/// <returns></returns>
		public static bool HaunchFilter(Part beam)
		{
			//find concrete blocks
			return beam is TS.Beam b && (b.Name == "HAUNCH");
		}

		/// <summary>
		/// Grout plate Filter
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool GroutFilter(Part part)
		{
			return part is TS.ContourPlate cp && part.Name == "GROUT";
		}

		/// <summary>
		/// Cast plate Filter
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool CastPlateFilter(Part part)
		{
			return part is TS.ContourPlate cp && part.Name == "CAST_PLATE";
		}



		/// <summary>
		/// Anchor Member Filter
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool AnchorMemberFilter(Part part)
		{
			// anchor member
			// TEKLA_CUSTOM_OBJECTS is declared by every version whose API has GetCustomObjectType, and it is declared
			// in the csproj rather than listed here: adding a Tekla version starts by copying the previous csproj, and
			// a version-numbered condition in this file is what a new version silently fails to satisfy.
#if TEKLA_CUSTOM_OBJECTS
			if (part.GetCustomObjectType() == "AnchorBolt")
			{
				return true;
			}
#endif
			return part.Name == "ANCHOR ROD";
		}

		/// <summary>
		/// Whether a bolt group is an anchor modelled as bolts, see <see cref="AnchorBoltGroupRule"/>.
		/// </summary>
		internal static bool AnchorBoltGroupFilter(BoltGroup boltGroup) => JudgeAnchorBoltGroup(boltGroup).IsAnchor;

		/// <summary>
		/// Reads the facts <see cref="AnchorBoltGroupRule"/> judges a bolt group on, and stops at the first one that already
		/// rules it out: the slots cost nothing, the detail's children, the plate's other groups and the geometry do.
		/// </summary>
		internal static AnchorBoltGroupVerdict JudgeAnchorBoltGroup(BoltGroup boltGroup)
		{
			var plate = boltGroup.PartToBoltTo as Part;
			var facts = new AnchorBoltGroupFacts
			{
				IsBolt = boltGroup.Bolt,
				FastensOnePartOnly = plate != null && FastensOnly(boltGroup, plate),
			};
			if (!facts.FastensOnePartOnly)
			{
				return AnchorBoltGroupRule.Judge(facts);
			}

			facts.PartIsPlate = BulkSelectionHelper.IsFlatPlate(plate);
			if (!facts.PartIsPlate)
			{
				return AnchorBoltGroupRule.Judge(facts);
			}

			facts.InRodAnchorDetail = IsRodAnchorDetail(boltGroup.GetFatherComponent()) || IsRodAnchorDetail(plate.GetFatherComponent());
			if (facts.InRodAnchorDetail)
			{
				return AnchorBoltGroupRule.Judge(facts);
			}

			if (!facts.IsBolt)
			{
				facts.PositionCount = boltGroup.BoltPositions?.Count ?? 0;
				facts.OtherAnchorGroupsOnPlate = facts.PositionCount < 2 ? 0 : OtherAnchorGroupsOn(plate, boltGroup);
				facts.DetailPrimaryId = facts.PositionCount < 2 || facts.OtherAnchorGroupsOnPlate > 0
					? null
					: ((boltGroup.GetFatherComponent() as Detail)?.GetPrimaryObject() as ModelObject)?.Identifier.GUID.ToString();
				if (string.IsNullOrEmpty(facts.DetailPrimaryId))
				{
					return AnchorBoltGroupRule.Judge(facts);
				}
			}

			var boltAxis = BulkSelectionHelper.BoltFrame(boltGroup).Z;
			facts.BoltAxis = new CI.Geometry3D.Vector3D(boltAxis.X, boltAxis.Y, boltAxis.Z);

			var solid = plate.GetSolid();
			var min = solid.MinimumPoint;
			var max = solid.MaximumPoint;
			facts.PlateMin = new CI.Geometry3D.Point3D(min.X, min.Y, min.Z);
			facts.PlateMax = new CI.Geometry3D.Point3D(max.X, max.Y, max.Z);
			facts.PlateThickness = Math.Min(Math.Min(max.X - min.X, max.Y - min.Y), max.Z - min.Z);
			facts.WeldedMembers = WeldedFrameMembers(plate);

			return AnchorBoltGroupRule.Judge(facts);
		}

		/// <summary>
		/// Groups on <paramref name="plate"/> other than <paramref name="group"/> that fasten it alone and could be its
		/// anchors - bolts, or two holes and more. Holes stand in for anchors only where no such group competes.
		/// </summary>
		private static int OtherAnchorGroupsOn(Part plate, BoltGroup group)
		{
			var count = 0;
			var bolts = plate.GetBolts();
			while (bolts.MoveNext())
			{
				if (bolts.Current is BoltGroup other
					&& !other.Identifier.Equals(group.Identifier)
					&& FastensOnly(other, plate)
					&& (other.Bolt || (other.BoltPositions?.Count ?? 0) >= 2))
				{
					count++;
				}
			}

			return count;
		}

		/// <summary>
		/// Whether a bolt group names <paramref name="part"/> in both bolting slots and no other part. This is the one
		/// shape the part walk in <see cref="GetIdentifier"/> never follows, and the only shape an anchor modelled as bolts
		/// can have - the two share this definition so they cannot drift apart.
		/// </summary>
		internal static bool FastensOnly(BoltGroup boltGroup, Part part)
			=> boltGroup.PartToBoltTo is Part boltTo
				&& boltGroup.PartToBeBolted is Part beBolted
				&& boltTo.Identifier.Equals(part.Identifier)
				&& beBolted.Identifier.Equals(part.Identifier)
				&& (boltGroup.OtherPartsToBolt == null || boltGroup.OtherPartsToBolt.Count == 0);

		/// <summary>A detail whose rods and bolt group make an anchor grid of their own.</summary>
		internal static bool IsRodAnchorDetail(BaseComponent component) => RodAnchorParts(component)?.Group != null;

		/// <summary>
		/// A detail's first rod part and first bolt group, the pair its anchor grid is built from. Null when the component
		/// is not a detail with a rod; the group is null when the detail has none.
		/// </summary>
		internal static (Part Rod, BoltGroup Group)? RodAnchorParts(BaseComponent component)
		{
			if (!(component is Detail detail))
			{
				return null;
			}

			Part rod = null;
			BoltGroup group = null;
			var children = detail.GetChildren();
			while (children.MoveNext())
			{
				if (rod == null && children.Current is Part part && AnchorMemberFilter(part))
				{
					rod = part;
				}
				else if (group == null && children.Current is BoltGroup bolts)
				{
					group = bolts;
				}
			}

			if (rod == null)
			{
				return null;
			}

			return (rod, group);
		}

		private static List<AnchorBoltGroupFacts.WeldedMember> WeldedFrameMembers(Part plate)
		{
			var members = new List<AnchorBoltGroupFacts.WeldedMember>();
			var welds = plate.GetWelds();
			if (welds == null)
			{
				return members;
			}

			while (welds.MoveNext())
			{
				if (!(welds.Current is BaseWeld weld))
				{
					continue;
				}

				foreach (var joined in new[] { weld.MainObject, weld.SecondaryObject })
				{
					if (!(joined is Part part) || part.Identifier.Equals(plate.Identifier))
					{
						continue;
					}

					if (!(part is PolyBeam || (part is TS.Beam && !BulkSelectionHelper.IsRectangularCssBeam(part))))
					{
						continue;
					}

					var centerLine = part.GetCenterLine(true).OfType<Point>().ToList();
					if (centerLine.Count < 2)
					{
						continue;
					}

					var begin = centerLine[0];
					var end = centerLine[centerLine.Count - 1];
					members.Add(new AnchorBoltGroupFacts.WeldedMember(
						part.Identifier.GUID.ToString(),
						new CI.Geometry3D.Point3D(begin.X, begin.Y, begin.Z),
						new CI.Geometry3D.Point3D(end.X, end.Y, end.Z),
						BulkSelectionHelper.Describe(part)));
				}
			}

			return members;
		}

		/// <summary>
		/// Plate Washer Member Filter
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool PlateWasherMemberFilter(Part part)
		{
			return part is TS.ContourPlate cp && (part.Name == "PLATE_WASHER" || part.Name == "Washer Plate");
		}

		/// <summary>
		/// Web Plate Member Filter
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool WebPlateMemberFilter(Part part)
		{
			return part is TS.ContourPlate cp && part.Name == "WEB_PLATE";
		}

		/// <summary>
		/// Flange Plate Member Filter
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool FlangePlateMemberFilter(Part part)
		{
			return part is TS.ContourPlate cp && part.Name == "FLANGE_PLATE";
		}

		/// <summary>
		/// Nut Member Filter
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool NutMemberFilter(Part part)
		{
			return part.Profile.ProfileString.Contains("NUT_") || part.Name == "NUT";
		}

		/// <summary>
		/// Washer Member Filter
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static bool WasherMemberFilter(Part part)
		{
			return part.Name == "WASHER";
		}
		internal static void AddIdentifier<TIdentifier>(List<IIdentifier> identifiers, ModelObject teklaObject, string filerObjectHandle)
		where TIdentifier : IIdeaObject
		{
			var result = identifiers.Exists(x => x is StringIdentifier<TIdentifier> stringId && (stringId.Id == teklaObject.Identifier.GUID.ToString() || stringId.Id == filerObjectHandle));
			if (!result)
			{
				identifiers.Add(new StringIdentifier<TIdentifier>(filerObjectHandle));
			}
		}
	}
}