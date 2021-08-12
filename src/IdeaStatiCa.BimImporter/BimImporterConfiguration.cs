namespace IdeaStatiCa.BimImporter
{
	public class BimImporterConfiguration
	{
		public double GeometryPrecision { get; set; }
		public double ResultSectionPositionPrecision { get; set; }
		public double LCSPrecision { get; set; }

		public BimImporterConfiguration()
		{
			GeometryPrecision = 1e-6;
			ResultSectionPositionPrecision = 1e-6;
			LCSPrecision = 1e-6;
		}
	}
}