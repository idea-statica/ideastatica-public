namespace BimApiLinkCadExample.CadExampleApi
{
	public interface ICadApi
	{
		ICadGeometryApi Geometry { get; }
	}

	public class CadApi : ICadApi
	{
		public ICadGeometryApi Geometry { get; } = new CadGeometryApi();
	}
}