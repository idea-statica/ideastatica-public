namespace IdeaStatiCa.CheckbotPlugin.PluginList.Storage
{
	internal class AppDataStorage : IStorage
	{
		public Stream? GetReadStream()
		{
			if (!File.Exists(GetPath()))
			{
				return null;
			}

			return File.OpenRead(GetPath());
		}

		public Stream GetWriteStream()
		{
			return File.OpenWrite(GetPath());
		}

		private static string GetPath()
		{
			return Path.Combine(
				 Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				 "IDEA_RS",
				 "CheckbotPlugins.json");
		}
	}
}