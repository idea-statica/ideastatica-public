using System.IO;

namespace IdeaStatiCa.BimImporter.Persistence
{
	public interface IFilePersistence : IPersistence
	{
		void Load(TextReader reader);

		void Save(TextWriter writer);
	}
}