namespace IdeaStatiCa.Public
{
	/// <summary>
	/// It is responsible for providing blob storages according to its name
	/// </summary>
	public interface IBlobStorageProvider
	{
		/// <summary>
		/// Provides required blob storage which is defined by its name <paramref name="blobStorageName"/>
		/// </summary>
		/// <param name="blobStorageName">The name of the required blob storage</param>
		/// <returns>The instance on the required blob storage</returns>
		IBlobStorage GetBlobStorage(string blobStorageName);

		/// <summary>
		/// Refresh blob storage in case it contains some old cached data
		/// </summary>
		/// <param name="blobStorageName"></param>
		void RefreshBlobStorage(string blobStorageName);

		/// <summary>
		/// Commits all changes in the blobstorage
		/// </summary>
		/// <param name="blobStorageName">The name of blob storage to commit</param>
		void Commit(string blobStorageName);

		/// <summary>
		/// Invalidates changes in all blob storages
		/// </summary>
		void CleanBlobStorages();

		/// <summary>
		/// Delete blob storage
		/// </summary>
		/// <param name="blobStorageName">The name of the blob storage to delete</param>
		void DeleteBlobStorage(string blobStorageName);
	}
}
