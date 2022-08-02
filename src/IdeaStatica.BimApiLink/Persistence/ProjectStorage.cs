using IdeaStatiCa.BimImporter.Persistence;

namespace IdeaStatica.BimApiLink.Persistence
{
	internal class JsonProjectStorage : IProjectStorage
	{
		private const string PersistencyStorage = "bimapi-data.json";

		private readonly string _path;
		private readonly IFilePersistence _filePersistence;

		public JsonProjectStorage(IFilePersistence filePersistence, string workingDirectory)
		{
			_filePersistence = filePersistence;
			_path = Path.Combine(workingDirectory, PersistencyStorage);
		}

		public void Save()
		{
			using FileStream fs = File.OpenWrite(_path);
			using StreamWriter streamWriter = new(fs);

			_filePersistence.Save(streamWriter);
		}

		public void Load()
		{
			if (File.Exists(_path))
			{
				using FileStream fs = File.OpenRead(_path);
				using StreamReader streamReader = new(fs);

				_filePersistence.Load(streamReader);
			}
		}
	}
}