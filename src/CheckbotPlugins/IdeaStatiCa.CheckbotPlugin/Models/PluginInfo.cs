namespace IdeaStatiCa.CheckbotPlugin.Models
{
	public class PluginInfo
	{
		public string Name { get; }

		public string Description { get; }

		public string Author { get; }

		public string Version { get; }

		public string ApiVersion { get; } = "0.1.0";

		public PluginInfo(string name, string description, string author, string version)
		{
			Name = name;
			Description = description;
			Author = author;
			Version = version;
		}
	}
}