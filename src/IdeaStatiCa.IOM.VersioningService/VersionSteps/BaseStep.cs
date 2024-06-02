using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps
{
	internal abstract class BaseStep : IUpgradeStep, IDowngradeStep
	{
		private readonly IPluginLogger _logger;

		protected BaseStep(IPluginLogger logger)
		{
			this._logger = logger;
		}

		public virtual void DoDownStep(SModel _model)
		{
			throw new NotImplementedException();
		}

		public virtual void DoUpStep(SModel _model)
		{
			throw new NotImplementedException();
		}

		public void UpgradeVersion(SModel _model)
		{
			_logger.LogDebug("UpgradeVersion");
			ChangeVersion(_model);
		}

		public void DowngradeVersion(SModel _model)
		{
			_logger.LogDebug("DowngradeVersion");
			ChangeVersion(_model);
		}

		private void ChangeVersion(SModel _model)
		{
			_logger.LogInformation($"ChangeVersion to {GetVersion()}");
			_model.ChangeElementValue("Version", GetVersion().ToString());
		}

		public virtual Version GetVersion()
		{
			return Version.Parse("0.0.0");
		}
	}
}
