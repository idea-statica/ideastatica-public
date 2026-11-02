namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Load in specific point on line
	/// </summary>
	/// <example> 
	/// This sample shows how to create a point load on the line.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	/// 
	/// //Create nodes
	/// Point3D pointA = new Point3D();
	/// pointA.X = 0;
	/// pointA.Y = 0;
	/// pointA.Z = 0;
	/// openModel.AddObject(pointA);
	/// 
	/// Point3D pointB = new Point3D();
	/// pointB.X = 0;
	/// pointB.Y = 0;
	/// pointB.Z = 1.2;
	/// openModel.AddObject(pointB);
	/// 
	/// Point3D pointC = new Point3D();
	/// pointC.X = 0;
	/// pointC.Y = 0;
	/// pointC.Z = 2.4;
	/// openModel.AddObject(pointC);
	/// 
	/// //Line between nodes
	/// LineSegment3D line1 = new LineSegment3D();
	/// line1.StartPoint = new ReferenceElement(pointA);
	/// line1.EndPoint = new ReferenceElement(pointB);
	/// //LCS of line
	/// line1.LocalCoordinateSystem = new CoordSystemByZup();
	/// openModel.AddObject(line1);
	/// 
	/// //Line between nodes
	/// LineSegment3D line2 = new LineSegment3D();
	/// line2.StartPoint = new ReferenceElement(pointB);
	/// line2.EndPoint = new ReferenceElement(pointC);
	/// //LCS of line
	/// line2.LocalCoordinateSystem = new CoordSystemByZup();
	/// openModel.AddObject(line2);
	///
	/// //Load case
	/// LoadCase loadCase = new LoadCase();
	/// //...
	/// openModel.AddObject(loadCase);
	/// 
	/// //Point loading impulse on the bottom of the "line1" - in the LCS of line
	/// PointLoadOnLine pointLoadA = new PointLoadOnLine();
	/// pointLoadA.Geometry = new ReferenceElement(line1);
	/// pointLoadA.RelativePosition = 0.0;
	/// pointLoadA.Fx = 10e3;
	/// pointLoadA.Ez = 0.024;
	/// 
	/// openModel.AddObject(pointLoadA);
	/// loadCase.PointLoadsOnLine.Add(new ReferenceElement(pointLoadA));
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Loading.PointLoadOnLine,CI.Loading", "CI.StructModel.Loading.IPointLoadOnLine,CI.BasicTypes")]
	public class PointLoadOnLine : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PointLoadOnLine()
		{
		}

		/// <summary>
		/// Local / global
		/// </summary>
		public LoadDirection Direction { get; set; }

		/// <summary>
		/// Force in X direction
		/// </summary>
		public System.Double Fx { get; set; }

		/// <summary>
		/// Force in Y direction
		/// </summary>
		public System.Double Fy { get; set; }

		/// <summary>
		/// Force in Z direction
		/// </summary>
		public System.Double Fz { get; set; }

		/// <summary>
		/// Moment about the x-axis
		/// </summary>
		public System.Double Mx { get; set; }

		/// <summary>
		/// Moment about the y-axis
		/// </summary>
		public System.Double My { get; set; }

		/// <summary>
		/// Moment about the z-axis
		/// </summary>
		public System.Double Mz { get; set; }

		/// <summary>
		/// Eccentricity in Y direction
		/// </summary>
		public System.Double Ey { get; set; }

		/// <summary>
		/// Eccentricity in Z direction
		/// </summary>
		public System.Double Ez { get; set; }

		/// <summary>
		/// Segment3D or PolyLine3D
		/// </summary>
		public ReferenceElement Geometry { get; set; }

		/// <summary>
		/// Relative position on geometry line
		/// </summary>
		public System.Double RelativePosition { get; set; }
	}
}