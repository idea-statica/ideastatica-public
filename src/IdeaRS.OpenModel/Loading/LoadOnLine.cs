namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Load on line
	/// </summary>
	/// <example> 
	/// This sample shows how to create a load on the line.
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
	/// //Uniform load on line2 - relative between 0.1-0.6. In the LCS of line. -2kN in the z-direction
	/// LoadOnLine loadLine = new LoadOnLine();
	/// loadLine.Bimp = new LoadImpulse() { Z = -2e3 };
	/// loadLine.Eimp = new LoadImpulse() { Z = -2e3 };
	/// loadLine.Type = LoadType.LoadForce;
	/// loadLine.Direction = LoadDirection.InLcs;
	/// loadLine.Geometry = new ReferenceElement(line2);
	/// loadLine.LoadProjection = LoadProjection.Length;
	/// loadLine.RelativeBeginPosition = 0.1;
	/// loadLine.RelativeBeginPosition = 0.6;
	/// 
	/// openModel.AddObject(loadLine);
	/// loadCase.LoadsOnLine.Add(new ReferenceElement(loadLine));
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Loading.LoadOnLine,CI.Loading", "CI.StructModel.Loading.ILoadOnLine,CI.BasicTypes")]
	public class LoadOnLine : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LoadOnLine()
		{
		}

		/// <summary>
		/// Begin position on Segment3D
		/// </summary>
		public System.Double RelativeBeginPosition { get; set; }

		/// <summary>
		/// End position on Segment3D
		/// </summary>
		public System.Double RelativeEndPosition { get; set; }

		/// <summary>
		/// Eccentricity local Y on the beginning
		/// </summary>
		public System.Double ExY { get; set; }

		/// <summary>
		/// Eccentricity local Z on the beginning
		/// </summary>
		public System.Double ExZ { get; set; }

		/// <summary>
		/// Eccentricity local Y at the end
		/// </summary>
		public System.Double ExYEnd { get; set; }

		/// <summary>
		/// Eccentricity local Z at the end
		/// </summary>
		public System.Double ExZEnd { get; set; }

		/// <summary>
		/// Type of load
		/// </summary>
		public LoadType Type { get; set; }

		/// <summary>
		/// 1=global, 0=local
		/// </summary>
		public LoadDirection Direction { get; set; }

		/// <summary>
		/// Impulse at the begin
		/// </summary>
		public LoadImpulse Bimp { get; set; }

		/// <summary>
		/// Impulse at the end
		/// </summary>
		public LoadImpulse Eimp { get; set; }

		/// <summary>
		/// Segment3D or PolyLine3D
		/// </summary>
		public ReferenceElement Geometry { get; set; }

		/// <summary>
		/// Gets, sets load projection
		/// </summary>
		public LoadProjection LoadProjection { get; set; }
	}

	/// <summary>
	/// LoadImpulse
	/// </summary>
	public struct LoadImpulse
	{
		/// <summary>
		/// Load in X direction
		/// </summary>
		public System.Double X { get; set; }

		/// <summary>
		/// Load in Y direction
		/// </summary>
		public System.Double Y { get; set; }

		/// <summary>
		/// Load in Z direction
		/// </summary>
		public System.Double Z { get; set; }
	}

	/// <summary>
	/// Type of load projection
	/// </summary>
	public enum LoadProjection
	{
		/// <summary>
		/// Impuls on member length
		/// </summary>
		Length,

		/// <summary>
		/// Impuls on member projection
		/// </summary>
		Projection,
	}

	/// <summary>
	/// Type of load impuls
	/// </summary>
	public enum LoadType
	{
		/// <summary>
		/// Force load impuls
		/// </summary>
		LoadForce,

		/// <summary>
		/// Moment load impuls
		/// </summary>
		LoadMoment,
	}
}