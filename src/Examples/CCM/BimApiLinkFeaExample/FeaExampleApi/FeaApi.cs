namespace BimApiLinkFeaExample.FeaExampleApi
{
	internal interface IFeaApi
	{
		IFeaGeometryApi Geometry { get; }
	}

	internal class FeaApi : IFeaApi
	{
		public IFeaGeometryApi Geometry { get; } = new FeaGeometryApi();
	}
}