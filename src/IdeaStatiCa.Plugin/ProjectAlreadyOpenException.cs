using System;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Raised when a Checkbot project cannot be opened because another Checkbot instance already has it open
	/// (see <see cref="ProjectInstanceLockFile"/>). Thrown by the Checkbot open/create funnel and by the BIM plugin
	/// launcher when it refuses to spawn a second Checkbot for an already-open project.
	/// </summary>
	public class ProjectAlreadyOpenException : Exception
	{
		public string ProjectDir { get; }

		public ProjectAlreadyOpenException(string projectDir)
			: base("The project '" + projectDir + "' is already open in another Checkbot instance.")
		{
			ProjectDir = projectDir;
		}
	}
}
