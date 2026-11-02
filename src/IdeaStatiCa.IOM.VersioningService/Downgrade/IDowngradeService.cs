using System;
using System.Collections.Generic;

namespace IdeaStatiCa.IOM.VersioningService.Downgrade
{
	public interface IDowngradeService : IVersioningService
	{

		/// <summary>
		/// Downgrade to specific version
		/// </summary>
		void Downgrade(Version version);

		void Downgrade(string version);

		IEnumerable<Version> GetVersionsToDowngrade();
	}
}
