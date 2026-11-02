namespace IdeaStatiCa.CheckbotPlugin.Models
{
	public class PluginInfo
	{
		public string DisplayName { get; }

		public string Description { get; }

		public string Author { get; }

		public string Version { get; }

		public string ApiVersion { get; } = "0.1.0";

		public PluginInfo(string displayName, string description, string author, string version)
		{
			DisplayName = displayName;
			Description = description;
			Author = author;
			Version = version;
		}
	}
}