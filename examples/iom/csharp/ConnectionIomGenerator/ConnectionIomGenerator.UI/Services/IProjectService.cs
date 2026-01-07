using System.Threading.Tasks;
using ConnectionIomGenerator.UI.Models;

namespace ConnectionIomGenerator.UI.Services
{
	public interface IProjectService
	{
		public Task<ProjectInfo> CreateProjectAsync();
	}
}
