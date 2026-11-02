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
			//File.OpenWrite cant be used because not rewrite all data only stream part and longer old one keep there => create invalid file 
			return File.Create(GetPath());
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