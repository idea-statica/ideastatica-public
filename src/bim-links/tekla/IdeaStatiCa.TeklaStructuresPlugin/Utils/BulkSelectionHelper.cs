using CI;
using CI.Geometry3D;
using IdeaStatiCa.BIM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using TS = Tekla.Structures;
using WM = System.Windows.Media.Media3D;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utilities
{
	internal static class BulkSelectionHelper
	{
		internal const string HaunchMemberName = "HAUNCH";
		internal const string TeklaAnchorRodName = "ANCHOR ROD";
		internal const string TeklaAnchorWasherName = "WASHER";
		internal const string TeklaAnchorNutName = "NUT";
		/// <summary>
		/// Find Joints
		/// </summary>
		/// <param name="myModel"></param>
		/// <param name="partsEnumerator"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static SorterResult FindJoints(Tekla.Structures.Model.Model myModel, ModelObjectEnumerator partsEnumerator)
		{
			List<BIM.Common.Member> bMembers = new List<BIM.Common.Member>();
			List<BIM.Common.Plate> plates = new List<BIM.Common.Plate>();
			List<BIM.Common.Weld> welds = new List<BIM.Common.Weld>();
			List<BIM.Common.FastenerGrid> fasteners = new List<BIM.Common.FastenerGrid>();

			Item.CustomComparer = new ItemEqualityComparer();

			while (partsEnumerator.MoveNext())
			{
				if (partsEnumerator.Current is Beam beam)
				{
					//skip concrete block
					if ((beam.Type == Beam.BeamTypeEnum.PAD_FOOTING || beam.Type == Beam.BeamTypeEnum.STRIP_FOOTING))
					{
						continue;
					}
					var partLcs = BulkSelectionHelper.CreateMatrix(beam);
					var bb = BulkSelectionHelper.CreateOrientedBoundingBox(myModel, beam);
					System.Windows.Rect cssBounds = new System.Windows.Rect(new System.Windows.Point(-1 * bb.Extent2, -1 * bb.Extent1), new System.Windows.Point(bb.Extent2, bb.Extent1));

					var cl1 = beam.GetCenterLine(false).OfType<Point>().ToArray();

					var begin = new Point3D(cl1[0].X, cl1[0].Y, cl1[0].Z);
					var end = new Point3D(cl1[1].X, cl1[1].Y, cl1[1].Z);

					var profileItem = new LibraryProfileItem();
					profileItem.Select(beam.Profile.ProfileString);

					if (IsRectangularCssBeam(beam))
					{
						var father = beam.GetFatherComponent();
						if (father?.Name.ToUpper() == HaunchMemberName || (father is Connection fCon && fCon.PositionType == TS.PositionTypeEnum.COLLISION_PLANE))
						{
							var vect1 = partLcs.AxisY * cssBounds.Y;
							var vect2 = partLcs.AxisY * -cssBounds.Y;

							var b1 = (begin.ToMediaPoint() + vect1.ToMediaVector());
							var b2 = (begin.ToMediaPoint() + vect2.ToMediaVector());

							var b3 = (end.ToMediaPoint() + vect1.ToMediaVector());
							var b4 = (end.ToMediaPoint() + vect2.ToMediaVector());

							plates.Add(new IdeaStatiCa.BIM.Common.Plate(beam, partLcs, new List<IPoint3D>() { b1.ToIndoPoint3D(), b3.ToIndoPoint3D(), b4.ToIndoPoint3D(), b2.ToIndoPoint3D() }, cssBounds.Width));
							continue;
						}
					}
					bMembers.Add(
						new BIM.Common.Member(
							beam,
							partLcs,
							begin,
							end,
							cssBounds
							));
				}

				if (partsEnumerator.Current is PolyBeam polyBeam)
				{
					var partLcs = BulkSelectionHelper.CreateMatrix(polyBeam);
					System.Windows.Rect cssBounds = new System.Windows.Rect(new System.Windows.Point(-1 * 200, -1 * 200), new System.Windows.Point(200, 200));

					var cl1 = polyBeam.GetCenterLine(false).OfType<Point>().ToArray();

					var begin = new Point3D(cl1[0].X, cl1[0].Y, cl1[0].Z);
					var end = new Point3D(cl1[1].X, cl1[1].Y, cl1[1].Z);

					var profileItem = new LibraryProfileItem();
					profileItem.Select(polyBeam.Profile.ProfileString);
					bMembers.Add(
						new BIM.Common.Member(
							polyBeam,
							partLcs,
							begin,
							end,
							cssBounds
							));
				}

				if (partsEnumerator.Current is ContourPlate contourPlate)
				{
					Matrix44 lcs = BulkSelectionHelper.CreateMatrix(contourPlate);

					var points = BulkSelectionHelper.GetContourPlatePoints(contourPlate);

					plates.Add(new BIM.Common.Plate(contourPlate, lcs, points, BulkSelectionHelper.GetContourPlateThickness(contourPlate)));
				}

				if (partsEnumerator.Current is BoltGroup boltGroup)
				{
					Matrix44 lcs = BulkSelectionHelper.CreateMatrix(boltGroup);
					var boltPositions = BulkSelectionHelper.GetBoltPositions(boltGroup);

					fasteners.Add(new BIM.Common.FastenerGrid(boltGroup, lcs, boltPositions));
				}

				if (partsEnumerator.Current is BentPlate bentPlate)
				{
					GeometrySectionEnumerator geometryEnumerator = bentPlate.Geometry.GetGeometryEnumerator();
					while (geometryEnumerator.MoveNext())
					{
						if (geometryEnumerator.Current?.GeometryNode is PolygonNode node)
						{
							var tuple = BulkSelectionHelper.GetPlateDataFromPolygon(node, bentPlate);
							plates.Add(new BIM.Common.Plate(bentPlate, tuple.Item1, tuple.Item2, bentPlate.Thickness));
						}
					}
				}

				if (partsEnumerator.Current is BaseWeld baseWeld)
				{
					welds.Add(new BIM.Common.Weld(baseWeld, BulkSelectionHelper.GetBIMItem(myModel, baseWeld.MainObject), BulkSelectionHelper.GetBIMItem(myModel, baseWeld.SecondaryObject)));
				}

				if (partsEnumerator.Current is Part part)
				{
					var weldsSet = part.GetWelds();
					while (weldsSet.MoveNext())
					{
						var modelObj = weldsSet.Current;

						if (modelObj is BaseWeld weld)
						{
							welds.Add(new BIM.Common.Weld(weld, BulkSelectionHelper.GetBIMItem(myModel, weld.MainObject), BulkSelectionHelper.GetBIMItem(myModel, weld.SecondaryObject)));
						}
					}

					var bolts = part.GetBolts();

					while (bolts.MoveNext())
					{
						var modelObj = bolts.Current;
						if (modelObj is BoltGroup boltGroupPart)
						{
							Matrix44 lcs = BulkSelectionHelper.CreateMatrix(boltGroupPart);
							var boltPositions = BulkSelectionHelper.GetBoltPositions(boltGroupPart);
							fasteners.Add(new BIM.Common.FastenerGrid(boltGroupPart, lcs, boltPositions));
						}
					}
				}
			}

			var sorterData = new BIM.Common.SorterData
			{
				Members = bMembers,
				Plates = plates,
				Welds = welds,
				Fasteners = fasteners
			};

			var sorter = new BIM.Common.ItemsSorter();
			var settings = new BIM.Common.SorterSettings();
			settings.EnlargeNodeXin = 1.6;
			settings.EnlargeNodeXout = 1.6;
			settings.EnlargeNodeY = 1.7;
			settings.EnlargeNodeZ = 1.7;

			var sortedJoints = sorter.Sort(sorterData, settings);
			return sortedJoints;
		}

		/// <summary>
		/// Is rectangular css of beam
		/// </summary>
		/// <param name="beam"></param>
		/// <returns></returns>
		public static bool IsRectangularCssBeam(Part beam)
		{
			string strProfName = beam.Profile.ProfileString;

			LibraryProfileItem profileItem = new LibraryProfileItem();
			profileItem.Select(strProfName);

			if (profileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
			{
				ParametricProfileItem paramProfileItem = new ParametricProfileItem();
				if (paramProfileItem.Select(strProfName) && paramProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_PL)
				{
					return true;
				}
			}
			return false;
		}

		private static BIM.Common.Item GetBIMItem(Tekla.Structures.Model.Model myModel, ModelObject item)
		{
			if (item is ContourPlate contourPlate)
			{
				Matrix44 lcsItem1 = CreateMatrix(contourPlate);
				var points = GetContourPlatePoints(contourPlate);
				return new BIM.Common.Plate(contourPlate, lcsItem1, points, GetContourPlateThickness(contourPlate));
			}
			else if (item is Beam beam)
			{
				Matrix44 lcsItem1 = CreateMatrix(beam);

				var bb = CreateOrientedBoundingBox(myModel, beam);

				var begin = new CI.Geometry3D.Point3D(beam.StartPoint.X, beam.StartPoint.Y, beam.StartPoint.Z);
				var end = new CI.Geometry3D.Point3D(beam.EndPoint.X, beam.EndPoint.Y, beam.EndPoint.Z);
				System.Windows.Rect cssBounds = new System.Windows.Rect(new System.Windows.Point(-0.5 * bb.Extent2, -0.5 * bb.Extent1), new System.Windows.Point(0.5 * bb.Extent2, 0.5 * bb.Extent1));

				return new BIM.Common.Member(beam, lcsItem1, begin, end, cssBounds);
			}
			else
			{
				throw new ArgumentException("GetBIMItem - Not found item" + item.ToString());
			}
		}

		private static List<CI.Geometry3D.Point3D> GetBoltPositions(BoltGroup boltGroup)
		{
			var midPoints = boltGroup.BoltPositions;

			List<CI.Geometry3D.Point3D> points = new List<CI.Geometry3D.Point3D>();
			foreach (var p in midPoints)
			{
				Point point = p as Point;
				points.Add(new CI.Geometry3D.Point3D() { X = point.X, Y = point.Y, Z = point.Z });
			}
			return points;
		}

		private static Tuple<Matrix44, List<CI.Geometry3D.IPoint3D>> GetPlateDataFromPolygon(PolygonNode node, BentPlate bentPlate)
		{
			var partCs = bentPlate.GetCoordinateSystem();
			Vector axisZ = Vector.Cross(partCs.AxisX, partCs.AxisY);
			WM.Vector3D pltAxisZ = new WM.Vector3D(axisZ.X, axisZ.Y, axisZ.Z);
			pltAxisZ.Normalize();

			List<WM.Point3D> points = new List<WM.Point3D>();
			List<CI.Geometry3D.IPoint3D> cIPoints = new List<CI.Geometry3D.IPoint3D>();
			foreach (ContourPoint point in node.Contour.ContourPoints)
			{
				points.Add(new WM.Point3D(point.X, point.Y, point.Z));
				cIPoints.Add(new CI.Geometry3D.Point3D(point.X, point.Y, point.Z));
			}

			WM.Vector3D translation = new WM.Vector3D(points[0].X, points[0].Y, points[0].Z);

			var proLcsYT = points[1] - points[0];
			proLcsYT.Normalize();
			var vYT = proLcsYT.ToIndoVector3D();

			proLcsYT = points[1] - points[0];
			var proLcsYT2 = points[2] - points[1];

			WM.Vector3D crossProduct = WM.Vector3D.CrossProduct(proLcsYT, proLcsYT2);
			if (crossProduct.Length == 0)
			{
				crossProduct = proLcsYT2;
			}
			var proLcsXT = crossProduct;
			proLcsXT.Normalize();
			var vZT = proLcsXT.ToIndoVector3D();
			var vXT = vZT * vYT;

			var pltMatrix = new Matrix44((WM.Point3D)translation, vXT, vYT, vZT);

			return new Tuple<Matrix44, List<IPoint3D>>(pltMatrix, cIPoints);
		}

		private static double GetContourPlateThickness(ContourPlate contourPlate)
		{
			const string WidthKey = "WIDTH";
			Hashtable propTable = new Hashtable();

			ArrayList PltThickness_ParamNames = new ArrayList
			{
				WidthKey
			};
			contourPlate.GetDoubleReportProperties(PltThickness_ParamNames, ref propTable);

			//Name = part.Name,
			return (double)propTable[WidthKey];
		}

		private static List<CI.Geometry3D.IPoint3D> GetContourPlatePoints(ContourPlate contourPlate)
		{
			var solid = contourPlate.GetSolid(Solid.SolidCreationTypeEnum.NORMAL);
			CoordinateSystem partCs = contourPlate.GetCoordinateSystem();
			Vector axisZ = Vector.Cross(partCs.AxisX, partCs.AxisY);
			WM.Vector3D pltAxisZ = new WM.Vector3D(axisZ.X, axisZ.Y, axisZ.Z);
			pltAxisZ.Normalize();

			var faceEnum = solid.GetFaceEnumerator();

			global::Tekla.Structures.Solid.Face foundFace = null;
			while (faceEnum.MoveNext())
			{
				WM.Vector3D tempFaceNormal = new WM.Vector3D(faceEnum.Current.Normal.X, faceEnum.Current.Normal.Y, faceEnum.Current.Normal.Z);
				tempFaceNormal.Normalize();

				if (tempFaceNormal.IsEqual(pltAxisZ))
				{
					foundFace = faceEnum.Current;
					break;
				}
			}

			if (foundFace == null)
			{
				foundFace = faceEnum.Current;
			}

			var loopEnumerator = foundFace.GetLoopEnumerator();
			if (!loopEnumerator.MoveNext())
			{
				throw new ArgumentException("Invalid loop");
			}

			var firstLoop = loopEnumerator.Current;

			var vertexEnumerator = firstLoop.GetVertexEnumerator();

			List<CI.Geometry3D.IPoint3D> points = new List<CI.Geometry3D.IPoint3D>();
			while (vertexEnumerator.MoveNext())
			{
				points.Add(new CI.Geometry3D.Point3D(vertexEnumerator.Current.X, vertexEnumerator.Current.Y, vertexEnumerator.Current.Z));
			}

			return points;
		}

		private static OBB CreateOrientedBoundingBox(Tekla.Structures.Model.Model model, Beam beam)
		{
			OBB obb = null;

			if (beam != null)
			{
				WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
				TransformationPlane originalTransformationPlane = workPlaneHandler.GetCurrentTransformationPlane();

				Solid solid = beam.GetSolid();
				Point minPointInCurrentPlane = solid.MinimumPoint;
				Point maxPointInCurrentPlane = solid.MaximumPoint;

				Point centerPoint = CalculateCenterPoint(minPointInCurrentPlane, maxPointInCurrentPlane);

				CoordinateSystem coordSys = beam.GetCoordinateSystem();
				TransformationPlane localTransformationPlane = new TransformationPlane(coordSys);
				workPlaneHandler.SetCurrentTransformationPlane(localTransformationPlane);

				solid = beam.GetSolid();
				Point minPoint = solid.MinimumPoint;
				Point maxPoint = solid.MaximumPoint;
				double extent0 = (maxPoint.X - minPoint.X) / 2;
				double extent1 = (maxPoint.Y - minPoint.Y) / 2;
				double extent2 = (maxPoint.Z - minPoint.Z) / 2;

				//for non anchor beams increase size of BB for small items 
				if (beam.Name != TeklaAnchorRodName && beam.Name != TeklaAnchorWasherName && beam.Name != TeklaAnchorNutName)
				{
					if (extent1 < 50)
					{
						extent1 *= 2;
					}
					if (extent2 < 50)
					{
						extent2 *= 2;
					}
				}
				workPlaneHandler.SetCurrentTransformationPlane(originalTransformationPlane);

				obb = new OBB(centerPoint, coordSys.AxisX, coordSys.AxisY,
												coordSys.AxisX.Cross(coordSys.AxisY), extent0, extent1, extent2);
			}

			return obb;
		}

		private static Matrix44 CreateMatrix(ModelObject part)
		{
			var partLcs = part.GetCoordinateSystem();

			var origin = new Point3D(partLcs.Origin.X, partLcs.Origin.Y, partLcs.Origin.Z);
			var axisX = new Vector3D(partLcs.AxisX.X, partLcs.AxisX.Y, partLcs.AxisX.Z).Normalize;
			var axisZ = new Vector3D(partLcs.AxisY.X, partLcs.AxisY.Y, partLcs.AxisY.Z).Normalize;
			var axisY = (axisX * axisZ).Normalize;
			return new Matrix44(origin, axisX, axisY, axisZ);
		}

		private static Point CalculateCenterPoint(Point min, Point max)
		{
			double x = min.X + ((max.X - min.X) / 2);
			double y = min.Y + ((max.Y - min.Y) / 2);
			double z = min.Z + ((max.Z - min.Z) / 2);

			return new Point(x, y, z);
		}
	}

	/// <summary>
	/// Item Equality Comparer for item sorter
	/// </summary>
	public class ItemEqualityComparer : IEqualityComparer<Item>
	{
		public bool Equals(Item x, Item y)
		{
			return (x.Parent as ModelObject)?.Identifier.Equals((y.Parent as ModelObject)?.Identifier) ?? false;
		}

		public int GetHashCode(Item obj)
		{
			return (obj.Parent as ModelObject)?.Identifier.GetHashCode() ?? 0;
		}
	}
}
