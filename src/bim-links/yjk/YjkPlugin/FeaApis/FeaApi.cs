namespace yjk.FeaApis
{
	public interface IFeaApi
	{
		IFeaGeometryApi Geometry { get; }		

		IFeaLoadsApi Load { get; }

		IFeaResultsApi Result { get; }
		IFeaCrossSectionApi CrossSection { get; }
		IFeaMaterialApi MaterialApi { get; }
	}

	public class FeaApi : IFeaApi
	{
		public IFeaGeometryApi Geometry { get; } = new FeaGeometryApi();		
		public IFeaLoadsApi Load { get;  } = new FeaLoadsApi();
		public IFeaResultsApi Result { get; } = new FeaResultsApi();
		public IFeaCrossSectionApi CrossSection { get; } = new FeaCrossSectionApi();
		public IFeaMaterialApi MaterialApi { get; } = new FeaMaterialApi();
	}
}