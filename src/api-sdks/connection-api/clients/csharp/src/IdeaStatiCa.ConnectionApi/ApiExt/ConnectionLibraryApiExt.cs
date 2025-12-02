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

	}
}
