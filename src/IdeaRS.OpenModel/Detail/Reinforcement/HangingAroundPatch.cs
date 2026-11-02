namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representing of hanging hanging around patch load/support in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Reinforcement))]
	public class HangingAroundPatch : Reinforcement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public HangingAroundPatch()
		{
		}

		/// <summary>
		/// Master component of hanging
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Number of bars in layer
		/// </summary>
		public int NumOfBarsInLayer { get; set; }

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
		/// Distance from patch support/load
		/// </summary>
		public double DistanceFromPatch { get; set; }

		/// <summary>
		/// Length of bottom part
		/// </summary>
		public double BottomPartLength { get; set; }

		/// <summary>
		/// Length of inclined parts
		/// </summary>
		public double InclinedPartsLength { get; set; }

		/// <summary>
		/// Length of legs
		/// </summary>
		public double LegsLength { get; set; }

		/// <summary>
		/// Inclination of inclined parts
		/// </summary>
		public double InclinedPartsAngle { get; set; }

		/// <summary>
		/// Angle of reinforcement rotation around patch point
		/// </summary>
		public double Rotation { get; set; }

		/// <summary>
		/// Add same reinforcement on oposide side (rotated 180°)
		/// </summary>
		public bool IsMirrored { get; set; }
	}
}
