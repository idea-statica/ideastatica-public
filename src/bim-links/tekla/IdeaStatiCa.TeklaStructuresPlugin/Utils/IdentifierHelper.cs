using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.TeklaStructuresPlugin.Utilities;
using System;
using System.Collections.Generic;
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
				&& StiffeningMemberFilterl(beamPart)
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
			else if ((teklaObject is TS.Beam beamAsPlate
						&& StiffeningMemberFilterl(beamAsPlate)
						&& BulkSelectionHelper.IsRectangularCssBeam(beamAsPlate)))
			{
				AddIdentifier<IIdeaPlate>(identifiers, teklaObject, beamAsPlate.Identifier.GUID.ToString());

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
						if (!IsWorkPlaneInShereOfConnection(connectionPoint, teklaPart, cutPlane.Plane))
						{
							//skip cut its on other side of member
							continue;
						}

						identifiers = GetIdentifier(teklaChildObject, ref identifiers, addToCollection, connectionPoint);
					}
					else if (teklaChildObject is Fitting fitting)
					{
						if (!IsWorkPlaneInShereOfConnection(connectionPoint, teklaPart, fitting.Plane))
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

					if (!(modelObj is TS.BoltGroup))
					{
						continue;
					}
					identifiers = GetIdentifier(modelObj, ref identifiers, addToCollection, connectionPoint);
				}

				//welds
				var welds = teklaPart.GetWelds();
				while (welds.MoveNext())
				{
					var modelObj = welds.Current;

					if (!(modelObj is TS.BaseWeld))
					{
						continue;
					}
					identifiers = GetIdentifier(modelObj, ref identifiers, addToCollection, connectionPoint);
				}
			}

			return identifiers;
		}

		private static bool IsWorkPlaneInShereOfConnection(Point connectionPoint, Part beam, Plane plane)
		{
			var originalCenterline = beam.GetCenterLine(false);
			var tsWorkplanePoint = plane.Origin;
			var tsWokplanePointHitByReferenceLine = Projection.LineToPlane(
				new Line(originalCenterline[0] as Point, originalCenterline[originalCenterline.Count - 1] as Point),
				new GeometricPlane(tsWorkplanePoint, plane.AxisX, plane.AxisY)
				);

			return IsPointInShereOfConnection(connectionPoint, beam, tsWokplanePointHitByReferenceLine.Origin);
		}

		private static bool IsPointInShereOfConnection(Point connectionPoint, Part beam, Point workPlanePoint)
		{
			var cuttedCenterline = beam.GetCenterLine(true);
			var originalCenterline = beam.GetCenterLine(false);

			//cuttedCenterline[0] as TSG.Point
			var tsCpPoint = connectionPoint;
			var tsWorkplanePoint = workPlanePoint;

			tsWorkplanePoint = Projection.PointToLine(tsWorkplanePoint, new Line(originalCenterline[0] as Point, cuttedCenterline[cuttedCenterline.Count - 1] as Point));

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
		private static bool StiffeningMemberFilterl(Part beam)
		{

			//skip anchor member
			if (beam.Profile.ProfileString.Contains("NUT_"))
			{
				return false;
			}

			//skip concrete blocks
			if (beam is TS.Beam b && (b.Type == TS.Beam.BeamTypeEnum.PAD_FOOTING || b.Type == TS.Beam.BeamTypeEnum.STRIP_FOOTING))
			{
				return false;
			}

			return true;
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