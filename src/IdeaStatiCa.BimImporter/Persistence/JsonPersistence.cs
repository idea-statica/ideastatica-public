using IdeaStatiCa.BimApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace IdeaStatiCa.BimImporter.Persistence
{
	public class JsonPersistence : AbstractPersistence, IFilePersistence
	{
		private static readonly PersistenceTokenConverter _tokenConverter = new PersistenceTokenConverter();

		private class StoredData
		{
			public HashSet<(int, string)> Mappings { get; set; }
			public HashSet<IIdeaPersistenceToken> Tokens { get; set; }
		}

		public void Load(TextReader reader)
		{
			StoredData storedData = JsonConvert.DeserializeObject<StoredData>(reader.ReadToEnd(), _tokenConverter);

			Mappings = storedData.Mappings;
			Tokens = storedData.Tokens;
		}

		public void Save(TextWriter writer)
		{
			StoredData storedData = new StoredData()
			{
				Mappings = Mappings,
				Tokens = Tokens
			};

			writer.Write(JsonConvert.SerializeObject(storedData, _tokenConverter));
		}
	}
}