using IdeaRS.OpenModel.Geometry2D;

namespace IdeaRS.OpenModel.Detail.Loading
{
	/// <summary>
	/// type position line support
	/// </summary>
	public enum TypeLinePosition : int
	{
		/// <summary>
		/// on point (0, 0) or selected point
		/// </summary>
		Point,

		/// <summary>
		/// on wall edge
		/// </summary>
		OnWallEdge,

		/// <summary>
		/// polyline
		/// </summary>
		Polyline
	}

	/// <summary>
	/// Direction type
	/// </summary>
	public enum DirectionType
	{
		/// <summary>
		/// GlobalX
		/// </summary>
		GlobalX = 0,

		/// <summary>
		/// Global Y
		/// </summary>
		GlobalY,

		/// <summary>
		/// Global Z
		/// </summary>
		GlobalZ,

		/// <summary>
		/// Local X
		/// </summary>
		LocalX,

		/// <summary>
		/// Local Y
		/// </summary>
		LocalY,

		/// <summary>
		/// Local Z
		/// </summary>
		LocalZ,
	}

	/// <summary>
	/// Line load
	/// </summary>
	public class LineLoad : LoadBase
	{
		/// <summary>
		/// constructor
		/// </summary>
		public LineLoad()
		{
			BegFx = 0.0;
			EndFx = 0.0;
			BegFy = 0.0;
			BegFy = 0.0;
			ResultantForces = "-10000";
			Direction = IdeaRS.OpenModel.Loading.LoadDirection.InGcs;
			GeometryPointsPath = null;
			TypeLinePosition = TypeLinePosition.OnWallEdge;
			//MasterComponentPath = string.Empty;
			MasterEdge = 2;
			MasterPoint = 0;
			TypeEdgePosition = TypeEdgePosition.WholeLength;
			PositionOnEdge = 0.0;
			LengthOnEdge = 1.0;
			GeneralLine = null;
			//LineVertexesInputData = new Line2DInputData();
		}

		/// <summary>
		/// Gets or set the name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// force in X at biginning
		/// </summary>
		public double BegFx { get; set; }

		/// <summary>
		/// force in X at end
		/// </summary>
		public double EndFx { get; set; }

		/// <summary>
		/// force in Y at beginning
		/// </summary>
		public double BegFy { get; set; }

		/// <summary>
		/// force in Y at end
		/// </summary>
		public double EndFy { get; set; }

		/// <summary>
		/// local / global
		/// </summary>
		public IdeaRS.OpenModel.Loading.LoadDirection Direction { get; set; }

		/// <summary>
		/// list of line load points
		/// </summary>
		public Polygon2D GeometryPointsPath { get; set; }

		/// <summary>
		/// forces value
		/// </summary>
		public string ResultantForces { get; set; }

		/// <summary>
		/// point support position X
		/// </summary>
		public double BegPositionX { get; set; }

		/// <summary>
		/// point support position Y
		/// </summary>
		public double BegPositionY { get; set; }

		/// <summary>
		/// point support position X
		/// </summary>
		public double EndPositionX { get; set; }

		/// <summary>
		/// point support position Y
		/// </summary>
		public double EndPositionY { get; set; }

		/// <summary>
		/// direction type of load
		/// </summary>
		public DirectionType DirectionType { get; set; }

		/// <summary>
		/// angle of load
		/// </summary>
		public double Angle { get; set; }

		/// <summary>
		/// type of position
		/// </summary>
		public TypeLinePosition TypeLinePosition { get; set; }

		/// <summary>
		/// Gets or sets the master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// master component edge
		/// </summary>
		public int MasterEdge { get; set; }

		/// <summary>
		/// master point
		/// </summary>
		public int MasterPoint { get; set; }

		/// <summary>
		/// edge position type
		/// </summary>
		public TypeEdgePosition TypeEdgePosition { get; set; }

		/// <summary>
		/// edge position type
		/// </summary>
		public EdgeOrientationType EdgePositionType { get; set; }

		/// <summary>
		/// position od edge
		/// </summary>
		public double PositionOnEdge { get; set; }

		/// <summary>
		/// length of load
		/// </summary>
		public double LengthOnEdge { get; set; }

		/// <summary>
		/// general shape
		/// </summary>
		public PolyLine2D GeneralLine { get; set; }

		///// <summary>
		///// 2D line load defined by vertexes data
		///// </summary>
		//[DataMember]
		//[AggregatedPropWrp("ILine2DInputParam, Line2DInputWrp, Line2DInputParam")]
		//public Line2DInputData LineVertexesInputData { get; set; }
	}
}