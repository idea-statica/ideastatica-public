namespace BimApiLinkFeaExample.FeaExampleApi
{
	public enum TypeOfLoadCase
	{ 
		Selfweight,
		DeadLoad,
		Snow,
		Wind
	}

	public enum ActionType
	{ 
		Permanent,
		Variable
	}

	public interface IFeaLoadCase
	{
		int Id { get; }
		string Name { get; }
		int LoadGroupId { get; }
		TypeOfLoadCase LoadCaseType { get; }		
		ActionType ActionType { get; }		
	}

	internal class FeaLoadCase : IFeaLoadCase
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int LoadGroupId { get; set; }
		public TypeOfLoadCase LoadCaseType { get; set; }
		public ActionType ActionType { get; set; }
	}
}
