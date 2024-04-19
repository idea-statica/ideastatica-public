namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// End type of longitudional reinforcement
	/// </summary>
	public enum LongReinfEndType
	{
		/// <summary>
		/// None
		/// </summary>
		None,

		/// <summary>
		/// Bend
		/// </summary>
		Bend,

		/// <summary>
		/// Hook
		/// </summary>
		Hook,

		/// <summary>
		/// Loop
		/// </summary>
		Loop,

		/// <summary>
		/// WeldedTransverseBar
		/// </summary>
		WeldedTransverseBar,

		/// <summary>
		/// Continuous bar - full anchorage
		/// </summary>
		ContinuousBar,

		/// <summary>
		/// Continuous bar outside - no anchorage
		/// </summary>
		ContinuousBarOutside,

		/// <summary>
		/// 90 degree hook for ACI
		/// </summary>
		Hook90DegreeACI,

		/// <summary>
		/// 180 degree hook for ACI
		/// </summary>
		Hook180DegreeACI,

		/// <summary>
		/// Perfect bond
		/// </summary>
		PerfectBond
	}

	/// <summary>
	/// Base class representing group reinforcement in IDEA StatiCa Detail
	/// </summary>
	public abstract class ReinforcementGroup : Reinforcement
	{
		/// <summary>
		/// Diameter o reinforcement
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// Number of bars in layer
		/// </summary>
		public int NumOfBarsInLayer { get; set; }

		/// <summary>
		/// Use cover from DetailModelSetting
		/// </summary>
		public bool CoverFromSetting { get; set; }

		/// <summary>
		/// Cover of reinforcement
		/// </summary>
		public double Cover { get; set; }

		/// <summary>
		/// End type of the beginning of reinforcement bar
		/// </summary>
		public LongReinfEndType EndsTypeBeg { get; set; }

		/// <summary>
		/// End type of the end of reinforcement bar
		/// </summary>
		public LongReinfEndType EndsTypeEnd { get; set; }

		/// <summary>
		/// Diameter of mandrel - multiplier of bar diameter to obtain inner diameter of bent reinforcement bars
		/// </summary>
		public double DiameterOfMandrel { get; set; }
	}
}
