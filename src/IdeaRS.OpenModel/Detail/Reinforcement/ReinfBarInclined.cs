namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Type of inclined bar
	/// </summary>
	public enum InclinedReinfBarType
	{
		/// <summary>
		/// Length of bar is directly in put by user
		/// </summary>
		InputByLength,

		/// <summary>
		/// Length is extended to outer edges of structure
		/// </summary>
		FullLength
	}

	/// <summary>
	/// Representing reinforcement inclined bar in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Reinforcement))]
	public class ReinfBarInclined : ReinforcementGroup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReinfBarInclined()
		{
		}

		/// <summary>
		/// Master component of reinforcement
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Master point of reinforcement
		/// </summary>
		public ReferenceElement MasterPoint { get; set; }

		/// <summary>
		/// Master edge of reinforcement - edge from which is measured angle
		/// </summary>
		public ReferenceElement MasterEdge { get; set; }

		/// <summary>
		/// Type of inclined reinf bar
		/// </summary>
		public InclinedReinfBarType InclinedReinfBarType { get; set; }

		/// <summary>
		/// Angle between bar and master edge
		/// </summary>
		public double Angle { get; set; }

		/// <summary>
		/// Add bar on top - possible only for FullLength
		/// </summary>
		public bool AddBarOnTop { get; set; }

		/// <summary>
		/// Length of additional bar on the top
		/// </summary>
		public double TopLength { get; set; }

		/// <summary>
		///  Add bar on Bottom - possible only for FullLength
		/// </summary>
		public bool AddBarOnBottom { get; set; }

		/// <summary>
		/// Length of additional bar on the bottom
		/// </summary>
		public double BottomLength { get; set; }
	}
}
