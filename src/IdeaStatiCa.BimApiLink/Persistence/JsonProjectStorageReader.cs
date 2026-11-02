using IdeaStatiCa.BimImporter.Persistence;
using System.IO;

namespace IdeaStatiCa.BimApiLink.Persistence
{
	internal class JsonProjectStorageReader : ProjectStorageBase, IProjectStorageReader
	{
		internal JsonProjectStorageReader(
			IFilePersistence filePersistence, 
			string workingDirectory) : 
			base(filePersistence, workingDirectory)
		{
		}

		public void Load()
		{
			if (File.Exists(_path))
			{
				using (FileStream fs = File.OpenRead(_path))
				using (StreamReader streamReader = new StreamReader(fs))
				{
					_filePersistence.Load(streamReader);
				}
			}
		}
	}
}
