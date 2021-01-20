using IdeaRS.OpenModel.Geometry3D;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representing reinforcement bar input by two points in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Reinforcement))]
	public class ReinfBarByTwoPoints : ReinforcementGroup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReinfBarByTwoPoints()
		{
		}

		/// <summary>
		/// First master component of reinforcement
		/// </summary>
		public ReferenceElement MasterComponent1 { get; set; }

		/// <summary>
		/// Second master component of reinforcement
		/// </summary>
		public ReferenceElement MasterComponent2 { get; set; }

		/// <summary>
		/// First master point of reinforcement
		/// </summary>
		public int MasterPoint1 { get; set; }

		/// <summary>
		/// Second master point of reinforcement
		/// </summary>
		public int MasterPoint2 { get; set; }

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
		/// Offset between first master point and first point of bar
		/// </summary>
		public Vector3D Position1 { get; set; }

		/// <summary>
		/// Offset between second master point and second point of bar
		/// </summary>
		public Vector3D Position2 { get; set; }
	}
}
