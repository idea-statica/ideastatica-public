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

		public ProjectInfo(string name, CountryCode countryCode)
		{
			Name = name;
			CountryCode = countryCode;
		}
	}
}