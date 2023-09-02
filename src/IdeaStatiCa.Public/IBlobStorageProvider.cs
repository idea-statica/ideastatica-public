namespace IdeaStatiCa.Public
{
	/// <summary>
	/// Is responsible for providing blob storages according to it names
	/// </summary>
	public interface IBlobStorageProvider
	{

		IBlobStorage GetBlobStorage(string blobStorageName);

		void Commit(string blobStorageName);
		void CleanBlobStorages();
	}
}
