namespace IdeaStatiCa.CheckbotPlugin.Models
{
	public class SettingsValue
	{
		public string Name { get; }
		public string Value { get; }

		public SettingsValue(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}