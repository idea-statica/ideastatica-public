using IdeaRS.OpenModel.Geometry2D;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Type edge position
	/// </summary>
	public enum TypeEdgePosition : int
	{
		/// <summary>
		/// whole length
		/// </summary>
		WholeLength,

		/// <summary>
		/// part of the edge, mereno od pocatku hrany nebo podle zadani v jinem properties
		/// </summary>
		PartEdge,

		/// <summary>
		/// parts of the edge to end edge - pouze na vybranem vlasniku - napriklad u linioveho zatizeni na hrane
		/// </summary>
		PartEdgeToEndEdge,

		/// <summary>
		/// parts on edge, protazene az k segmentu nebo k poslenimu pruseciku s vnejsim obrysem - napriklad u vyztuze
		/// </summary>
		PartEdgeToSegment,

		/// <summary>
		/// on part of edge measured from end of the edge
		/// </summary>
		PartEdgeFromEnd
	}

	/// <summary>
	/// Line support
	/// </summary>
	public class LineSupport : OpenElementId
	{
		/// <summary>
		/// constructor
		/// </summary>
		public LineSupport()
		{
			//TypeLineSupport = Data.TypeLineSupport.Default;
			IsUserStiffnessX = false;
			X = true;
			IsUserStiffnessY = false;
			IsPressureOnlyY = true;
			Y = true;
			Rz = false;
			Direction = IdeaRS.OpenModel.Loading.LoadDirection.InLcs;
			GeometryPointsPath = null;
			OnWallEdge = true;
			MasterEdge = 0;
			EdgePositionType = EdgeOrientationType.FromBegin;
			PositionOnEdge = 0.0;
			TypeEdgePosition = TypeEdgePosition.WholeLength;
			SupportLength = 1.0;
			GeneralLine = null;
			StiffnessX = 0.0;
			StiffnessY = 0.0;
			StiffnessRz = 0.0;
		}

		/// <summary>
		/// Gets or set the name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// displacement in X
		/// </summary>
		public bool X { get; set; }

		/// <summary>
		/// displacement in Y
		/// </summary>
		public bool Y { get; set; }

		/// <summary>
		/// rotation Rz
		/// </summary>
		public bool Rz { get; set; }

		/// <summary>
		/// true for user stiffness
		/// </summary>
		public bool IsUserStiffnessX { get; set; }

		/// <summary>
		/// support stiffness X
		/// </summary>
		public double StiffnessX { get; set; }

		/// <summary>
		/// true for user stiffness
		/// </summary>
		public bool IsUserStiffnessY { get; set; }

		/// <summary>
		/// support stiffness Y
		/// </summary>
		public double StiffnessY { get; set; }

		/// <summary>
		/// pressure only for Y
		/// </summary>
		public bool IsPressureOnlyY { get; set; }

		/// <summary>
		/// support stiffness Rz
		/// </summary>
		public double StiffnessRz { get; set; }

		/// <summary>
		/// local / global
		/// </summary>
		public IdeaRS.OpenModel.Loading.LoadDirection Direction { get; set; }

		/// <summary>
		/// list of line load points
		/// </summary>
		public Polygon2D GeometryPointsPath { get; set; }

		/// <summary>
		/// true to add line support on wall edge
		/// </summary>
		public bool OnWallEdge { get; set; }

		/// <summary>
		/// Id representing geometrical parts of Detail
		/// </summary>
		public int GeomId { get; set; }

		/// <summary>
		/// Gets or sets the master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// master component edge
		/// </summary>
		public int MasterEdge { get; set; }

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
		/// support lengtj
		/// </summary>
		public double SupportLength { get; set; }

		/// <summary>
		/// general shape
		/// </summary>
		public PolyLine2D GeneralLine { get; set; }
	}
}
