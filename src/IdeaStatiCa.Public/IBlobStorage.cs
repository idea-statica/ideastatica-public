using System.Collections.Generic;
using System.IO;

namespace IdeaStatiCa.Public
{
	/// <summary>
	/// Represents a storage of streams for the project file. Each component of the project is represented by a content with a specific id,
	/// that can be stored in a separate file or other type of storage depending of the implementation.
	/// </summary>
	public interface IBlobStorage
	{
		/// <summary>
		/// Initialize blob storage.
		/// </summary>
		/// <param name="basePath">Base path to storage.</param>
		void Init(string basePath);

		/// <summary>
		/// Save <paramref name="content"/> blob with <paramref name="contentId"/> identificator to the storage.
		/// </summary>
		/// <param name="content">Blob to be saved</param>
		/// <param name="contentId">Uniqe blob identificator</param>
		void Write(Stream content, string contentId);

		/// <summary>
		/// Read blob with <paramref name="contentId"/> identificator from the storage into the stream. Client have to DISPOSE stream when it is no longer needed.
		/// </summary>
		/// <param name="contentId">Uniqe blob identificator</param>
		/// <returns>Readed blob as stream. Client have to dispose stream.</returns>
		Stream Read(string contentId);

		/// <summary>
		/// Tells whether blob with <paramref name="contentId"/> identificator exists or not.
		/// </summary>
		/// <param name="contentId">Uniqe blob identificator</param>
		/// <returns>Whether the blob with contentId identificator exists or not</returns>
		bool Exist(string contentId);

		/// <summary>
		/// Deletes blob with <paramref name="contentId"/> identificator.
		/// </summary>
		/// <param name="contentId">Uniqe blob identificator</param>
		void Delete(string contentId);

		/// <summary>
		/// The collection of relative paths of entries that are currently in the storage with excluding the .lock files
		/// </summary>
		/// <returns>The collection of relative paths of entries that are currently in the storage, while the .lock files are excluded</returns>
		IReadOnlyCollection<string> GetEntries();
	}
}
