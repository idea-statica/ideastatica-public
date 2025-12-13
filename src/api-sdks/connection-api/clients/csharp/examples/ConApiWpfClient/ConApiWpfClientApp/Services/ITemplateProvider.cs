using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	public interface ITemplateProvider
	{
		Task<string> GetTemplateAsync(Guid projectId, int connectionId, CancellationToken cts);
	}
}
