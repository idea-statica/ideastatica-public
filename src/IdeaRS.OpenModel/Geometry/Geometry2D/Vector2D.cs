namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Represents a vector in two-dimensional space.
	/// </summary>
	[OpenModelClass("System.Windows.Vector, WindowsBase")]
	public class Vector2D : OpenObject
	{
		/// <summary>
		/// Gets or sets the X-dirrection value
		/// </summary>
		public double X { get; set; }

		/// <summary>
		/// Gets or sets the Y-dirrection value
		/// </summary>
		public double Y { get; set; }
	}
}