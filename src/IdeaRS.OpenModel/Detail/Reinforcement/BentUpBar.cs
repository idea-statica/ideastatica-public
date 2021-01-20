namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representing bent-Up bar in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Reinforcement))]
	public class BentUpBar : ReinforcementGroup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public BentUpBar()
		{
		}

		/// <summary>
		/// Master component of reinforcement
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Bar is on left or right side of member1D
		/// </summary>
		public bool IsOnLeft { get; set; }

		/// <summary>
		/// Offset of bent-Up bar from left/right edge
		/// </summary>
		public double Offset { get; set; }

		/// <summary>
		/// Angle between bar and master edge
		/// </summary>
		public double Angle { get; set; }

		/// <summary>
		/// Length of additional bar on the top
		/// </summary>
		public double TopLength { get; set; }

		/// <summary>
		/// Length of additional bar on the bottom
		/// </summary>
		public double BottomLength { get; set; }

		/// <summary>
		/// Add anchor to bent up bar
		/// </summary>
		public bool AddAnchor { get; set; }

		/// <summary>
		/// Length of anchor for bent up bar
		/// </summary>
		public double AnchorLength { get; set; }
	}
}
