using System;
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi.Api
{
	/// <summary>
	/// Represents an extension of the asynchronous connection library API.
	/// </summary>
	/// <remarks>This interface serves as a marker for additional functionality that extends the capabilities of the
	/// <see cref="IConnectionLibraryApiAsync"/> interface. Implementers can provide further methods or properties to
	/// enhance connection management features.</remarks>
	public interface IConnectionLibraryApiExt : IConnectionLibraryApiAsync
	{
		/// <summary>
		/// Asynchronously retrieves the picture data (PNG) for a specified design item.
		/// </summary>
		/// <param name="designSetId">The unique identifier of the design set. (optional)</param>
		/// <param name="designItemId">The unique identifier of the design item for which the template is requested. (optional)</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see
		/// cref="System.Threading.CancellationToken.None"/>.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a byte array of the picture data.</returns>
		Task<byte[]> GetDesignItemPictureDataAsync(Guid designSetId, Guid designItemId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

		/// <summary>
		/// Asynchronously downloads the picture (PNG) for a specified design item and saves it to a file.
		/// </summary>
		/// <param name="designSetId">The unique identifier of the design set.</param>
		/// <param name="designItemId">The unique identifier of the design item for which the picture is requested.</param>
		/// <param name="filePath">The full path to the PNG file which will be created (or overwritten).</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see
		/// cref="System.Threading.CancellationToken.None"/>.</param>
		/// <returns>A task that represents the asynchronous file write operation.</returns>
		Task SaveDesignItemPictureAsync(Guid designSetId, Guid designItemId, string filePath, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
	}

	/// <summary>
	/// Provides an extended implementation of the <see cref="ConnectionLibraryApi"/> class,  offering additional
	/// functionality for managing connections.
	/// </summary>
	/// <remarks>This class is intended for use in scenarios where the default connection management  provided by
	/// <see cref="ConnectionLibraryApi"/> needs to be extended or customized.  It implements the <see
	/// cref="IConnectionLibraryApiExt"/> interface to ensure compatibility  with extended connection operations.</remarks>
	public class ConnectionLibraryApiExt : ConnectionLibraryApi, IConnectionLibraryApiExt
	{
		internal ConnectionLibraryApiExt(Client.ISynchronousClient client, Client.IAsynchronousClient asyncClient, Client.IReadableConfiguration configuration) : base(client, asyncClient, configuration)
		{
		}

		/// <inheritdoc cref="IConnectionLibraryApiExt.GetDesignItemPictureDataAsync(Guid, Guid, System.Threading.CancellationToken)"/>
		public async Task<byte[]> GetDesignItemPictureDataAsync(Guid designSetId, Guid designItemId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
		{
			var response = await base.GetDesignItemPictureWithHttpInfoAsync(designSetId, designItemId, "image/png", 0, cancellationToken);
			byte[] buffer = (byte[])response.Data;
			return buffer;
		}

		/// <inheritdoc cref="IConnectionLibraryApiExt.SaveDesignItemPictureAsync(Guid, Guid, string, System.Threading.CancellationToken)"/>
		public async Task SaveDesignItemPictureAsync(Guid designSetId, Guid designItemId, string filePath, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
		{
			byte[] buffer = await GetDesignItemPictureDataAsync(designSetId, designItemId, cancellationToken);

			string directory = Path.GetDirectoryName(filePath);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

#if NETSTANDARD2_1_OR_GREATER
			await File.WriteAllBytesAsync(filePath, buffer, cancellationToken);
#else
			File.WriteAllBytes(filePath, buffer);
#endif
		}
	}
}
