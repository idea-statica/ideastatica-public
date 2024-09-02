namespace IdeaStatiCa.CheckbotPlugin.Models
{
	public enum CountryCode
	{
		ECEN = 0,
		American = 1,
		Canada = 2,
		Australia = 3,
		RUS = 4,
		CHN = 5,
		India = 6,
		HKG = 7
	}

	public class ProjectInfo
	{
		public string Name { get; }

		public CountryCode CountryCode { get; }

		/// <summary>
		/// Source Application specify data source
		/// </summary>
		public string SourceApplication { get; }

		/// <summary>
		/// Source Application Type [CAD|FEA|None] specify data source
		/// </summary>
		public string SourceApplicationType { get; }

		public ProjectInfo(string name, CountryCode countryCode, string sourceApplication = "", string sourceApplicationType = "")
		{
			Name = name;
			CountryCode = countryCode;
			SourceApplication = sourceApplication;
			SourceApplicationType = sourceApplicationType;
		}
	}
}