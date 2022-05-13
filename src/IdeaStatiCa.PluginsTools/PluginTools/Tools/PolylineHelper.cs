using CI;
using CI.Geometry2D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace IdeaRS.GeometricItems
{
	public static class PolylineHelper
	{
		internal static readonly char[] PolygonSeparators = new char[] { ' ' };

		public static void Close(this Polyline p)
		{
			if (p.Segments.Count > 2)
			{
				var start = p.Segments.FirstOrDefault(s => s.Type == SegmentType.StartPoint);
				if (start != null)
				{
					var sp = GetSegmentEndPoint(start);
					var ep = GetSegmentEndPoint(p.Segments.LastOrDefault(s => s.Type != SegmentType.StartPoint));
					if (sp != ep)
					{
						p.Segments.Add(Segment.LineSegment(sp.X, sp.Y));
					}
				}
			}
		}

		public static bool IsValidForCss(this Polyline p)
		{
			if (p.Segments.Count < 3)
			{
				return false;
			}

			if (p.IsCollinear())
			{
				return false;
			}

			// TODO check for crossing lines

			return true;
		}

		public static Region RegionFromString(string stringData)
		{
			Region r = new Region();
			string[] parts = stringData.Split('M');
			bool first = true;
			foreach (string part in parts)
			{
				if (string.IsNullOrEmpty(part))
				{
					continue;
				}

				if (first)
				{
					first = false;
					r.Outline = PolylineFromString("M" + part);
				}
				else
				{
					if (r.Openings == null)
					{
						r.Openings = new List<Polyline>();
					}

					r.Openings.Add(PolylineFromString("M" + part));
				}
			}

			return r;
		}

		private static Polyline GetPolyline(IdeaRS.OpenModel.Geometry2D.PolyLine2D polyLine2D)
		{
			Polyline omOutline = new Polyline();
			omOutline.Segments = new List<Segment>();
			omOutline.Segments.Add(Segment.StartPoint(polyLine2D.StartPoint.X, polyLine2D.StartPoint.Y));

			System.Windows.Point prevpoint = new System.Windows.Point(polyLine2D.StartPoint.X, polyLine2D.StartPoint.Y);
			foreach (var segment in polyLine2D.Segments)
			{
				if (segment is IdeaRS.OpenModel.Geometry2D.LineSegment2D lineSegment)
				{
					omOutline.Segments.Add(Segment.LineSegment(lineSegment.EndPoint.X, lineSegment.EndPoint.Y));
					prevpoint = new System.Windows.Point(lineSegment.EndPoint.X, lineSegment.EndPoint.Y);
				}
				else if (segment is IdeaRS.OpenModel.Geometry2D.ArcSegment2D acrSegment)
				{
					var arc = new CI.Geometry2D.CircularArcSegment2D(new System.Windows.Point(acrSegment.EndPoint.X, acrSegment.EndPoint.Y), new System.Windows.Point(acrSegment.Point.X, acrSegment.Point.Y));

					double radius = arc.GetRadius(ref prevpoint);
					var sweepFlag = arc.IsCounterClockwise(ref prevpoint) > 0 ? 0 : 1;
					var largeArcFlag = arc.GetAngle(ref prevpoint) > 180 ? 1 : 0;

					omOutline.Segments.Add(IdeaRS.GeometricItems.Segment.ArcSegment(radius, largeArcFlag, sweepFlag, acrSegment.EndPoint.X, acrSegment.EndPoint.Y));

					prevpoint = new System.Windows.Point(acrSegment.EndPoint.X, acrSegment.EndPoint.X);
				}
			}

			return omOutline;
		}

		public static Region Region2DIOMToRegion(IdeaRS.OpenModel.Geometry2D.Region2D iomRegion)
		{
			Region r = new Region();
			r.Openings = new List<Polyline>();
			r.Outline = GetPolyline(iomRegion.Outline);

			foreach (var opening in iomRegion.Openings)
			{
				r.Openings.Add(GetPolyline(opening));
			}

			return r;
		}

		public static int getQuadrant(Point Center, Point Start, double radius)
		{
			double a = Math.Sqrt(Math.Pow(radius, 2) * 2) / 2;
			Point s1 = new Point(Center.X - a, Center.Y + a);
			Point s2 = new Point(Center.X + a, Center.Y + a);
			Point s3 = new Point(Center.X + a, Center.Y - a);
			Point s4 = new Point(Center.X - a, Center.Y - a);

			if (Start.X >= s1.X && Start.X < s2.X && Start.Y >= s1.Y && Start.Y > s2.Y)
			{
				return 1;
			}
			else if (Start.X >= s2.X && Start.X > s3.X && Start.Y <= s2.Y && Start.Y > s3.Y)
			{
				return 2;
			}
			else if (Start.X <= s3.X && Start.X > s4.X && Start.Y <= s3.Y && Start.Y < s4.Y)
			{
				return 3;
			}
			else if (Start.X <= s4.X && Start.X < s1.X && Start.Y >= s4.Y && Start.Y < s1.Y)
			{
				return 4;
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Creates polyline from path - see http://tech.pro/tutorial/728/creating-custom-shapes-with-xaml
		/// </summary>
		/// <param name="stringData">String in the format of svg path</param>
		/// <param name="tolerance">The tolerance which is used for check of positions of points</param>
		/// <returns>The instance of the Polyline</returns>
		public static Polyline PolylineFromString(string stringData, double tolerance = 1e-6)
		{
			var stringVals = stringData.Split(PolygonSeparators, StringSplitOptions.RemoveEmptyEntries);

			Polyline p = new Polyline();

			int paramCount = stringVals.Length;
			int i = 0;
			Point prevPoint = new Point();
			while (i < paramCount)
			{
				switch (stringVals[i++].ToUpper())
				{
					case "M":
						{
							p.Segments.Add(Segment.StartPoint(Double.Parse(stringVals[i++], CultureInfo.InvariantCulture),
								Double.Parse(stringVals[i++], CultureInfo.InvariantCulture)));
							prevPoint.X = Double.Parse(stringVals[i - 2], CultureInfo.InvariantCulture);
							prevPoint.Y = Double.Parse(stringVals[i - 1], CultureInfo.InvariantCulture);
							continue;
						}
					case "L":
						{
							p.Segments.Add(Segment.LineSegment(Double.Parse(stringVals[i++], CultureInfo.InvariantCulture),
								Double.Parse(stringVals[i++], CultureInfo.InvariantCulture)));
							prevPoint.X = Double.Parse(stringVals[i - 2], CultureInfo.InvariantCulture);
							prevPoint.Y = Double.Parse(stringVals[i - 1], CultureInfo.InvariantCulture);
							continue;
						}

					case "Z":
						{
							if (p.Segments.Count < 3)
							{
								return null;
							}
							var fpar = p.Segments.First().Parameters;
							var lpar = p.Segments.Last().Parameters;

							double fparX = fpar[0];
							double fparY = fpar[1];

							double lparX = lpar[lpar.Count - 2];
							double lparY = lpar[lpar.Count - 1];

							if (!(fparX.IsEqual(lparX, tolerance) && fparY.IsEqual(lparX, tolerance)))
							{
								p.Segments.Add(Segment.LineSegment(fparX, fparY));
							}

							goto Finish;
						}
					case "A":
						{
							//Get values
							double radius = Double.Parse(stringVals[i++], CultureInfo.InvariantCulture);
							//radius = 0.05;
							i = i + 3;
							double ClockWiseArc = Double.Parse(stringVals[i++], CultureInfo.InvariantCulture);
							//ClockWiseArc = 0;

							Point endArcPoint = new Point(Double.Parse(stringVals[i++], CultureInfo.InvariantCulture), Double.Parse(stringVals[i++], CultureInfo.InvariantCulture));
							//endArcPoint.X = 0.05;
							//endArcPoint.Y = 0.15;

							#region calCenterpoint
							Point CenterArcPoint = new Point();

							//double radius
							double l2 = Math.Sqrt(Math.Pow(endArcPoint.X - prevPoint.X, 2) + Math.Pow(endArcPoint.Y - prevPoint.Y, 2));
							double l = l2 / 2;

							var vY = (((endArcPoint.Y - prevPoint.Y) * Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(l, 2))) / l2);
							if (double.IsNaN(vY))
							{
								vY = 0;
							}
							var vX = (((endArcPoint.X - prevPoint.X) * Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(l, 2))) / l2);
							if (double.IsNaN(vX))
							{
								vX = 0;
							}
							double X1 = ((prevPoint.X + endArcPoint.X) / 2) - vY;
							double Y1 = ((prevPoint.Y + endArcPoint.Y) / 2) + vX;

							double ClockWise = (prevPoint.X - X1) * (endArcPoint.Y - Y1) - (prevPoint.Y - Y1) * (endArcPoint.X - X1);
							if ((ClockWise.IsGreater(0.0) && ClockWiseArc == 0) || (ClockWise.IsLesserOrEqual(0.0) && ClockWiseArc > 0))
							{
								CenterArcPoint.X = X1;
								CenterArcPoint.Y = Y1;
							}
							else
							{
								var vY2 = (((endArcPoint.Y - prevPoint.Y) * Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(l, 2))) / l2);
								if (double.IsNaN(vY2))
								{
									vY2 = 0;
								}
								var vX2 = (((endArcPoint.X - prevPoint.X) * Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(l, 2))) / l2);
								if (double.IsNaN(vX2))
								{
									vX2 = 0;
								}
								double X2 = ((prevPoint.X + endArcPoint.X) / 2) + vY2;
								double Y2 = ((prevPoint.Y + endArcPoint.Y) / 2) - vX2;
								CenterArcPoint.X = X2;
								CenterArcPoint.Y = Y2;
							}
							#endregion

							#region calSomeMiddlePoin
							double midX = 0;
							double midY = 0;
							midX = (prevPoint.X + endArcPoint.X) / 2;
							midY = (prevPoint.Y + endArcPoint.Y) / 2;

							if (double.IsNaN(midX))
							{
								midX = 0;
							}
							if (double.IsNaN(midY))
							{
								midY = 0;
							}

							//circle (x -m)^2 + (y-n)^2 -r^2 = 0
							if (Math.Abs(prevPoint.X - endArcPoint.X) > 4e-6)
							{
								double c = Math.Pow(midX, 2) - (2 * midX * CenterArcPoint.X) + Math.Pow(CenterArcPoint.X, 2) + Math.Pow(CenterArcPoint.Y, 2) - Math.Pow(radius, 2);
								double b = -2 * CenterArcPoint.Y;
								double a = 1;
								double D = Math.Pow(b, 2) - (4 * a * c);

								double y1 = (-1 * b + Math.Sqrt(D)) / (2 * a);
								double y2 = (-1 * b - Math.Sqrt(D)) / (2 * a);

								double lenStarToMiddle1 = Math.Sqrt(Math.Pow(midX - prevPoint.X, 2) + Math.Pow(y1 - prevPoint.Y, 2));
								double lenStarToMiddle2 = Math.Sqrt(Math.Pow(midX - prevPoint.X, 2) + Math.Pow(y2 - prevPoint.Y, 2));

								double lenEndToMiddle1 = Math.Sqrt(Math.Pow(midX - endArcPoint.X, 2) + Math.Pow(y1 - endArcPoint.Y, 2));
								double lenEndToMiddle2 = Math.Sqrt(Math.Pow(midX - endArcPoint.X, 2) + Math.Pow(y2 - endArcPoint.Y, 2));

								//case if len Star and End arch == 2* radius
								if (Math.Abs(l2 - (2 * radius)) < 4e-6)
								{
									int quadratStart = getQuadrant(CenterArcPoint, prevPoint, radius);
									int quadratP1 = getQuadrant(CenterArcPoint, new Point(midX, y1), radius);
									int quadratP2 = getQuadrant(CenterArcPoint, new Point(midX, y2), radius);
									if (ClockWiseArc == 0)
									{
										while (true)
										{
											if (quadratStart == quadratP1)
											{
												midY = y1;
												break;
											}
											if (quadratStart == quadratP2)
											{
												midY = y2;
												break;
											}
											quadratStart--;
											if (quadratStart < 1)
											{
												quadratStart = 4;
											}
										}
									}
									else
									{
										while (true)
										{
											if (quadratStart == quadratP1)
											{
												midY = y1;
												break;
											}
											if (quadratStart == quadratP2)
											{
												midY = y2;
												break;
											}
											quadratStart++;
											if (quadratStart > 4)
											{
												quadratStart = 1;
											}
										}
									}
								}
								else
								{
									if ((lenStarToMiddle1 + lenEndToMiddle1) < (lenStarToMiddle2 + lenEndToMiddle2))
									{
										midY = y1;
									}
									else
									{
										midY = y2;
									}
								}
							}
							else
							{
								double c = Math.Pow(midY, 2) - (2 * midY * CenterArcPoint.Y) + Math.Pow(CenterArcPoint.X, 2) + Math.Pow(CenterArcPoint.Y, 2) - Math.Pow(radius, 2);
								double b = -2 * CenterArcPoint.X;
								double a = 1;
								double D = Math.Pow(b, 2) - (4 * a * c);

								double x1 = (-1 * b + Math.Sqrt(D)) / (2 * a);
								double x2 = (-1 * b - Math.Sqrt(D)) / (2 * a);

								double lenStarToMiddle1 = Math.Sqrt(Math.Pow(x1 - prevPoint.X, 2) + Math.Pow(midY - prevPoint.Y, 2));
								double lenStarToMiddle2 = Math.Sqrt(Math.Pow(x2 - prevPoint.X, 2) + Math.Pow(midY - prevPoint.Y, 2));

								double lenEndToMiddle1 = Math.Sqrt(Math.Pow(x1 - endArcPoint.X, 2) + Math.Pow(midY - endArcPoint.Y, 2));
								double lenEndToMiddle2 = Math.Sqrt(Math.Pow(x2 - endArcPoint.X, 2) + Math.Pow(midY - endArcPoint.Y, 2));

								//case if len Star and End arch == 2* radius
								if ((l2 - (2 * radius)) < 4e-6)
								{
									int quadratStart = getQuadrant(CenterArcPoint, prevPoint, radius);
									int quadratP1 = getQuadrant(CenterArcPoint, new Point(x1, midY), radius);
									int quadratP2 = getQuadrant(CenterArcPoint, new Point(x2, midY), radius);
									if (ClockWiseArc == 0)
									{
										while (true)
										{
											if (quadratStart == quadratP1)
											{
												midX = x1;
												break;
											}
											if (quadratStart == quadratP2)
											{
												midX = x2;
												break;
											}
											quadratStart--;
											if (quadratStart < 1)
											{
												quadratStart = 4;
											}
										}
									}
									else
									{
										while (true)
										{
											if (quadratStart == quadratP1)
											{
												midX = x1;
												break;
											}
											if (quadratStart == quadratP2)
											{
												midX = x2;
												break;
											}
											quadratStart++;
											if (quadratStart > 4)
											{
												quadratStart = 1;
											}
										}
									}
								}
								else
								{
									if ((lenStarToMiddle1 + lenEndToMiddle1) < (lenStarToMiddle2 + lenEndToMiddle2))
									{
										midX = x1;
									}
									else
									{
										midX = x2;
									}
								}
							}
							#endregion
							p.Segments.Add(Segment.ArcSegment(endArcPoint.X, endArcPoint.Y, midX, midY));
							continue;
						}

					default:
						throw new ArgumentException("Unsupported command");
				}
			}

		Finish:
			if (p.Segments.Count < 3)
			{
				return null;
			}

			return p;
		}

		public static string RegionToString(this Region p)
		{
			StringBuilder regionSB = new StringBuilder();
			regionSB.Append(PolylineToString(p.Outline));
			if (p.Openings != null)
			{
				foreach (Polyline opening in p.Openings)
				{
					regionSB.Append(" ");
					regionSB.Append(PolylineToString(opening));
				}
			}

			return regionSB.ToString();
		}

		private static OpenModel.Geometry2D.PolyLine2D GetPolyline2D(IPolyLine2D polyLine2D)
		{
			OpenModel.Geometry2D.PolyLine2D omOutline = new OpenModel.Geometry2D.PolyLine2D();
			omOutline.StartPoint = new OpenModel.Geometry2D.Point2D();
			omOutline.StartPoint.X = polyLine2D.StartPoint.X;
			omOutline.StartPoint.Y = polyLine2D.StartPoint.Y;

			foreach (var drdSegment in polyLine2D.Segments)
			{
				OpenModel.Geometry2D.LineSegment2D omSegmen = new OpenModel.Geometry2D.LineSegment2D();
				omSegmen.EndPoint = new OpenModel.Geometry2D.Point2D();
				omSegmen.EndPoint.X = drdSegment.EndPoint.X;
				omSegmen.EndPoint.Y = drdSegment.EndPoint.Y;
				omOutline.Segments.Add(omSegmen);
			}

			return omOutline;
		}

		/// <summary>
		/// conver  IdeaRS.GeometricItems.Region to the IdeaRS.OpenModel.Geometry2D.Region2
		/// </summary>
		/// <param name="region"></param>
		/// <returns></returns>
		public static IdeaRS.OpenModel.Geometry2D.Region2D RegionToRegion2DIOM(Region region)
		{
			return RegionToRegion2DIOM(region.Convert());
		}

		/// <summary>
		/// conver  IdeaRS.GeometricItems.Region to the IdeaRS.OpenModel.Geometry2D.Region2
		/// </summary>
		/// <param name="region"></param>
		/// <returns></returns>
		public static IdeaRS.OpenModel.Geometry2D.Region2D RegionToRegion2DIOM(IRegion2D region)
		{
			var iomRegion = new IdeaRS.OpenModel.Geometry2D.Region2D();
			iomRegion.Openings = new List<OpenModel.Geometry2D.PolyLine2D>();
			iomRegion.Outline = GetPolyline2D(region.Outline);
			if (region.Openings != null)
			{
				foreach (var opening in region.Openings)
				{
					iomRegion.Openings.Add(GetPolyline2D(opening));
				}
			}

			return iomRegion;

		}

		public static string PolylineToString(this Polyline p)
		{
			StringBuilder sb = new StringBuilder();
			int segmentsCount = p.Segments.Count;
			for (int i = 0; i < segmentsCount; i++)
			{
				if (i > 0)
				{
					sb.Append(" ");
				}

				var s = p.Segments[i];
				if (i == (segmentsCount - 1))
				{
					// last segment
					if (s.Type == SegmentType.Line)
					{
						double x = s.Parameters[0];
						double y = s.Parameters[1];
						if (x.IsEqual(p.Segments[0].Parameters[0]) && y.IsEqual(p.Segments[0].Parameters[1]))
						{
							sb.Append("Z");
						}
						else
						{
							sb.AppendFormat("L {0} {1}", s.Parameters[0].ToString(CultureInfo.InvariantCulture), s.Parameters[1].ToString(CultureInfo.InvariantCulture));
						}
					}
					else if (s.Type == SegmentType.CircArc3Points)
					{
						sb.AppendFormat("A {0} {0} 0 1 {1} {2} {3}", s.Parameters[2].ToString(CultureInfo.InvariantCulture), s.Parameters[3].ToString(CultureInfo.InvariantCulture), s.Parameters[0].ToString(CultureInfo.InvariantCulture), s.Parameters[1].ToString(CultureInfo.InvariantCulture));
					}
				}
				else
				{
					switch (s.Type)
					{
						case SegmentType.StartPoint:
							sb.AppendFormat("M {0} {1}", s.Parameters[0].ToString(CultureInfo.InvariantCulture), s.Parameters[1].ToString(CultureInfo.InvariantCulture));
							break;
						case SegmentType.Line:
							sb.AppendFormat("L {0} {1}", s.Parameters[0].ToString(CultureInfo.InvariantCulture), s.Parameters[1].ToString(CultureInfo.InvariantCulture));
							break;
						case SegmentType.CircArc3Points:
							sb.AppendFormat("A {0} {0} 0 1 {1} {2} {3}", s.Parameters[2].ToString(CultureInfo.InvariantCulture), s.Parameters[3].ToString(CultureInfo.InvariantCulture), s.Parameters[0].ToString(CultureInfo.InvariantCulture), s.Parameters[1].ToString(CultureInfo.InvariantCulture));
							break;
						default:
							Debug.Fail("TODO");
							break;
					}
				}
			}

			return sb.ToString();
		}

		private static bool IsCollinear(this Polyline p)
		{
			var segments = p.Segments;
			var count = segments.Count;
			if (count < 3)
			{
				return false;
			}

			if (segments[0].Type != SegmentType.StartPoint || segments[0].Type != SegmentType.Line)
			{
				return false;
			}

			if (segments[1].Type != SegmentType.StartPoint || segments[1].Type != SegmentType.Line)
			{
				return false;
			}

			var p1 = GetSegmentEndPoint(segments[1]);
			var v1 = Point.Subtract(GetSegmentEndPoint(segments[0]), p1);
			Point pi;
			for (int i = 2; i < count; ++i)
			{
				if (segments[i].Type != SegmentType.StartPoint || segments[i].Type != SegmentType.Line)
				{
					return false;
				}

				pi = GetSegmentEndPoint(segments[i]);
				var vi = Point.Subtract(pi, p1);
				if (!Vector.CrossProduct(v1, vi).IsZero(1e-12))
				{
					return false;
				}

				p1 = pi;
			}

			return true;
		}

		private static Point GetSegmentEndPoint(Segment segment)
		{
			return new Point(segment.Parameters[0], segment.Parameters[1]);
		}
	}
}