using IdeaStatiCa.CheckbotPlugin.Common;
using System;
using System.IO;

namespace IdeaStatiCa.PluginSystem.PluginList.Storage
{
	internal class AppDataStorage : IStorage
	{
		public Maybe<Stream> GetReadStream()
		{
			if (!File.Exists(GetPath()))
			{
				return new Maybe<Stream>();
			}

			return new Maybe<Stream>(File.OpenRead(GetPath()));
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