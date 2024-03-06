using System;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps
{
	public interface IStep
	{
		/// <summary>
		/// Get version of step
		/// </summary>
		/// <returns></returns>
		Version GetVersion();
	}
}
