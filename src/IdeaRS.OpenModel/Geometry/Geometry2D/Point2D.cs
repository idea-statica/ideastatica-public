using System.Runtime.Serialization;
namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Represents an x- and y-coordinate pair in two-dimensional space.
	/// </summary>
	[OpenModelClass("System.Windows.Point, WindowsBase")]
	[DataContract]
	public class Point2D : OpenObject
	{
		/// <summary>
		/// Gets or sets the X-coordinate value
		/// </summary>
		[DataMember]
		public double X { get; set; }

		/// <summary>
		/// Gets or sets the Y-coordinate value
		/// </summary>
		[DataMember]
		public double Y { get; set; }
	}
}