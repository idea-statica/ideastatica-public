namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// Provides <see cref="IGeometry"/> instance.
	/// </summary>
	public interface IGeometryProvider
	{
		/// <summary>
		/// Returns instance of <see cref="IGeometry"/> with current geometry.
		/// </summary>
		/// <returns><see cref="IGeometry"/> instance.</returns>
		IGeometry GetGeometry();
	}
}