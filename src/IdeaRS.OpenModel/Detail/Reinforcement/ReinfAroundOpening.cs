namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representing reinforcement around opening in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Reinforcement))]
	public class ReinfAroundOpening : ReinforcementGroup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReinfAroundOpening()
		{
		}

		/// <summary>
		/// Master component of reinforcement
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Number of layers
		/// </summary>
		public int NumOfLayers { get; set; }

		/// <summary>
		/// Distance between reinforcement layers
		/// </summary>
		public double Distance { get; set; }

		/// <summary>
		/// Anchor length of main bars
		/// </summary>
		public double AnchorLength { get; set; }

		/// <summary>
		/// Add diagonal reinforcement
		/// </summary>
		public bool DiagonalReinforcement{ get; set; }

		/// <summary>
		/// Diameter of diagonal reinforcement
		/// </summary>
		public double DiagonalDiameter { get; set; }

		/// <summary>
		/// Number of diagonal bars in layer
		/// </summary>
		public int DiagonalNumOfBarsInLayer { get; set; }

		/// <summary>
		/// Number of diagonal layers
		/// </summary>
		public int DiagonalNumOfLayers { get; set; }

		/// <summary>
		/// Distance between diagonal reinforcement layers
		/// </summary>
		public double DiagonalDistance { get; set; }

		/// <summary>
		/// length of diagonal bar
		/// </summary>
		public double DiagonalLength { get; set; }

		/// <summary>
		/// End type of diagonal reinforcement bar
		/// </summary>
		public LongReinfEndType DiagonalEndsType { get; set; }
	}
}
