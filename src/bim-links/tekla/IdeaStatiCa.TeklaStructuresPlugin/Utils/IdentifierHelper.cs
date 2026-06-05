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
		internal static List<IIdentifier> GetIdentifier(ModelObject teklaObject, ref List<IIdentifier> identifiers, bool addToCollection = true, Point connectionPoint = null, IReadOnlyList<Point> allConnectionPoints = null, IdeaStatiCa.Plugin.IPluginLogger logger = null)
		{
			var cpStr = connectionPoint != null ? $"[{connectionPoint.X:F1},{connectionPoint.Y:F1},{connectionPoint.Z:F1}]" : "?";

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
						logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED beam '{beamPart.Name}' ({beamPart.Profile.ProfileString}) guid={beamPart.Identifier.GUID} as ConnectedMember");
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
						logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED polybeam '{polybeamPart.Name}' guid={polybeamPart.Identifier.GUID} as ConnectedMember");
					}
				}
			}
			else if (teklaObject is TS.ContourPlate plate)
			{
				AddIdentifier<IIdeaPlate>(identifiers, teklaObject, plate.Identifier.GUID.ToString());
				logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED ContourPlate '{plate.Name}' guid={plate.Identifier.GUID}");
			}
			else if ((teklaObject is TS.Beam beamAsPlate))
			{
				if (AnchorMemberFilter(beamAsPlate))
				{
					AddIdentifier<IIdeaAnchorGrid>(identifiers, teklaObject, teklaObject.Identifier.GUID.ToString());
					AddConcreteBlockToAnchor(identifiers);
					logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED beam '{beamAsPlate.Name}' guid={beamAsPlate.Identifier.GUID} as AnchorGrid");
				}
				else if (ConcreteBlocksFilter(beamAsPlate))
				{
					AddIdentifier<IIdeaConcreteBlock>(identifiers, teklaObject, beamAsPlate.Identifier.GUID.ToString());
					AddConcreteBlockToAnchor(identifiers);
					logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED beam '{beamAsPlate.Name}' guid={beamAsPlate.Identifier.GUID} as ConcreteBlock");
				}
				else if (StiffeningMemberFilter(beamAsPlate) && addToCollection && BulkSelectionHelper.IsRectangularCssBeam(beamAsPlate))
				{
					AddIdentifier<IIdeaPlate>(identifiers, teklaObject, beamAsPlate.Identifier.GUID.ToString());
					logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED beam '{beamAsPlate.Name}' ({beamAsPlate.Profile.ProfileString}) guid={beamAsPlate.Identifier.GUID} as Plate (rectangular stiffening)");
				}
			}

			if (teklaObject is BoltGroup bg0)
			{
				AddIdentifier<IIdeaBoltGrid>(identifiers, teklaObject, teklaObject.Identifier.GUID.ToString());
				logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED BoltGroup guid={bg0.Identifier.GUID}");
			}
			else if (teklaObject is BaseWeld bw0)
			{
				AddIdentifier<IIdeaWeld>(identifiers, teklaObject, teklaObject.Identifier.GUID.ToString());
				logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED Weld guid={bw0.Identifier.GUID}");
			}
			else if (teklaObject is TS.BentPlate bentPlate)
			{
				AddIdentifier<IIdeaFoldedPlate>(identifiers, teklaObject, bentPlate.Identifier.GUID.ToString());
				logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED BentPlate guid={bentPlate.Identifier.GUID}");
			}
			else if (teklaObject is TS.CutPlane cutPlane0)
			{
				AddIdentifier<IIdeaCut>(identifiers, teklaObject, cutPlane0.Identifier.GUID.ToString());
			}
			else if (teklaObject is TS.Fitting fitting0)
			{
				AddIdentifier<IIdeaCut>(identifiers, teklaObject, fitting0.Identifier.GUID.ToString());
			}
			else if (teklaObject is TS.BooleanPart booleanPart0 && booleanPart0.Type == TS.BooleanPart.BooleanTypeEnum.BOOLEAN_CUT)
			{
				AddIdentifier<IIdeaCut>(identifiers, teklaObject, booleanPart0.Identifier.GUID.ToString());
			}

			if (teklaObject is Part teklaPart)
			{
				var beamChildrenEnumerator = teklaPart.GetChildren();
				while (beamChildrenEnumerator.MoveNext())
				{
					var teklaChildObject = beamChildrenEnumerator.Current;

					if (teklaChildObject is CutPlane cutPlane)
					{
						if (!IsWorkPlaneInSphereOfConnection(connectionPoint, teklaPart, cutPlane.Plane))
						{
							//skip cut its on other side of member
							continue;
						}

						identifiers = GetIdentifier(teklaChildObject, ref identifiers, addToCollection, connectionPoint, allConnectionPoints, logger);
					}
					else if (teklaChildObject is Fitting fitting)
					{
						if (!IsWorkPlaneInSphereOfConnection(connectionPoint, teklaPart, fitting.Plane))
						{
							//skip cut its on other side of member
							continue;
						}

						identifiers = GetIdentifier(teklaChildObject, ref identifiers, addToCollection, connectionPoint, allConnectionPoints, logger);
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

							identifiers = GetIdentifier(teklaChildObject, ref identifiers, addToCollection, connectionPoint, allConnectionPoints, logger);
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
					var boltOrigin = boltCs.Origin as Point;
					var boltNear = IsPointNearOfConnection(connectionPoint, teklaPart, boltOrigin, allConnectionPoints);

					if (!boltNear)
					{
						// In MC mode (allConnectionPoints set) always filter by nearest CP — even for stiffening plates.
						// In single-joint mode keep the original exception for stiffening plates.
						bool skipStiffeningException = allConnectionPoints != null && allConnectionPoints.Count > 1;
						if (skipStiffeningException || !(StiffeningMemberFilter(teklaPart) && addToCollection && BulkSelectionHelper.IsRectangularCssBeam(teklaPart)))
						{
							logger?.LogTrace($"IdentifierHelper: cp={cpStr} REJECTED BoltGroup guid={boltGroup.Identifier.GUID} origin=[{boltOrigin?.X:F1},{boltOrigin?.Y:F1},{boltOrigin?.Z:F1}] on part '{teklaPart.Name}' (not nearest cp)");
							continue;
						}
					}

					//test if is baseplate with dummy bolt group
					if (!teklaPart.Identifier.Equals(boltGroup.PartToBeBolted.Identifier) || !teklaPart.Identifier.Equals(boltGroup.PartToBoltTo.Identifier) || boltGroup.OtherPartsToBolt.Count != 0)
					{
						var distDbg = allConnectionPoints != null && allConnectionPoints.Count > 1
						? $" distToThis={Distance.PointToPoint(connectionPoint, boltOrigin):F0} minDist={allConnectionPoints.Min(cp => Distance.PointToPoint(cp, boltOrigin)):F0}"
						: "";
					logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED BoltGroup guid={boltGroup.Identifier.GUID} origin=[{boltOrigin?.X:F1},{boltOrigin?.Y:F1},{boltOrigin?.Z:F1}] on part '{teklaPart.Name}'{distDbg}");
						identifiers = GetIdentifier(modelObj, ref identifiers, addToCollection, connectionPoint, allConnectionPoints, logger);
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
					var weldOrigin = weldCs.Origin as Point;
					var weldNear = IsPointNearOfConnection(connectionPoint, teklaPart, weldOrigin, allConnectionPoints);

					if (!weldNear)
					{
						bool skipStiffeningException = allConnectionPoints != null && allConnectionPoints.Count > 1;
						if (skipStiffeningException || !(StiffeningMemberFilter(teklaPart) && addToCollection && BulkSelectionHelper.IsRectangularCssBeam(teklaPart)))
						{
							logger?.LogTrace($"IdentifierHelper: cp={cpStr} REJECTED Weld guid={weld.Identifier.GUID} origin=[{weldOrigin?.X:F1},{weldOrigin?.Y:F1},{weldOrigin?.Z:F1}] on part '{teklaPart.Name}' (not nearest cp)");
							continue;
						}
					}

					logger?.LogTrace($"IdentifierHelper: cp={cpStr} ACCEPTED Weld guid={weld.Identifier.GUID} origin=[{weldOrigin?.X:F1},{weldOrigin?.Y:F1},{weldOrigin?.Z:F1}] on part '{teklaPart.Name}'");
					identifiers = GetIdentifier(modelObj, ref identifiers, addToCollection, connectionPoint, allConnectionPoints, logger);
				}
			}

			return identifiers;
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

		private static bool IsPointNearOfConnection(Point connectionPoint, Part beam, Point nearPoint, IReadOnlyList<Point> allConnectionPoints = null)
		{
			if (nearPoint == null) return false;
			var distanceToNearPoint = Distance.PointToPoint(connectionPoint, nearPoint);

			// When multiple connection points are known (MC bulk run), accept the item only if
			// this connection point is the closest one to nearPoint — prevents objects belonging
			// to a neighbouring joint from leaking into this one.
			if (allConnectionPoints != null && allConnectionPoints.Count > 1)
			{
				var minDistance = allConnectionPoints.Min(cp => Distance.PointToPoint(cp, nearPoint));
				return distanceToNearPoint <= minDistance + 1e-6;
			}

			var cuttedCenterline = beam.GetCenterLine(true);
			// 1/4 of len member
			var beamLen = Distance.PointToPoint(cuttedCenterline[0] as Point, cuttedCenterline[cuttedCenterline.Count - 1] as Point);

			return distanceToNearPoint.IsLesserOrEqual(beamLen / 4);
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