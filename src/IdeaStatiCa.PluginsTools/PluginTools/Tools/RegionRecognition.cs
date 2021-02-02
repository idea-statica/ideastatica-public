using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CI.Geometry2D
{
	public static class RegionRecognition
	{
		public static bool TryGetCenterlineOfOpenCFSection(IPolyLine2D outline, double thickness, out IList<IPolyLine2D> centerlines, double tolerance = 0.0001, bool isClosed = true)
		{
			int edgeSegmentsCount = 0;

			centerlines = new List<IPolyLine2D>();
			List<Tuple<int, int, Point, Point>> segLst = new List<Tuple<int, int, Point, Point>>();
			Point begPt = outline.StartPoint;
			for (int i = 0, sz = outline.Segments.Count; i < sz; i++)
			{
				ISegment2D seg = outline.Segments[i];
				IList<ISegment2D> candidates = FindSecondSideOfPlate(begPt, seg, outline, thickness, tolerance);
				if (!candidates.Any())
				{
					edgeSegmentsCount++;
				}

				foreach (ISegment2D candidate in candidates)
				{
					int ix = outline.Segments.IndexOf(candidate);
					Point candidateBegPt = (ix == 0) ? outline.StartPoint : outline.Segments[ix - 1].EndPoint;
					CreateCenterlineSegment(begPt, seg, candidateBegPt, candidate, out Point clBegPt, out Point clEndPt);
					segLst.Add(new Tuple<int, int, Point, Point>(i, ix, clBegPt, clEndPt));
				}

				begPt = seg.EndPoint;
			}

			if (edgeSegmentsCount != 2)
			{
				return false;
			}

			//iterations
			List<Tuple<Point, Point>> centerlineSegments = new List<Tuple<Point, Point>>();
			while (segLst.Any())
			{
				List<Tuple<int, int, Point, Point>> resolved = new List<Tuple<int, int, Point, Point>>();

				for (int i = 0, sz = segLst.Count; i < sz; i++)
				{
					bool pair = false;
					Tuple<int, int, Point, Point> seg = segLst[i];
					if (resolved.Contains(seg) && isClosed)
					{
						continue;
					}

					if (i < sz - 1)
					{
						if (i > 0)
						{
							if (segLst[i - 1].Item1 == seg.Item1)
							{
								pair = true;
							}
						}

						if (segLst[i + 1].Item1 == seg.Item1)
						{
							pair = true;
						}
					}

					if (!pair)
					{
						centerlineSegments.Add(new Tuple<Point, Point>(seg.Item3, seg.Item4));
						resolved.Add(seg);

						Tuple<int, int, Point, Point> secondPart = segLst.FirstOrDefault((item) => item.Item1 == seg.Item2 && item.Item2 == seg.Item1);
						if (secondPart != null)
						{
							resolved.Add(secondPart);
							int secIx = segLst.IndexOf(secondPart);
							Tuple<int, int, Point, Point> prev = (secIx == 0) ? null : segLst[secIx - 1];
							Tuple<int, int, Point, Point> next = (secIx == segLst.Count - 1) ? null : segLst[secIx + 1];

							Tuple<int, int, Point, Point> pairMember = null;
							if (prev != null)
							{
								if (prev.Item1 == secondPart.Item1)
								{
									pairMember = prev;
								}
							}

							if (next != null)
							{
								if (next.Item1 == secondPart.Item1)
								{
									pairMember = next;
								}
							}

							if (pairMember != null)
							{
								resolved.Add(pairMember);
								Tuple<int, int, Point, Point> part3 = segLst.FirstOrDefault((item) => item.Item1 == pairMember.Item2 && item.Item2 == pairMember.Item1);
								if (part3 != null)
								{
									resolved.Add(part3);
								}
							}
						}
					}
				}

				var toBeRemoved = resolved.Distinct();
				foreach(Tuple<int, int, Point, Point> remItem in toBeRemoved)
				{
					segLst.Remove(remItem);
				}
			}

			if (!centerlineSegments.Any())
			{
				return false;
			}

			List<Tuple<Point, Point>> centerlineOrder = new List<Tuple<Point, Point>>();
			centerlineOrder.Add(centerlineSegments.First());
			centerlineSegments.Remove(centerlineSegments.First());

			bool escape = false;
			while (centerlineSegments.Any() && !escape)
			{
				List<Tuple<Point, Point>> toBeRemoved = new List<Tuple<Point,Point>>();
				Tuple<Point, Point> forwardDir = centerlineOrder.Last();
				Tuple<Point, Point> backwardDir = centerlineOrder.First();

				foreach(Tuple<Point, Point> seg in centerlineSegments)
				{
					if (seg.Item1.IsEqualWithTolerance(forwardDir.Item2))
					{
						forwardDir = seg;
						toBeRemoved.Add(seg);
						centerlineOrder.Add(seg);
					}
					else if (seg.Item2.IsEqualWithTolerance(forwardDir.Item2))
					{
						forwardDir = new Tuple<Point,Point>(seg.Item2, seg.Item1);
						toBeRemoved.Add(seg);
						centerlineOrder.Add(forwardDir);
					}
					else if (seg.Item2.IsEqualWithTolerance(backwardDir.Item1))
					{backwardDir = seg;
						toBeRemoved.Add(seg);
						centerlineOrder.Insert(0, seg);
					}
					else if (seg.Item1.IsEqualWithTolerance(backwardDir.Item1))
					{
						backwardDir = new Tuple<Point, Point>(seg.Item2, seg.Item1);
						toBeRemoved.Add(seg);
						centerlineOrder.Insert(0, backwardDir);
					}
				}

				foreach (Tuple<Point, Point> remItem in toBeRemoved)
				{
					centerlineSegments.Remove(remItem);
				}

				if (!toBeRemoved.Any())
				{
					Debug.Fail("Nespojitá střednice!! V algoritmu je chyba.");
					escape = true;
				}
			}

			IPolyLine2D centerline = new PolyLine2D(centerlineOrder.Count);
			for (int i = 0, sz = centerlineOrder.Count; i < sz; i++)
			{
				Tuple<Point, Point> segPts = centerlineOrder[i];
				if (i == 0)
				{
					centerline.StartPoint = segPts.Item1;
				}

				centerline.Segments.Add(new LineSegment2D(segPts.Item2));
			}

			centerlines.Add(centerline);
			return true;
		}

		private static IList<ISegment2D> FindSecondSideOfPlate(Point plateSideBegPt, ISegment2D plateSide, IPolyLine2D outline, double distance, double tolerance = 0.0001)
		{
			Vector vec2 = new Vector(plateSideBegPt.X - plateSide.EndPoint.X, plateSideBegPt.Y - plateSide.EndPoint.Y);
			List<ISegment2D> foundCandidates = new List<ISegment2D>();
			Point begPt = outline.StartPoint;
			foreach(ISegment2D seg in outline.Segments)
			{
				if (seg != plateSide)
				{
					Vector vec1 = new Vector(begPt.X - seg.EndPoint.X, begPt.Y - seg.EndPoint.Y);
					double angle = Math.Abs(GeomTools2D.AngleBetweenVectors(vec1, vec2));
					if (angle.IsEqual(0.0, 0.035) || angle.IsEqual(Math.PI, 0.035))
					{
						if (GeomTools2D.Distance(plateSideBegPt, plateSide.EndPoint, seg.EndPoint).IsEqual(distance, tolerance))
						{
							Vector vector = new Vector(1.0, 0.0);
							Vector vector2 = new Vector(plateSide.EndPoint.X - plateSideBegPt.X, plateSide.EndPoint.Y - plateSideBegPt.Y);
							double angle2 = Vector.AngleBetween(vector, vector2);//GeomTools2D.AngleBetweenVectors(vector, vector2);
							Matrix mx = Matrix.Identity;
							mx.Rotate(-angle2);
							Point a1 = mx.Transform(plateSideBegPt);
							Point a2 = mx.Transform(plateSide.EndPoint);
							Point b1 = mx.Transform(begPt);
							Point b2 = mx.Transform(seg.EndPoint);
							bool test = false;

							if (b1.X.IsEqual(a1.X) || b1.X.IsEqual(a2.X) || b2.X.IsEqual(a1.X) || b2.X.IsEqual(a2.X))
							{
								test = true;
							}
							else
							{
								if (Math.Sign(b1.X - a1.X) != Math.Sign(b1.X - a2.X))
								{
									test = true;
								}

								if (Math.Sign(b2.X - a1.X) != Math.Sign(b2.X - a2.X))
								{
									test = true;
								}

								if (Math.Sign(a1.X - b1.X) != Math.Sign(a1.X - b2.X))
								{
									test = true;
								}

								if (Math.Sign(a2.X - b1.X) != Math.Sign(a2.X - b2.X))
								{
									test = true;
								}
							}

							if (test)
							{
								foundCandidates.Add(seg);
							}
						}
					}
				}

				begPt = seg.EndPoint;
			}

			return foundCandidates;
		}

		private static void CreateCenterlineSegment(Point plateSideBegPt, ISegment2D plateSide, Point oppositeSideBegPt, ISegment2D oppositeSide, out Point clBegPt, out Point clEndPt)
		{
			clBegPt = new Point((plateSideBegPt.X + oppositeSide.EndPoint.X) * 0.5, (plateSideBegPt.Y + oppositeSide.EndPoint.Y) * 0.5);
			clEndPt = new Point((plateSide.EndPoint.X + oppositeSideBegPt.X) * 0.5, (plateSide.EndPoint.Y + oppositeSideBegPt.Y) * 0.5);
		}
	}
}
