using IdeaStatiCa.BimImporter;

namespace IdeaRstabPlugin.Geometry
{
	internal class RstabGeometryProvider : IGeometryProvider
	{
		private readonly IGeometry _geometry;

		public RstabGeometryProvider(IGeometry geometry)
		{
			_geometry = geometry;
		}

		public IGeometry GetGeometry()
		{
			return _geometry;
		}
	}
}