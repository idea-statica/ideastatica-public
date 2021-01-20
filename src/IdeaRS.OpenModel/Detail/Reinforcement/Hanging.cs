namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Type of hanging
	/// </summary>
	public enum HangingType
	{
		/// <summary>
		/// Straight anchor with straight branches ||
		/// </summary>
		StraightAnchorStraightBranches,

		/// <summary>
		/// Straight anchor with L-shaped branches _||_
		/// </summary>
		StraightAnchorLShapedBranches,

		/// <summary>
		/// Angle anchor with straight branches /\
		/// </summary>
		AngleAnchorStraightBranches,

		/// <summary>
		/// Angle anchor with L-shaped branches _/\_
		/// </summary>
		AngleAnchorLShapedBranches,

		/// <summary>
		/// Straight single bar |
		/// </summary>
		StraightAnchorSingleBar
	}

	/// <summary>
	/// Edge position type
	/// </summary>
	public enum EdgeOrientationType
	{
		/// <summary>
		/// Distance measured from the beginning of edge
		/// </summary>
		FromBegin,

		/// <summary>
		/// Distance measured from the end of edge
		/// </summary>
		FromEnd
	}

	/// <summary>
	/// Representing hanging in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Reinforcement))]
	public class Hanging : Reinforcement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Hanging()
		{
		}

		/// <summary>
		/// Master component of hanging
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Master edge of hanging
		/// </summary>
		public ReferenceElement MasterEdge { get; set; }

		/// <summary>
		/// Edge orientation type
		/// </summary>
		public EdgeOrientationType EdgeOrientationType { get; set; }

		/// <summary>
		/// Position on edge
		/// </summary>
		public double PositionOnEdge { get; set; }

		/// <summary>
		/// Diameter o reinforcement
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// End type of the reinforcement bar - same for both ends
		/// </summary>
		public LongReinfEndType EndsType { get; set; }

		/// <summary>
		/// Diameter of mandrel - multiplier of bar diameter to obtain inner diameter of bent reinforcement bars
		/// </summary>
		public double DiameterOfMandrel { get; set; }

		/// <summary>
		/// Type of hanging
		/// </summary>
		public HangingType HangingType { get; set; }

		/// <summary>
		/// Length of upper part of hanging
		/// </summary>
		public double UpperLength { get; set; }

		/// <summary>
		/// Length of inside part of anchor
		/// </summary>
		public double BottomLength { get; set; }

		/// <summary>
		/// Length of legs
		/// </summary>
		public double LegsLength { get; set; }

		/// <summary>
		/// Angle between braches
		/// </summary>
		public double Angle { get; set; }

		/// <summary>
		/// Radius of top loop
		/// </summary>
		public double Radius { get; set; }
	}
}
