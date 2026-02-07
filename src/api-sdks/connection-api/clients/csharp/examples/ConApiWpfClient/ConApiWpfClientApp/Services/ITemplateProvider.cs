using ConApiWpfClientApp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	/// <summary>
	/// Provides a connection template for applying to a connection.
	/// Implementations may load templates from files or from the connection library.
	/// </summary>
	public interface ITemplateProvider
	{
		/// <summary>
		/// Gets a connection template for the specified connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="cts">A cancellation token to cancel the operation.</param>
		/// <returns>A <see cref="ConnectionLibraryModel"/> containing the template XML, or <see langword="null"/> if cancelled.</returns>
		Task<ConnectionLibraryModel?> GetTemplateAsync(Guid projectId, int connectionId, CancellationToken cts);
	}
}
