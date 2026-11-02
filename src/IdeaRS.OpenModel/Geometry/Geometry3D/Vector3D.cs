using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Represents a vector in three-dimensional space.
	/// </summary>
	[OpenModelClass("CI.Geometry3D.Vector3D,CI.BasicTypes")]
	[DataContract]
	public class Vector3D : OpenObject
	{
		/// <summary>
		/// Gets or sets the X-dirrection value
		/// </summary>
		[OpenModelProperty("DirectionX")]
		[DataMember]
		public double X { get; set; }

		/// <summary>
		/// Gets or sets the Y-dirrection value
		/// </summary>
		[OpenModelProperty("DirectionY")]
		[DataMember]
		public double Y { get; set; }

		/// <summary>
		/// Gets or sets the Z-dirrection value
		/// </summary>
		[OpenModelProperty("DirectionZ")]
		[DataMember]
		public double Z { get; set; }

		public Vector3D()
		{

		}

		public Vector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
}