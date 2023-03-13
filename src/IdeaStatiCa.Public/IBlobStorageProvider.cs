namespace IdeaStatiCa.Public
{
	public interface IBlobStorageProvider
	{
		IBlobStorage GetBlobStorage(string blobStorageName);

		void Commit(string blobStorageName);
	}
}
