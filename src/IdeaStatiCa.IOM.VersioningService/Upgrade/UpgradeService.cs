using IdeaStatiCa.IOM.VersioningService.Configuration;
using IdeaStatiCa.IOM.VersioningService.Exceptions;
using IdeaStatiCa.Plugin;
using System;
using System.Diagnostics;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.Upgrade
{
	public class UpgradeService : VersioningServiceBase, IUpgradeService
	{
		public UpgradeService(IPluginLogger logger, IConfigurationStepService configurationStepService) : base(logger, configurationStepService)
		{
		}

		public void Upgrade()
		{
			if (IsModelEmpty)
			{
				_logger.LogDebug("Upgrade: model is empty");
			}

			var versionBeforeUpgrade = this.GetCurrentVersion();
			_logger.LogInformation($"Upgrade from version {versionBeforeUpgrade}");
			foreach (var step in ConfigurationStepService.UpgradeSteps())
			{
				//skip older versions
				if (step.GetVersion() > versionBeforeUpgrade)
				{
					try
					{
						//upgrade version number
						step.UpgradeVersion(_model);

						//upgrade model
						step.DoUpStep(_model);
					}
					catch (Exception ex)
					{
						_logger.LogError($"Upgrade step {step.GetVersion()} fail", ex);
						throw new UpgradeStepException(step.GetVersion(), ex);
					}
				}
				else
				{
					_logger.LogDebug($"Upgrade step {step.GetVersion()} was skipped.");
				}
			}
		}
	}
}
