namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	public class ConMissingWeld
	{
		public string EdgePlate { get; set; }
		public string SurfacePlate { get; set; }
		public int EdgeIndex { get; set; }
		public int SurfaceIndex { get; set; }
		public double WeldSize { get; set; } = 0;
		public string WeldTypeCode { get; set; }
		public string WeldMaterialName { get; set; } = null;
		public bool IsHollow_OrUncoiledSection { get; set; }
	}
}
