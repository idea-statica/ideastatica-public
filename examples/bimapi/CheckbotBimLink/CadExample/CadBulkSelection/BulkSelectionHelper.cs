using CI.Geometry3D;
using IdeaStatiCa.BIM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using WM = System.Windows.Media.Media3D;
using BimApiLinkCadExample.CadExampleApi;
using System.Windows.Documents;
using System.Windows;
using System.Net;

namespace BimApiLinkCadExample.CadBulkSelection
{
	public static class BulkSelectionHelper
	{
		internal static double tolerance = 1e-9;

		public static SorterResult FindJoints(List<int> partsEnumerator, ICadGeometryApi geometryApi)
		{
			//Lists of a BIM.Common Items that the Item Sorter uses to sort.
			List<Member> members = new List<Member>();
			List<Plate> plates = new List<Plate>();
			List<Weld> welds = new List<Weld>();
			List<FastenerGrid> fasteners = new List<FastenerGrid>();

			Item.CustomComparer = new ItemEqualityComparer();

			foreach (var current in partsEnumerator)
			{
				//Iterate through objects
				//Get the object
				CadExampleApi.CadObject obj = geometryApi.GetObjectById(current);

				//This code filters through provided objects and populates the lists provided above.
				if (obj == null)
				{
					continue;
				}
				else if (obj is CadMember mem)
				{
					var member = GetMemberItem(mem, geometryApi);
					members.Add(member);

					continue;
				}
				else if (obj is CadPlate pl)
				{
					//Do not add negative plates to the selection.
					//We add them based on the cuts present in the project
					if (pl.IsNegativeObject)
						continue;

					var plate = GetPlateItem(pl);
					plates.Add(plate);
					continue;
				}
				else if (obj is CadBoltGrid bg)
				{
					Matrix44 lcs = CreateMatrix(bg);

					var boltPositions = GetBoltPositions(bg);

					fasteners.Add(new FastenerGrid(bg, lcs, boltPositions));

					continue;

				}
				else if (obj is CadWeld weld)
				{

					List<IConnectedPart> connectedparts = weld.ConnectedParts;

					List<Item> connecteditems = new List<Item>();

					foreach (var connectedPart in connectedparts)
					{
						if (connectedPart.PartType == "CadPlate")
						{
							connecteditems.Add(GetPlateItem(geometryApi.GetPlate(connectedPart.PartId)));
						}
						else if (connectedPart.PartType == "CadMember")
						{
							connecteditems.Add(GetMemberItem(geometryApi.GetMember(connectedPart.PartId), geometryApi));
						}
					}
					if (connecteditems.Count > 1)
						welds.Add(new Weld(weld, connecteditems[0], connecteditems[1]));

					continue;
				}
			}


			var sorterData = new SorterData
			{
				Members = members,
				Plates = plates,
				Welds = welds,
				Fasteners = fasteners
			};

			var sorter = new ItemsSorter();
			var settings = new SorterSettings();
			
			//Settings to drive the sorter process.
			settings.EnlargeNodeXin = 1.6;
			settings.EnlargeNodeXout = 1.6;
			settings.EnlargeNodeY = 1.7;
			settings.EnlargeNodeZ = 1.7;

			var sortedJoints = sorter.Sort(sorterData, settings);
			return sortedJoints;
				
		}

		private static List<CI.Geometry3D.Point3D> GetBoltPositions(CadBoltGrid boltGrid)
		{
			List<CadPoint2D> pts = boltGrid.BoltPositions;
			CadPlane3D plane = boltGrid.BoltPlane;

			List<CI.Geometry3D.Point3D> points = new List<CI.Geometry3D.Point3D>();
			
			foreach (var point in pts)
			{
				CadPoint3D globalPt = CadPoint2D.Get2DPointInWorldCoords(point, plane);

				points.Add(new CI.Geometry3D.Point3D(globalPt.X, globalPt.X, globalPt.Z));
			}

			return points;
		}


		private class ItemEqualityComparer : IEqualityComparer<Item>
		{
			public bool Equals(Item x, Item y)
			{
				return (x.Parent as CadObject).Id.Equals((y.Parent as CadObject).Id);
			}

			public int GetHashCode(Item obj)
			{
				return (obj.Parent as CadObject).Id.GetHashCode();
			}
		}

		private static Plate GetPlateItem(CadPlate plate)
		{
			Matrix44 lcs = CreateMatrix(plate);

			var points = GetContourPlatePoints(plate);
			double maxBoundingWidth = GetCountourPlateMaxBoundingWidth(plate);

			return new Plate(plate, lcs, points, maxBoundingWidth);
		}

		private static List<CI.Geometry3D.IPoint3D> GetContourPlatePoints(CadPlate plate)
		{
			List<CI.Geometry3D.IPoint3D> points = new List<CI.Geometry3D.IPoint3D>();

			List<CadPoint2D> pts = plate.CadOutline2D.Points;

			foreach (var point in pts)
			{
				CadPoint3D globalPt = CadPoint2D.Get2DPointInWorldCoords(point, plate.CadOutline2D.Plane);

				points.Add(new CI.Geometry3D.Point3D(globalPt.X, globalPt.Y, globalPt.Z));
			}

			return points;
		}

		private static double GetCountourPlateMaxBoundingWidth(CadPlate plate)
		{
			List<CadPoint2D> points = plate.CadOutline2D.Points;

			if (points == null || points.Count == 0)
			{
				throw new ArgumentException("The list of points cannot be null or empty.");
			}

			double minX = points.Min(p => p.X);
			double maxX = points.Max(p => p.X);
			double minY = points.Min(p => p.Y);
			double maxY = points.Max(p => p.Y);

			double width = maxX - minX;
			double height = maxY - minY;

			return Math.Max(width, height);
		}


		private static Member GetMemberItem(CadMember member, ICadGeometryApi geometryApi)
		{
			Matrix44 lcs = CreateMatrix(member, geometryApi);

			CadPoint3D startPt = member.StartPoint;
			CadPoint3D endPt = member.EndPoint;
			
			//Beam Start and End Points
			var begin = new Point3D(startPt.X, startPt.Y, startPt.Z);
			var end = new Point3D(endPt.X, endPt.Y, endPt.Z);

			string strElProfName = member.CrossSection;

			//NEED TO GET THE BOUNDING BOX OF THE PROFILE.
			CadCrossSection css = geometryApi.GetCrossSectionByName(strElProfName);

			double cssWidth = css.CrossSectionWidth / 2;     //Section width divided by 2 to get domain st and end
			double cssHeight = css.CrossSectionHeight / 2;       //Section height divided by 2 to get domain st and end

			System.Windows.Rect cssBounds = new System.Windows.Rect(new System.Windows.Point(- cssWidth, -cssHeight), new System.Windows.Point(cssWidth, cssHeight));

			return new Member(member, lcs, begin, end, cssBounds);
		}

		private static Matrix44 CreateMatrix(CadPlate plate)
		{

			CadPlane3D plane = plate.CadOutline2D.Plane;
			CadPoint3D pt = plane.Origin;

			WM.Point3D lcsOrigin = new WM.Point3D(pt.X, pt.Y, pt.Z);
			WM.Vector3D axisX = new WM.Vector3D(plane.X.X, plane.X.Y, plane.X.Z);
			WM.Vector3D axisY = new WM.Vector3D(plane.Y.X, plane.Y.Y, plane.Y.Z);
			WM.Vector3D axisZ = new WM.Vector3D(plane.Z.X, plane.Z.Y, plane.Z.Z);

			//THE CENTRE OF THE PLATE SHOULD BE THE LOCATION OF THE LCS.
			//double portioning = plate.Portioning;
			//lcsOrigin += ((plate.TopIsZPositive ? 1 : -1) * (portioning - 0.5) * plate.Thickness * axisZ);

			WM.Vector3D translation = new WM.Vector3D(lcsOrigin.X, lcsOrigin.Y, lcsOrigin.Z);

			CI.Geometry3D.IPoint3D origin = new CI.Geometry3D.Point3D(translation.X, translation.Y, translation.Z);
			CI.Geometry3D.Vector3D axX = new CI.Geometry3D.Vector3D(axisX.X, axisX.Y, axisX.Z);
			axX = axX.Normalize;
			CI.Geometry3D.Vector3D axY = new CI.Geometry3D.Vector3D(axisY.X, axisY.Y, axisY.Z);
			axY = axY.Normalize;

			var lcs = new CI.Geometry3D.Matrix44(origin, axX, axY);
			return lcs;
		}

		private static Matrix44 CreateMatrix(CadBoltGrid boltGrid)
		{
			CadPlane3D plane = boltGrid.BoltPlane;
			CadPoint3D orig = plane.Origin;

			CI.Geometry3D.IPoint3D origin = new CI.Geometry3D.Point3D(orig.X, orig.Y, orig.Z);
			CI.Geometry3D.Vector3D axisX = new CI.Geometry3D.Vector3D(plane.X.X, plane.X.Y, plane.X.Z);
			axisX = axisX.Normalize;
			CI.Geometry3D.Vector3D axisY = new CI.Geometry3D.Vector3D(plane.Y.X, plane.Y.Y, plane.Y.Z);
			axisY = axisY.Normalize;

			var lcs = new CI.Geometry3D.Matrix44(origin, axisX, axisY);
			return lcs;
		}

		private static Matrix44 CreateMatrix(CadMember member, ICadGeometryApi geometryApi)
		{
			var lcs = geometryApi.GetMemberLcs(member.Id);

			CadPoint3D st = member.StartPoint;

			var origin = new Point3D(st.X, st.Y, st.Z);
			var axisX = new Vector3D(lcs.X.X, lcs.X.Y, lcs.X.Z).Normalize;
			var axisZ = new Vector3D(lcs.Z.X, lcs.Z.Y, lcs.Z.Z).Normalize;
			var axisY = (axisX * axisZ).Normalize;
			return new Matrix44(origin, axisX, axisY, axisZ);
		}

	}
}