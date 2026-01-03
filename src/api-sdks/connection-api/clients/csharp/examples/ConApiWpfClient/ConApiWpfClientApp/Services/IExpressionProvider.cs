using ConApiWpfClientApp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	public interface IExpressionProvider
	{
		Task<ExpressionModel?> GetExpressionAsync(Guid projectId, int connectionId, CancellationToken cts);
	}
}
