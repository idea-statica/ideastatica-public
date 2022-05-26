using IdeaRS.OpenModel;

namespace IdeaStatiCa.CheckbotPlugin
{
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