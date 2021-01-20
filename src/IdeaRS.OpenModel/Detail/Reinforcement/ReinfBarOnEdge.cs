using IdeaRS.OpenModel.Geometry3D;

namespace IdeaRS.OpenModel.Detail
{

	/// <summary>
	/// Position of reinforcement on edge
	/// </summary>
	public enum EdgePositionType
	{
		/// <summary>
		/// WholeLength
		/// </summary>
		WholeLength,

		/// <summary>
		/// Part edge from beginning
		/// </summary>
		PartEdgeFromBegin,

		/// <summary>
		/// Part edge from end
		/// </summary>
		PartEdgeFromEnd,

		/// <summary>
		/// Part edge to the end
		/// </summary>
		PartEdgeToSegment
	}

	/// <summary>
	/// Type of extension of bar
	/// </summary>
	public enum BarEndExtendType
	{
		/// <summary>
		/// No extension
		/// </summary>
		No,

		/// <summary>
		/// Extension to the end of segment
		/// </summary>
		ExtendToSegment,

		/// <summary>
		/// Extension by distance
		/// </summary>
		ExtendByDistance
	}

	/// <summary>
	/// Representing reinforcement bar input on edge in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Reinforcement))]
	public class ReinfBarOnEdge : ReinforcementGroup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReinfBarOnEdge()
		{

		}

		/// <summary>
		/// Master component of reinforcement
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Master edge on Outline of MasterComponent
		/// </summary>
		public ReferenceElement MasterEdge { get; set; }

		/// <summary>
		/// Vector determining direction of reinforcement layers, vector is defined in local coordinates of first segment
		/// </summary>
		public Vector3D BarDistance { get; set; }

		/// <summary>
		/// Number of layers
		/// </summary>
		public int NumOfLayers { get; set; }

		/// <summary>
		/// Distance between reinforcement layers
		/// </summary>
		public double Distance { get; set; }

		/// <summary>
		/// Position of reinforcement on edge
		/// </summary>
		public EdgePositionType EdgePositionType { get; set; }

		/// <summary>
		/// Position of beginning of bar from start or end of edge
		/// </summary>
		public double PositionOnEdge { get; set; }

		/// <summary>
		/// length of bar
		/// </summary>
		public double Length { get; set; }

		/// <summary>
		/// Anchor length of main bars, if MasterComponent is opening
		/// </summary>
		public double AnchorLength { get; set; }

		/// <summary>
		/// type of extension of beginning of bar
		/// </summary>
		public BarEndExtendType ExtendTypeBegin { get; set; }

		/// <summary>
		/// Distance of extension of beginning
		/// </summary>
		public double ExtendDistanceBegin { get; set; }

		/// <summary>
		/// type of extension of end of bar
		/// </summary>
		public BarEndExtendType ExtendTypeEnd { get; set; }

		/// <summary>
		/// Distance of extension of end
		/// </summary>
		public double ExtendDistanceEnd { get; set; }

	}
}
