using IdeaStatiCa.IOM.VersioningService.VersionSteps;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.IOM.VersioningService.Configuration
{
	/// <summary>
	/// service responsible for registration of versioning steps
	/// base on registration provide upgrade steps / downgrade steps
	/// </summary>
	public interface IConfigurationStepService
	{
		/// <summary>
		/// Get sequence of downgrade steps
		/// </summary>
		/// <returns></returns>
		IEnumerable<IDowngradeStep> DowngradeSteps();

		/// <summary>
		/// Get sequence of upgrade steps
		/// </summary>
		/// <returns></returns>
		IEnumerable<IUpgradeStep> UpgradeSteps();


		/// <summary>
		/// Get version of latest step
		/// </summary>
		/// <returns></returns>
		Version GetLatestStepVersion();

	}
}
