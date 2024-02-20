namespace IdeaStatiCa.BimImporter
{
	public class BimImporterConfiguration
	{
		public double GeometryPrecision { get; set; }
		public double ResultSectionPositionPrecision { get; set; }
		public double LCSPrecision { get; set; }
		public double LCSPrecisionForNormalization { get; set; }
		public bool ThrowOnResultsDuplicate { get; set; }
		public bool IgnoreOutOfBoundsResultSections { get; set; }

		public BimImporterConfiguration()
		{
			GeometryPrecision = 1e-6;
			ResultSectionPositionPrecision = 1e-6;
			LCSPrecision = 1e-6;
			LCSPrecisionForNormalization = 1e-6;
			ThrowOnResultsDuplicate = false;
			IgnoreOutOfBoundsResultSections = true;
		}
	}
}