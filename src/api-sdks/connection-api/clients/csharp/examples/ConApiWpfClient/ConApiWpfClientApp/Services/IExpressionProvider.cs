using ConApiWpfClientApp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	/// <summary>
	/// Provides a parametric expression for evaluation against a connection.
	/// Implementations may generate a default expression and allow the user to modify it.
	/// </summary>
	public interface IExpressionProvider
	{
		/// <summary>
		/// Gets an expression from the user for evaluation in the context of a connection.
		/// </summary>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection providing the evaluation context.</param>
		/// <param name="cts">A cancellation token to cancel the operation.</param>
		/// <returns>An <see cref="ExpressionModel"/> containing the expression, or <see langword="null"/> if cancelled.</returns>
		Task<ExpressionModel?> GetExpressionAsync(Guid projectId, int connectionId, CancellationToken cts);
	}
}
