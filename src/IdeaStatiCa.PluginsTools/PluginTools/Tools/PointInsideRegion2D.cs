using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Checks if point is inside of <c>IRegion2D</c>.
	/// </summary>
	public class PointInsideRegion2D
	{
		public enum MethodType
		{
			ByAngle,
			Mic,
		}

		public enum Result
		{
			Inside,
			Cross,
			Outside,
		}

		#region Fields

		private readonly List<OnePolygonData> outlines;
		private readonly List<OnePolygonData> openings;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// The constructor.
		/// </summary>
		/// <param name="regions">The enumerable of <c>IRegion2D</c>.</param>
		public PointInsideRegion2D(IEnumerable<IRegion2D> regions)
		{
			if (regions == null)
			{
				throw new ArgumentNullException(nameof(regions), "PointInsideRegion2D - regions can't be null");
			}

			this.IncludeOpenings = true;
			this.Method = MethodType.ByAngle;

			outlines = new List<OnePolygonData>(regions.Count());
			openings = new List<OnePolygonData>();
			var discretizator = new PolyLine2DDiscretizator();
			discretizator.NumberOfTiles = 1;
			discretizator.LengthOfTile = double.MaxValue;
			discretizator.Angle = 5;
			foreach (var region in regions)
			{
				IPolygon2D outline = new Polygon2D();
				discretizator.Discretize(region.Outline, ref outline, null);
				outlines.Add(new OnePolygonData(outline));

				foreach (var opening in region.Openings)
				{
					IPolygon2D open = new Polygon2D();
					discretizator.Discretize(opening, ref open, null);
					openings.Add(new OnePolygonData(open));
				}
			}
		}

		/// <summary>
		/// The constructor.
		/// </summary>
		/// <param name="region">The shape as <c>IRegion2D</c>.</param>
		public PointInsideRegion2D(IRegion2D region)
		{
			if (region == null)
			{
				throw new ArgumentNullException("PointInsideRegion2D");
			}

			this.IncludeOpenings = true;
			this.Method = MethodType.ByAngle;

			outlines = new List<OnePolygonData>(1);
			openings = new List<OnePolygonData>(region.Openings.Count);
			var discretizator = new PolyLine2DDiscretizator();
			discretizator.NumberOfTiles = 1;
			discretizator.LengthOfTile = double.MaxValue;
			discretizator.Angle = 5;
			IPolygon2D outline = new Polygon2D();
			discretizator.Discretize(region.Outline, ref outline, null);
			outlines.Add(new OnePolygonData(outline));

			foreach (var opening in region.Openings)
			{
				IPolygon2D open = new Polygon2D();
				discretizator.Discretize(opening, ref open, null);
				openings.Add(new OnePolygonData(open));
			}
		}

		/// <summary>
		/// Initializes a new instance of PointInsideRegion2D.
		/// </summary>
		/// <param name="polyline">The shape as polyline.</param>
		public PointInsideRegion2D(IPolyLine2D polyline)
		{
			this.IncludeOpenings = false;
			this.Method = MethodType.ByAngle;

			outlines = new List<OnePolygonData>(1);
			openings = new List<OnePolygonData>(0);
			var discretizator = new PolyLine2DDiscretizator();
			discretizator.NumberOfTiles = 1;
			discretizator.LengthOfTile = double.MaxValue;
			discretizator.Angle = 5;
			IPolygon2D outline = new Polygon2D();
			discretizator.Discretize(polyline, ref outline, null);
			outlines.Add(new OnePolygonData(outline));
		}

		/// <summary>
		/// The constructor.
		/// </summary>
		/// <param name="polygon">The shape as polygon.</param>
		public PointInsideRegion2D(IPolygon2D polygon)
		{
			this.IncludeOpenings = false;
			this.Method = MethodType.ByAngle;

			outlines = new List<OnePolygonData>(1);
			openings = new List<OnePolygonData>(0);
			outlines.Add(new OnePolygonData(polygon));
		}

		/// <summary>
		/// The constructor.
		/// </summary>
		/// <param name="polygon">The shape as polygon.</param>
		public PointInsideRegion2D(IEnumerable<Point> polygon)
		{
			this.IncludeOpenings = false;
			this.Method = MethodType.ByAngle;

			outlines = new List<OnePolygonData>(1);
			openings = new List<OnePolygonData>(0);
			outlines.Add(new OnePolygonData(new Polygon2D(polygon)));
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets or sets the information, if evaluate opening also.
		/// Default value is true;
		/// </summary>
		public bool IncludeOpenings { private get; set; }

		public MethodType Method { private get; set; }

		#endregion Properties

		#region Public methods

		/// <summary>
		/// Evaluates, if the point is inside <c>IRegion2D</c>.
		/// Use this static method only for single use, for multiple
		/// using check for non static method.
		/// </summary>
		/// <param name="region">The <c>IRegion2D</c> as outline with openings.</param>
		/// <param name="pt">The point for evaluation.</param>
		/// <param name="includeOpenings">Include/exclude openigs to the evaluation.</param>
		/// <param name="tolerance">The tolerance of calculation.</param>
		/// <returns>True, if point is inside, false otherwise.</returns>
		public static bool IsInside(IRegion2D region, ref Point pt, bool includeOpenings = true, double tolerance = 1e-12)
		{
			var discretizator = GetDiscretizator();
			IPolygon2D outline = new Polygon2D();
			discretizator.Discretize(region.Outline, ref outline, null);

			if (IsInside(outline, ref pt, tolerance))
			{
				if (includeOpenings)
				{
					IPolygon2D open = new Polygon2D();
					foreach (var opening in region.Openings)
					{
						open.Clear();
						discretizator.Discretize(opening, ref open, null);
						if (IsInside(open, ref pt, tolerance))
						{
							return false;
						}
					}
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Evaluates, if the point is inside the <c>IPolygon2D</c>.
		/// </summary>
		/// <param name="polygon">The polygon 2D.</param>
		/// <param name="pt">The point forevaluation.</param>
		/// <param name="tolerance">The tolerance of calculation.</param>
		/// <returns>True, if point is inside, false otherwise.</returns>
		public static bool IsInside(IPolygon2D polygon, ref Point pt, double tolerance = 1e-12)
		{
			double angle = 0;

			int count = polygon.Count;
			var begVec = Point.Subtract(pt, polygon[0]);
			for (int i = 1; i < count; ++i)
			{
				var endVec = Point.Subtract(pt, polygon[i]);
				angle += Vector.AngleBetween(begVec, endVec);
				begVec = endVec;
			}

			if (Math.Abs(angle).IsGreaterOrEqual(180))
			{
				return true;
			}

			var begPoint = polygon[0];
			for (int i = 1; i < count; ++i)
			{
				var endPoint = polygon[i];
				if (GeomTools2D.PointToAbscissa(ref pt, ref begPoint, ref endPoint, tolerance))
				{
					return true;
				}

				begPoint = endPoint;
			}

			return false;
		}

		/// <summary>
		/// Evaluates, if the point is inside shape, defined in constructor.
		/// </summary>
		/// <param name="pt">The point for evaluation.</param>
		/// <param name="tolerance">The tolerance of calculation.</param>
		/// <returns>True, if point is inside, false otherwise.</returns>
		public bool IsInside(ref Point pt, double tolerance = 1e-12)
		{
			if (this.IncludeOpenings)
			{
				for (int i = 0; i < this.openings.Count; i++)
				{
					if (this.Method == MethodType.ByAngle)
					{
						if (IsInside(this.openings[i].Polygon, ref pt, tolerance))
						{
							return false;
						}
					}
					else
					{
						if (this.openings[i].IsInside(ref pt))
						{
							return false;
						}
					}
				}
			}

			for (int i = 0; i < this.outlines.Count; i++)
			{
				if (this.Method == MethodType.ByAngle)
				{
					if (IsInside(this.outlines[i].Polygon, ref pt, tolerance))
					{
						return true;
					}
				}
				else
				{
					if (this.outlines[i].IsInside(ref pt))
					{
						return true;
					}
				}
			}

			return false;
		}

		public Result IsInside(IPolyLine2D polyline, double tolerance = 1e-12)
		{
			var discretizator = GetDiscretizator();
			IPolygon2D polygon = new Polygon2D();
			discretizator.Discretize(polyline, ref polygon, null);

			bool inside = false;
			bool first = true;
			var size = polygon.Count - 1;
			if (this.IncludeOpenings)
			{
				for (int j = 0; j < this.openings.Count; j++)
				{
					for (var i = size; i >= 0; --i)
					{
						var pt = polygon[i];
						var ins = this.Method == MethodType.ByAngle ? !IsInside(this.openings[j].Polygon, ref pt, tolerance) : !this.openings[j].IsInside(ref pt);
						if (first)
						{
							inside = ins;
							first = false;
						}
						else if (ins != inside)
						{
							return Result.Cross;
						}
					}
				}
			}

			for (int j = 0; j < this.outlines.Count; j++)
			{
				for (var i = size; i >= 0; --i)
				{
					var pt = polygon[i];
					var ins = this.Method == MethodType.ByAngle ? IsInside(this.outlines[j].Polygon, ref pt, tolerance) : this.outlines[j].IsInside(ref pt);
					if (first)
					{
						inside = ins;
						first = false;
					}
					else if (ins != inside)
					{
						return Result.Cross;
					}
				}
			}

			return inside ? Result.Inside : Result.Outside;
		}

		#endregion Public methods

		private static PolyLine2DDiscretizator GetDiscretizator()
		{
			var discretizator = new PolyLine2DDiscretizator();
			discretizator.NumberOfTiles = 1;
			discretizator.LengthOfTile = double.MaxValue;
			discretizator.Angle = 5;
			return discretizator;
		}

		#region Inner class

		private class OnePolygonData
		{
			private readonly IPolygon2D polygon;
			private double[][] myab;
			private sbyte[] myib;

			public OnePolygonData(IPolygon2D polygon)
			{
				this.polygon = polygon;
				Initialize();
			}

			public IPolygon2D Polygon
			{
				get
				{
					return polygon;
				}
			}

			public bool IsInside(ref Point pt)
			{
				int j, k, kk, n = polygon.Count - 1;
				double x = pt.X;
				double y = pt.Y;
				for (kk = 0, j = 0; j < n; j++)
				{
					if (myib[j] > 0)
					{
						continue;
					}

					if (myib[j] < 0)
					{
						if ((x > polygon[j].X) && (x <= polygon[j + 1].X))
						{
							if ((myab[j][0] * x) + myab[j][1] < y)
							{
								k = 1;
							}
							else
							{
								k = -1;
							}
						}
						else
						{
							continue;
						}
					}
					else
					{
						if ((x > polygon[j + 1].X) && (x <= polygon[j].X))
						{
							if ((myab[j][0] * x) + myab[j][1] >= y)
							{
								k = 1;
							}
							else
							{
								k = -1;
							}
						}
						else
						{
							continue;
						}
					}

					kk += k;
				}

				return kk != 0;
			}

			private void Initialize()
			{
				int n = polygon.Count;
				double db;

				// inicializuji pamet
				myib = new sbyte[n - 1];
				myab = new double[n - 1][];

				// inicializuji vnitrni data
				for (int i = 0; i < n - 1; ++i)
				{
					db = polygon[i + 1].X - polygon[i].X;
					if (db == 0)
					{
						myib[i] = 1;
						continue;
					}

					if (db < 0)
					{
						myib[i] = 0;
					}
					else
					{
						myib[i] = -1;
					}

					myab[i] = new double[2];
					myab[i][0] = (polygon[i + 1].Y - polygon[i].Y) / db;
					myab[i][1] = polygon[i].Y - (myab[i][0] * polygon[i].X);
				}
			}
		}

		#endregion Inner class
	}
}