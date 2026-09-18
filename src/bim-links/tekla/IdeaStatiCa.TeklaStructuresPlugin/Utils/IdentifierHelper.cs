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
			if (teklaObject is BoltGroup)
			{
				AddIdentifier<IIdeaBoltGrid>(identifiers, teklaObject, teklaObject.Identifier.GUID.ToString());
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

					//test if is baseplate with dummy bolt group
					if (!teklaPart.Identifier.Equals(boltGroup.PartToBeBolted.Identifier) || !teklaPart.Identifier.Equals(boltGroup.PartToBoltTo.Identifier) || boltGroup.OtherPartsToBolt.Count != 0)
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
#if TEKLA2025
			if (part.GetCustomObjectType() == "AnchorBolt")
			{
				return true;
			}
#endif
			return part.Name == "ANCHOR ROD";
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