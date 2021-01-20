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
		[OpenModelPropertyAttribute("DirectionX")]
		[DataMember]
		public double X { get; set; }

		/// <summary>
		/// Gets or sets the Y-dirrection value
		/// </summary>
		[OpenModelPropertyAttribute("DirectionY")]
		[DataMember]
		public double Y { get; set; }

		/// <summary>
		/// Gets or sets the Z-dirrection value
		/// </summary>
		[OpenModelPropertyAttribute("DirectionZ")]
		[DataMember]
		public double Z { get; set; }
	}
}