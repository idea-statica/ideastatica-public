using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimApiLink.Persistence
{
	public class ProjectStorageFactory
	{
		public IProjectStorageReader CreateStorageReader(
			IFilePersistence jsonPersistence,
			string projectPath)
		{
			return new JsonProjectStorageReader(jsonPersistence, projectPath);
		}

		internal IProjectStorage CreateStorage(
			JsonPersistence jsonPersistence,
			string projectPath)
		{
			return new JsonProjectStorage(jsonPersistence, projectPath);
		}
	}
}
