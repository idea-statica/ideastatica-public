using IdeaStatiCa.IOM.VersioningService.Configuration;
using IdeaStatiCa.IOM.VersioningService.Exceptions;
using IdeaStatiCa.IOM.VersioningService.Tools;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.Downgrade
{
	public class DowngradeService : VersioningServiceBase, IDowngradeService
	{
		public DowngradeService(IPluginLogger logger, IConfigurationStepService configurationStepService) : base(logger, configurationStepService)
		{
		}

		public void Downgrade(string version)
		{
			var versionToDowngrade = VersionTool.GetVersion(version);
			_logger.LogInformation($"Downgrade from version {versionToDowngrade}");
			this.Downgrade(versionToDowngrade);
		}

		public void Downgrade(Version version)
		{
			if (IsModelEmpty)
			{
				_logger.LogInformation("Downgrade: model is empty");
				return;
			}

			var versionBeforeDowngrade = this.GetCurrentVersion();
			_logger.LogInformation($"Downgrade from version {versionBeforeDowngrade} to version {version}");
			foreach (var step in ConfigurationStepService.DowngradeSteps())
			{
				//skip newer versions and also skip downgrade steps lower as required version
				if (step.GetVersion() <= versionBeforeDowngrade && step.GetVersion() >= version)
				{
					try
					{
						_logger.LogDebug("downgrade version information");
						//downgrade version information
						step.DowngradeVersion(_model);

						//if its not last step for fixing version number do down step
						if (step.GetVersion() > version)
						{
							_logger.LogDebug("downgrade model by step");
							step.DoDownStep(_model);
						}
					}
					catch (Exception ex)
					{
						_logger.LogError($"Downgrade step {step.GetVersion()} fail", ex);
						throw new DowngradeStepException(step.GetVersion(), ex);
					}
				}
				else
				{
					_logger.LogDebug($"Downgrade step {step.GetVersion()} was skipped. Is step older than required {step.GetVersion() <= versionBeforeDowngrade} is step newer than actual {step.GetVersion() >= version}");
				}
			}
		}

		public IEnumerable<Version> GetVersionsToDowngrade()
		{
			_logger.LogDebug("GetVersionsToDowngrade");

			if (IsModelEmpty)
			{
				_logger.LogDebug("GetVersionsToDowngrade model is empty");
				return Enumerable.Empty<Version>();
			}

			var currentVersion = this.GetCurrentVersion();
			List<Version> availableVersionsForDowngrade = new List<Version>();
			foreach (var step in ConfigurationStepService.DowngradeSteps())
			{
				//skip newer versions and also skip downgrade steps lower as required version
				if (step.GetVersion() < currentVersion)
				{
					availableVersionsForDowngrade.Add(step.GetVersion());
				}
			}

			return availableVersionsForDowngrade;
		}
	}
}
