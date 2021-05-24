using System.IO;

namespace IdeaStatiCa.BimImporter.Persistence
{
	/// <summary>
	/// Id mapping and tokens persistence using file system as a backend.
	/// </summary>
	public interface IFilePersistence : IPersistence
	{
		/// <summary>
		/// Load previously stored data.
		/// </summary>
		/// <param name="reader">TextReader instance.</param>
		void Load(TextReader reader);

		/// <summary>
		/// Save data.
		/// </summary>
		/// <param name="writer">TextWritter instance.</param>
		void Save(TextWriter writer);
	}
}