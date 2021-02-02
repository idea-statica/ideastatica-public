using ClipperLib;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CI.Geometry2D
{
	public sealed class ClipperController : ClipperControllerBase
	{
		private PolyFillType clipFillType = PolyFillType.pftNonZero;
		private readonly Clipper clipper = new Clipper();
		private PolyFillType subjFillType = PolyFillType.pftNonZero;

		public ClipperController(double maxDiscretizationAngle = 5)
			: base(maxDiscretizationAngle)
		{
		}

		public ClipperController(IRegion2D subject, IRegion2D clip, double maxDiscretizationAngle = 5)
			: base(maxDiscretizationAngle)
		{
			this.Add(subject, PolyType.ptSubject);
			this.Add(clip, PolyType.ptClip);
		}

		public PolyFillType ClipFillType
		{
			get { return this.clipFillType; }
			set { this.clipFillType = value; }
		}

		public PolyFillType SubjFillType
		{
			get { return this.subjFillType; }
			set { this.subjFillType = value; }
		}

		public static IRegion2D Simplify(IRegion2D region, double maxDiscretizationAngle = 5)
		{
			PolyLine2DDiscretizator discretizator = new PolyLine2DDiscretizator
			{
				NumberOfTiles = 1,
				LengthOfTile = double.MaxValue,
				Angle = maxDiscretizationAngle
			};
			var polyline = region.Outline;
			var containsCircArc = polyline.Segments.FirstOrDefault(s => s is CircularArcSegment2D) != null;
			var polygon = CreatePolygon(polyline, discretizator, true);
			var solution = Clipper.SimplifyPolygon(polygon);
			if (solution.Count == 1)
			{
				var newRegion = new Region2D(CreatePolyline(solution[0], containsCircArc, maxDiscretizationAngle + 1));

				foreach (var opening in region.Openings)
				{
					containsCircArc = opening.Segments.FirstOrDefault(s => s is CircularArcSegment2D) != null;
					polygon = CreatePolygon(opening, discretizator, true);
					solution = Clipper.SimplifyPolygon(polygon);
					if (solution.Count == 1)
					{
						newRegion.Openings.Add(CreatePolyline(solution[0], containsCircArc, maxDiscretizationAngle + 1));
					}
					else
					{
						throw new System.NotImplementedException();
					}
				}

				return newRegion;
			}

			throw new System.NotImplementedException();
		}

		public void Add(IRegion2D region, PolyType type)
		{
			AddRegion(region, type);
		}

		public void Add(IEnumerable<IRegion2D> regions, PolyType type)
		{
			regions.ForEach(r => AddRegion(r, type));
		}

		public void Add(IEnumerable<IPolyLine2D> polylines, PolyType type)
		{
			polylines.ForEach(p => Add(p, type));
		}

		public void Add(IPolyLine2D polyline, PolyType type)
		{
			if (!this.containsCircArc)
				this.containsCircArc |= polyline.Segments.FirstOrDefault(s => s is CircularArcSegment2D) != null;

			var p = CreatePolygon(polyline, this.Discretizator, false);
			this.clipper.AddPath(p, type, polyline.IsClosed);
		}

		public void Add(IPolygon2D polygon, PolyType type)
		{
			var p = CreatePolygon(polygon);
			this.clipper.AddPath(p, type, polygon.IsClosed);
		}

		public void Add(IEnumerable<IPolygon2D> polygons, PolyType type)
		{
			foreach (var polygon in polygons)
			{
				this.Add(polygon, type);
			}
		}

		public void Add(IRegion2D subject, IRegion2D clip)
		{
			AddRegion(subject, PolyType.ptSubject);
			AddRegion(clip, PolyType.ptClip);
		}

		public void Clear()
		{
			this.clipper.Clear();
		}

		public IEnumerable<IRegion2D> Difference()
		{
			return this.Execute(ClipType.ctDifference);
		}

		public IEnumerable<IPolygon2D> DifferencePolygons()
		{
			return this.ExecutePolygons(ClipType.ctDifference);
		}

		public IEnumerable<IRegion2D> Execute(ClipType clipType)
		{
			var solution = new PolyTree();
			bool succeeded = this.clipper.Execute(clipType, solution, this.subjFillType, this.clipFillType);
			if (succeeded)
			{
				foreach (var ch in solution.Childs)
				{
					var regions = CreateRegions(ch, this.containsCircArc, MaxDiscretizationAngle + 1, TryRecoverCircArcs);
					foreach (var r in regions)
					{
						yield return r;
					}
				}
			}

			yield break;
		}

		public IEnumerable<IPolygon2D> ExecutePolygons(ClipType clipType)
		{
			var solution = new PolyTree();
			bool succeeded = this.clipper.Execute(clipType, solution, this.subjFillType, this.clipFillType);
			if (succeeded)
			{
				foreach (var ch in solution.Childs)
				{
					var polygons = CreatePolygons(ch);
					foreach (var p in polygons)
					{
						yield return p;
					}
				}
			}

			yield break;
		}

		public IEnumerable<IRegion2D> Intersection()
		{
			return this.Execute(ClipType.ctIntersection);
		}

		public IEnumerable<IPolygon2D> IntersectionPolygons()
		{
			return this.ExecutePolygons(ClipType.ctIntersection);
		}

		public IEnumerable<IRegion2D> Union()
		{
			return this.Execute(ClipType.ctUnion);
		}

		public IEnumerable<IPolygon2D> UnionPolygons()
		{
			return this.ExecutePolygons(ClipType.ctUnion);
		}

		public IEnumerable<IRegion2D> Xor()
		{
			return this.Execute(ClipType.ctXor);
		}

		public IEnumerable<IPolygon2D> XorPolygons()
		{
			return this.ExecutePolygons(ClipType.ctXor);
		}

		private void AddRegion(IRegion2D r, PolyType polyType)
		{
			var polygons = CreatePolygons(r);
			foreach (var poly in polygons)
			{
				this.clipper.AddPath(poly, polyType, true);
			}
		}
	}

	public abstract class ClipperControllerBase
	{
		protected const double ClipperScale = 1e8;

		protected bool containsCircArc = false;

		private PolyLine2DDiscretizator discretizator;

		protected ClipperControllerBase(double maxDiscretizationAngle = 5)
		{
			this.MaxDiscretizationAngle = maxDiscretizationAngle;
		}

		protected PolyLine2DDiscretizator Discretizator
		{
			get
			{
				if (this.discretizator == null)
				{
					this.discretizator = new PolyLine2DDiscretizator { Angle = this.MaxDiscretizationAngle };
				}

				return this.discretizator;
			}
		}

		protected double MaxDiscretizationAngle { get; }

		public bool TryRecoverCircArcs { get; set; } = true;

		protected static List<IntPoint> CreatePolygon(IPolyLine2D polyline, PolyLine2DDiscretizator discretizator, bool counterClockwise)
		{
			IPolygon2D polygon = new Polygon2D();
			discretizator.Discretize(polyline, ref polygon, null);
			return CreatePolygon(polygon, counterClockwise);
		}

		protected static List<IntPoint> CreatePolygon(IPolygon2D polygon)
		{
			var count = polygon.Count;
			var cpolygon = new List<IntPoint>(count);
			for (int i = 0; i < count; ++i)
			{
				cpolygon.Add(new IntPoint((long)(polygon[i].X * ClipperScale), (long)(polygon[i].Y * ClipperScale)));
			}

			return cpolygon;
		}

		protected static IEnumerable<IPolygon2D> CreatePolygons(PolyNode polynode)
		{
			yield return CreatePolygon(polynode.Contour);

			foreach (var ch in polynode.Childs)
			{
				var polygons = CreatePolygons(ch);
				foreach (var p in polygons)
				{
					yield return p;
				}
			}
		}

		protected static IPolyLine2D CreatePolyline(List<IntPoint> cpolyline, bool containsCircArc, double maxCircArcAngle, bool tryRecoverCircArc = true)
		{
			IPolyLine2D polyline;
			if (containsCircArc && tryRecoverCircArc)
			{
				polyline = GeomTools2D.CreatePolyline(CreatePolygon(cpolyline), maxCircArcAngle);
			}
			else
			{
				var count = cpolyline.Count;
				polyline = new PolyLine2D(count);
				if (count > 0)
				{
					polyline.StartPoint = new IdaComPoint2D(cpolyline[0].X / ClipperScale, cpolyline[0].Y / ClipperScale);
					var segments = polyline.Segments;
					for (int i = 1; i < count; ++i)
					{
						segments.Add(new LineSegment2D(new Point(cpolyline[i].X / ClipperScale, cpolyline[i].Y / ClipperScale)));
					}
				}
			}

			polyline.Close();
			return polyline;
		}

		protected static IEnumerable<IRegion2D> CreateRegions(PolyNode polynode, bool containsCircArc, double maxCircArcAngle, bool tryRecoverCircArc = true)
		{
			var region = new Region2D(CreatePolyline(polynode.Contour, containsCircArc, maxCircArcAngle, tryRecoverCircArc));
			yield return region;

			foreach (var ch in polynode.Childs)
			{
				if (ch.IsHole)
				{
					region.Openings.Add(CreatePolyline(ch.Contour, containsCircArc, maxCircArcAngle, tryRecoverCircArc));
					foreach (var ch2 in ch.Childs)
					{
						var regions = CreateRegions(ch2, containsCircArc, maxCircArcAngle, tryRecoverCircArc);
						foreach (var r in regions)
						{
							yield return r;
						}
					}
				}
				else
				{
					var regions = CreateRegions(ch, containsCircArc, maxCircArcAngle, tryRecoverCircArc);
					foreach (var r in regions)
					{
						yield return r;
					}
				}
			}
		}

		protected IEnumerable<List<IntPoint>> CreatePolygons(IRegion2D region)
		{
			var discretizator = this.Discretizator;

			containsCircArc |= region.Outline.Segments.FirstOrDefault(s => s is CircularArcSegment2D) != null;
			yield return CreatePolygon(region.Outline, discretizator, true);

			int openingsCount = region.Openings.Count;
			if (openingsCount > 0)
			{
				for (int i = 0; i < openingsCount; ++i)
				{
					containsCircArc |= region.Openings[i].Segments.FirstOrDefault(s => s is CircularArcSegment2D) != null;
					yield return CreatePolygon(region.Openings[i], discretizator, false);
				}
			}
		}

		private static List<IntPoint> CreatePolygon(IPolygon2D polygon, bool counterClockwise)
		{
			if (polygon.IsCounterClockwise != counterClockwise)
			{
				polygon.Reverse();
			}

			return CreatePolygon(polygon);
		}

		private static IPolygon2D CreatePolygon(List<IntPoint> cpolyline)
		{
			var count = cpolyline.Count;
			var polygon = new Polygon2D(count);
			for (var i = 0; i < count; ++i)
			{
				polygon.Add(new Point(cpolyline[i].X / ClipperScale, cpolyline[i].Y / ClipperScale));
			}

			return polygon;
		}
	}

	public sealed class OffsetController : ClipperControllerBase
	{
		private ClipperOffset clipper = new ClipperOffset();

		public OffsetController(double maxDiscretizationAngle = 5)
			: base(maxDiscretizationAngle)
		{
		}

		public static IEnumerable<IPolyLine2D> BuildOffset(IPolygon2D polygon, double delta, JoinType joinType, EndType endType, double mitterLimit, double maxDiscretizationAngle = 5)
		{
			bool containsCircArc = false;
			var p = CreatePolygon(polygon);

			if (joinType == JoinType.jtRound || endType == EndType.etOpenRound)
				containsCircArc = true;

			var co = new ClipperOffset(mitterLimit);
			co.AddPath(p, joinType, endType);
			var solution = new List<List<IntPoint>>();
			co.Execute(ref solution, delta * ClipperScale);
			foreach (var poly in solution)
			{
				yield return CreatePolyline(poly, containsCircArc, maxDiscretizationAngle + 1);
			}
		}

		public static IEnumerable<IPolyLine2D> BuildOffset(IEnumerable<IPolygon2D> pgons, double delta, JoinType joinType, EndType endType, double mitterLimit, double maxDiscretizationAngle = 5)
		{
			bool containsCircArc = false;
			var polygons = new List<List<IntPoint>>(pgons.Count());
			foreach (var polygon in pgons)
			{
				var p = CreatePolygon(polygon);
				polygons.Add(p);
			}

			if (joinType == JoinType.jtRound || endType == EndType.etOpenRound)
				containsCircArc = true;

			var co = new ClipperOffset(mitterLimit);
			co.AddPaths(polygons, joinType, endType);
			var solution = new List<List<IntPoint>>();
			co.Execute(ref solution, delta * ClipperScale);
			foreach (var poly in solution)
			{
				yield return CreatePolyline(poly, containsCircArc, maxDiscretizationAngle + 1);
			}
		}

		public static IEnumerable<IPolyLine2D> BuildOffset(IPolyLine2D polyline, double delta, JoinType joinType, EndType endType, double mitterLimit, bool sort = false, double maxDiscretizationAngle = 5)
		{
			PolyLine2DDiscretizator discretizator = new PolyLine2DDiscretizator
			{
				NumberOfTiles = 1,
				LengthOfTile = double.MaxValue,
				Angle = maxDiscretizationAngle
			};
			var p = CreatePolygon(polyline, discretizator, true);

			bool containsCircArc = joinType == JoinType.jtRound || endType == EndType.etOpenRound;
			if (!containsCircArc)
				containsCircArc = polyline.Segments.FirstOrDefault(s => s is CircularArcSegment2D) != null;

			var co = new ClipperOffset(mitterLimit);
			co.AddPath(p, joinType, endType);
			var solution = new List<List<IntPoint>>();
			co.Execute(ref solution, delta * ClipperScale);
			if (sort && polyline.IsClosed && !containsCircArc && joinType == JoinType.jtMiter && (endType == EndType.etClosedLine || endType == EndType.etClosedPolygon) && solution.Count == 1)
			{
				// try to sort offset path according to source path
				var newPolyline = CreatePolyline(solution[0], containsCircArc, maxDiscretizationAngle + 1);
				TrySortSegments(polyline, newPolyline, delta);
				yield return newPolyline;
			}
			else
			{
				foreach (var polygon in solution)
				{
					yield return CreatePolyline(polygon, containsCircArc, maxDiscretizationAngle + 1);
				}
			}
		}

		public static IEnumerable<IPolyLine2D> BuildOffset(IEnumerable<IPolyLine2D> polylines, double delta, JoinType joinType, EndType endType, double mitterLimit, bool sort = false, double maxDiscretizationAngle = 5)
		{
			PolyLine2DDiscretizator discretizator = new PolyLine2DDiscretizator
			{
				NumberOfTiles = 1,
				LengthOfTile = double.MaxValue,
				Angle = maxDiscretizationAngle
			};
			bool containsCircArc = joinType == JoinType.jtRound || endType == EndType.etOpenRound;
			var polygons = new List<List<IntPoint>>(polylines.Count());
			foreach (var polyline in polylines)
			{
				if (!containsCircArc)
					containsCircArc |= polyline.Segments.FirstOrDefault(s => s is CircularArcSegment2D) != null;

				var polygon = CreatePolygon(polyline, discretizator, true);
				polygons.Add(polygon);
			}

			var co = new ClipperOffset(mitterLimit);
			co.AddPaths(polygons, joinType, endType);
			var solution = new List<List<IntPoint>>();
			co.Execute(ref solution, delta * ClipperScale);
			foreach (var polygon in solution)
			{
				yield return CreatePolyline(polygon, containsCircArc, maxDiscretizationAngle + 1);
			}
		}

		/// <summary>
		/// Ofset regionu.
		/// </summary>
		/// <param name="region"></param>
		/// <param name="delta"></param>
		/// <param name="joinType"></param>
		/// <param name="endType">Pokud bude etClosed, tak dela ofset vcetne otvoru, kladne delta je ven, zaporne dovnitr.
		/// Pokud nebude etClosed, tak dela ofset outline dovnitr i ven.</param>
		/// <param name="mitterLimit"></param>
		/// <returns></returns>
		public IRegion2D BuildOffset(IRegion2D region, double delta, JoinType joinType, EndType endType, double mitterLimit)
		{
			var polygons = CreatePolygons(region).ToList();
			var co = new ClipperOffset(mitterLimit);
			co.AddPaths(polygons, joinType, endType);
			var solution = new List<List<IntPoint>>();
			co.Execute(ref solution, delta * ClipperScale);
			if (solution.Count > 0)
			{
				var outline = CreatePolyline(solution[0], containsCircArc, MaxDiscretizationAngle + 1);
				var openings = new List<IPolyLine2D>(solution.Count - 1);
				for (var i = 1; i < solution.Count; ++i)
				{
					openings.Add(CreatePolyline(solution[i], containsCircArc, MaxDiscretizationAngle + 1));
				}

				var result = new Region2D(outline, openings);
				return result;
			}

			return null;
		}

		public void Clear()
		{
			this.clipper.Clear();
		}

		private static bool TrySortSegments(IPolyLine2D pattern, IPolyLine2D polyline, double offset)
		{
			if (pattern.Segments.Count == polyline.Segments.Count && pattern.Segments.Count > 0)
			{
				Point patStart = pattern.StartPoint;
				var patSegment = pattern.Segments[0];
				var patU = patSegment.EndPoint - patStart;
				patU.Normalize();
				Point start = polyline.StartPoint;
				for (var i = 0; i < polyline.Segments.Count; ++i)
				{
					var segment = polyline.Segments[i];
					var u = segment.EndPoint - start;
					var t = Vector.CrossProduct(patU, u);
					if (t.IsZero(1e-4))
					{
						// jsou rovnobezne, zkusime vzdalenost
						t = GeomTools2D.LineToPointDistance(ref patStart, ref patU, ref start);
						if (t.IsEqual(System.Math.Abs(offset), 0.001))
						{
							// mam segment odpovidajici prvnimu ve vzorove geometrii
							u.Normalize();
							if (!u.X.IsEqual(patU.X, 0.1) || !u.Y.IsEqual(patU.Y, 0.1))
							{
								// jsou opracne orientovane
								start = segment.EndPoint;
								GeomTools2D.ReversePolyline(ref polyline);
							}

							if (i == 0)
							{
								return true;
							}

							var circ = polyline.Segments.AsCircular();
							var rotated = circ.Skip(i).Take(polyline.Segments.Count).ToList();
							polyline.Segments.Clear();
							(polyline.Segments as List<ISegment2D>).AddRange(rotated);
							polyline.StartPoint = start;
							return true;
						}
					}

					start = segment.EndPoint;
				}
			}

			return false;
		}
	}
}