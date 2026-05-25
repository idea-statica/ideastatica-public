namespace yjk.FeaApis
{
	public enum TypeOfLoadCase
	{ 
		Dead,
		Live,
		Wind,
		HorizontalSeismic,
		VerticalSeismic,
		CivilDefence,
		Crane,
		Temperature
	}

	public interface IFeaLoadCase
	{
		int Id { get; }
		string Name { get; }
		int LoadGroupId { get; }
		TypeOfLoadCase LoadCaseType { get; }	
	}

	internal class FeaLoadCase : IFeaLoadCase
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int LoadGroupId { get; set; }
		public TypeOfLoadCase LoadCaseType { get; set; }
	}
}
