using IdeaRS.OpenModel.Geometry3D;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representing reinforcement bar input by polyline in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Reinforcement))]
	public class ReinfBarByPolyline : ReinforcementGroup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReinfBarByPolyline()
		{
		}

		/// <summary>
		/// Master component of reinforcement
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Master point of reinforcement
		/// </summary>
		public int MasterPoint { get; set; }

		/// <summary>
		/// Polyline representing shape of bar
		/// </summary>
		public ReferenceElement BarShape { get; set; }

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
		/// Offset between master point and first point of BarShape
		/// </summary>
		public Vector3D Position { get; set; }
	}
}
