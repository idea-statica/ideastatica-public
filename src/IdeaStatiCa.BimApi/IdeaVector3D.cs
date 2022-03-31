namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// A immutable class representing a point in a 3D euclidean space.
	/// </summary>
	public class IdeaVector3D
	{
		/// <summary>
		/// X coordinate
		/// </summary>
		public double X { get; }

		/// <summary>
		/// Y coordinate
		/// </summary>
		public double Y { get; }

		/// <summary>
		/// Z coordinate
		/// </summary>
		public double Z { get; }

		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="z">Z coordinate</param>
		public IdeaVector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
}