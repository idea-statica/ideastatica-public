using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest
{
	public interface IConnectionApiController : IDisposable
	{
		/// <summary>
		/// Open idea project in the service
		/// </summary>
		/// <param name="ideaConProject">Idea Connection project.</param>
		Task<ConProject> OpenProjectAsync(string ideaConProject, CancellationToken token = default);
	}
}
