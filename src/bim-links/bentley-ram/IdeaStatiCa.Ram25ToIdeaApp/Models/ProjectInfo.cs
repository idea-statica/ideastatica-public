namespace IdeaStatiCa.RamToIdeaApp.Models
{
	public interface IProjectInfo
	{
		string RamDbFileName { get; set; }
		string ProjectWorkingDir { get; set; }
	}

	public class ProjectInfo : IProjectInfo
	{
		public string RamDbFileName { get; set; }

		public string ProjectWorkingDir { get; set; }
	}
}
