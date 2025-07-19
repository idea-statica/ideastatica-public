using IdeaStatiCa.BimImporter.Persistence;
using System.IO;

namespace IdeaStatiCa.BimApiLink.Persistence
{
	internal class JsonProjectStorage : JsonProjectStorageReader, IProjectStorage
	{
		internal JsonProjectStorage(
			IFilePersistence filePersistence, 
			string workingDirectory) 
			: base(filePersistence, workingDirectory)
		{
		}

		public void Save()
		{
			using (FileStream fs = File.Create(_path))
			using (StreamWriter streamWriter = new StreamWriter(fs))
			{
				_filePersistence.Save(streamWriter);
			}
		}

		public bool IsValid()
		{
			return File.Exists(_path);
		}
	}
}