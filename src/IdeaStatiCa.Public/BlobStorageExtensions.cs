using System.IO;

namespace IdeaStatiCa.Public
{
	/// <summary>
	/// Implementation of helpers end extensions related to <see cref="IBlobStorage"/>
	/// </summary>
	public static class BlobStorageExtensions
	{
		/// <summary>
		/// Extension method which copies the content of <paramref name="sourceStorage"/> to this storage (<paramref name="destination"/>)
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="sourceStorage"></param>
		public static void CopyFrom(this IBlobStorage destination, IBlobStorage sourceStorage)
		{
			var sourceEntries = sourceStorage.GetEntries();
			foreach (var contentId in sourceEntries)
			{
				using (var sourceStream = sourceStorage.Read(contentId))
				{
					destination.Write(sourceStream, contentId);
				}
			}
		}

		/// <summary>
		/// Read Get all text from blob <paramref name="contentId"/>
		/// </summary>
		/// <param name="blobStorage">The storage</param>
		/// <param name="contentId">The id of the blob</param>
		/// <returns></returns>
		public static string ReadText(this IBlobStorage blobStorage, string contentId)
		{
			using (var stream = blobStorage.Read(contentId))
			{
				using (var reader = new StreamReader(stream))
				{
					var text = reader.ReadToEnd();
					return text;
				}
			}
		}
	}
}
