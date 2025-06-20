using IdeaStatiCa.IOM.VersioningService.VersionSteps;
using IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.Configuration
{
	public class ConfigurationStepService : IConfigurationStepService
	{
		private SortedDictionary<Version, BaseStep> _steps = new SortedDictionary<Version, BaseStep>();
		private IPluginLogger _logger;

		public ConfigurationStepService(IPluginLogger logger)
		{
			this._logger = logger;
			RegisterSteps();
		}

		private void RegisterSteps()
		{
			RegisterStep(new Step200(_logger));
			RegisterStep(new Step201(_logger));
			RegisterStep(new Step205(_logger));
			RegisterStep(new Step206(_logger));
			RegisterStep(new Step210(_logger));
			RegisterStep(new Step220(_logger));
		}

		private void RegisterStep(BaseStep step)
		{
			_logger.LogDebug($"Register step {step.GetVersion()}");
			_steps.Add(step.GetVersion(), step);
		}

		/// <summary>
		/// Get sequence of IDowngradeStep steps
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public IEnumerable<IDowngradeStep> DowngradeSteps()
		{
			if (_steps == null)
			{
				_logger.LogError("ConfigurationStepService.DowngradeSteps dictionary of steps is null.");
				throw new InvalidOperationException("Not registered downgrade steps");
			}
			var stepsCollection = _steps.Reverse().Select(stp => stp.Value).Where(stp => stp is IDowngradeStep);

			_logger.LogInformation($"ConfigurationStepService.DowngradeSteps found IDowngradeStep {stepsCollection.Count()}");
			return stepsCollection;
		}

		/// <summary>
		/// Get version of latest step
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public Version GetLatestStepVersion()
		{
			var latestVersion = _steps.Keys.Max();
			_logger.LogInformation($"ConfigurationStepService.GetLatestStepVersion version {latestVersion}");
			return latestVersion;
		}

		/// <summary>
		/// Get sequence of IUpgradeStep steps
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public IEnumerable<IUpgradeStep> UpgradeSteps()
		{
			if (_steps == null)
			{
				_logger.LogError("ConfigurationStepService.UpgradeSteps dictionary of steps is null.");
				throw new InvalidOperationException("Not registered upgrade steps");
			}
			var stepsCollection = _steps.Select(stp => stp.Value).Where(stp => stp is IUpgradeStep);

			_logger.LogInformation($"ConfigurationStepService.UpgradeSteps found IUpgradeStep {stepsCollection.Count()}");
			return stepsCollection;
		}
	}
}
