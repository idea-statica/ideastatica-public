using System.Threading.Tasks;
using ConnectionIomGenerator.UI.Models;

namespace ConnectionIomGenerator.UI.Services
{
	public class ProjectService : IProjectService
	{
		public async Task<ProjectInfo> CreateProjectAsync()
		{
			var projInfo = new ProjectInfo() { Id = "1" };
			return await Task.FromResult(projInfo);
		}
	}
}
