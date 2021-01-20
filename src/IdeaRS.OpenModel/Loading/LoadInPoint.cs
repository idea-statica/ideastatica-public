namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Direction of load
	/// </summary>
	public enum LoadDirection
	{
		/// <summary>
		/// Direction in Local coordinate system
		/// </summary>
		InLcs,

		/// <summary>
		/// Direction in global coordinate system
		/// </summary>
		InGcs,
	}

	/// <summary>
	/// Concentrated load
	/// </summary>
	/// <example> 
	/// This sample shows how to create a load in the point.
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
	/// //Point loading impulse in the node - int rthe LCS of node = GCS
	/// LoadInPoint loadPoint = new LoadInPoint();
	/// loadPoint.Geometry = new ReferenceElement(pointB);
	/// loadPoint.Fx = -5e3; //in the global X direction
	/// 
	/// openModel.AddObject(loadPoint);
	/// loadCase.LoadsInPoint.Add(new ReferenceElement(loadPoint));
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Loading.LoadInPoint,CI.Loading", "CI.StructModel.Loading.ILoadInPoint,CI.BasicTypes")]
	public class LoadInPoint : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LoadInPoint()
		{
		}

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
		/// Gets, sets geometry
		/// </summary>
		public ReferenceElement Geometry { get; set; }

		/// <summary>
		/// Local / global
		/// </summary>
		public LoadDirection Direction { get; set; }

		///// <summary>
		///// Eccentricity Y
		///// </summary>
		//public System.Double EccY { get; set; }

		///// <summary>
		///// Eccentricity Z
		///// </summary>
		//public System.Double EccZ { get; set; }

		///// <summary>
		///// Impuls data group
		///// </summary>
		//public System.Int32 ImpulsDataGroup { get; set; }
	}
}