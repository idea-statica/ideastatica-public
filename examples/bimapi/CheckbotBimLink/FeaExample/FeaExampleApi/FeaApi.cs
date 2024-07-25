namespace BimApiLinkFeaExample.FeaExampleApi
{
	public interface IFeaApi
	{
		IFeaGeometryApi Geometry { get; }		

		IFeaLoadsApi Loads { get; }

		IFeaResultsApi Results { get; }
	}

	public class FeaApi : IFeaApi
	{
		public IFeaGeometryApi Geometry { get; } = new FeaGeometryApi();		
		public IFeaLoadsApi Loads { get;  } = new FeaLoadsApi();
		public IFeaResultsApi Results { get; } = new FeaResultsApi();
	}
}