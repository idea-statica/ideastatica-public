using IdeaStatiCa.BimImporter.Persistence;
using System.IO;

namespace IdeaStatiCa.BimApiLink.Persistence
{
	internal abstract class ProjectStorageBase
	{
		private const string PERSISTANCY_STORAGE = "bimapi-data.json";

		protected readonly string _path;
		protected readonly IFilePersistence _filePersistence;

		protected ProjectStorageBase(IFilePersistence filePersistence, string workingDirectory)
		{
			_filePersistence = filePersistence;
			_path = Path.Combine(workingDirectory, PERSISTANCY_STORAGE);
		}
	}
}
