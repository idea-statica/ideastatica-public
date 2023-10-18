using System;
using System.Collections.Generic;
using System.Text;
using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Geometry2D;
using BimApiLinkCadExample.CadExampleApi;

namespace BimApiLinkCadExample
{
	internal static class Convert
	{

		internal static Vector3D ToIdea(this CadVector3D vector)
		{
			return new Vector3D() { X = vector.X, Y = vector.Y, Z = vector.Z };
		}

		internal static CoordSystemByVector ToIdea(this CadPlane3D plane)
		{
			return new CoordSystemByVector() { VecX = plane.X.ToIdea(), VecY = plane.Y.ToIdea(), VecZ = plane.Z.ToIdea() };
		}

		internal static Point2D ToIdea(this CadPoint2D point2D) 
		{
			return new Point2D() { X = point2D.X, Y = point2D.Y };
		}

		internal static LineSegment2D ToIdeaSegment(this Point2D end) 
		{
			return new LineSegment2D() { EndPoint = end };
		}

		internal static Region2D ToIdea(this CadOutline2D outline) 
		{
			Region2D region = new Region2D();

			PolyLine2D pl = new PolyLine2D();

			//Simple Conversion. Can be upgraded to work with arc segments.
			for(int i = 0; i < outline.Points.Count; i++) 
			{
				Point2D pt = outline.Points[i].ToIdea();

				if (i == 0)
				{
					pl.StartPoint = pt;
				}
				else
				{
					pl.Segments.Add(pt.ToIdeaSegment());
				}
			}

			region.Outline = pl;

			return region;
		}
	}
}
