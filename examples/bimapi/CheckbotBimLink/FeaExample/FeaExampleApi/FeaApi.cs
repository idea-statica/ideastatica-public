namespace FeaExampleApi
{
	/// <summary>
	/// The API of the example FEA application. A real link gets this object from the application it runs in.
	/// </summary>
	public interface IFeaApi
	{
		/// <summary>
		/// Reads the structure of the model open in the application.
		/// </summary>
		IFeaGeometryApi Geometry { get; }

		/// <summary>
		/// Reads the load groups, load cases and combinations of the model.
		/// </summary>
		IFeaLoadsApi Loads { get; }

		/// <summary>
		/// Reads the internal forces the application has calculated.
		/// </summary>
		IFeaResultsApi Results { get; }
	}

	/// <summary>
	/// The example FEA application, with its fixed model and results.
	/// </summary>
	public class FeaApi : IFeaApi
	{
		public IFeaGeometryApi Geometry { get; } = new FeaGeometryApi();

		public IFeaLoadsApi Loads { get; } = new FeaLoadsApi();

		public IFeaResultsApi Results { get; } = new FeaResultsApi();
	}
}
