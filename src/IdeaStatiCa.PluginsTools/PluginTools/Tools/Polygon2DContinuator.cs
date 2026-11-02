using System.Collections.Generic;

namespace CI.Geometry2D
{
	public static class Polygon2DContinuator
	{
		public static void CreatePolyLine2D(IPolygon2D source, out IPolyLine2D target)
		{
			target = new PolyLine2D(source.Count);
			if (source.Count < 1)
			{
				return;
			}

			target.StartPoint = source[0];
			IList<ISegment2D> segments = target.Segments;
			for (int i = 1; i < source.Count; ++i)
			{
				segments.Add(new LineSegment2D(source[i]));
			}
		}
	}
}