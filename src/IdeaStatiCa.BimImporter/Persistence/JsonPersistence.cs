using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace IdeaStatiCa.BimImporter.Persistence
{
	public class JsonPersistence : IFilePersistence
	{
		private class StoredData
		{
			public List<(int, string)> Mappings { get; set; }
			public List<IIdeaPersistenceToken> Tokens { get; set; }
		}

		private List<(int, string)> _mappings = new List<(int, string)>();
		private List<IIdeaPersistenceToken> _tokens = new List<IIdeaPersistenceToken>();

		public IEnumerable<(int, string)> GetMappings()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IIdeaPersistenceToken> GetTokens()
		{
			throw new NotImplementedException();
		}

		public void StoreMapping(int iomId, string bimApiId)
		{
			throw new NotImplementedException();
		}

		public void StoreToken(IIdeaPersistenceToken serializable)
		{
			throw new NotImplementedException();
		}

		public void Load(TextReader reader)
		{
			//JsonSerializer.Serialize
		}

		public void Save(TextWriter writer)
		{
			/*StoredData storedData = new StoredData()
			{
				Mappings = _mappings,
				Tokens = _tokens
			};

			writer.Write(JsonConvert.SerializeObject(storedData));*/
		}
	}
}