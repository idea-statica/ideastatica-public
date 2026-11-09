namespace CadExampleApi
{
	/// <summary>
	/// The API of the example CAD application. A real link gets this object from the application it runs in.
	/// </summary>
	public interface ICadApi
	{
		/// <summary>
		/// Reads the model open in the application.
		/// </summary>
		ICadGeometryApi Geometry { get; }
	}

	/// <summary>
	/// The example CAD application, with its fixed model.
	/// </summary>
	public class CadApi : ICadApi
	{
		public ICadGeometryApi Geometry { get; } = new CadGeometryApi();
	}
}
