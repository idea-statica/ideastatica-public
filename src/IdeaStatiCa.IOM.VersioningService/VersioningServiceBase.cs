using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Configuration;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.IOM.VersioningService.Tools;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaStatiCa.IOM.VersioningService
{
	public class VersioningServiceBase : IVersioningService
	{
		public bool IsModelEmpty { get; private set; } = true;

		protected SModel _model;
		protected readonly IPluginLogger _logger;
		private readonly IConfigurationStepService _configurationStepService;

		protected IConfigurationStepService ConfigurationStepService => _configurationStepService;

		public VersioningServiceBase(
			IPluginLogger logger,
			IConfigurationStepService configurationStepService
			)
		{
			_logger = logger;
			_configurationStepService = configurationStepService;
		}

		public bool IsModelActual()
		{
			_logger.LogDebug("IsModelActual");

			if (IsModelEmpty)
			{
				_logger.LogDebug("IsModelActual model is empty");
				return true;
			}

			bool isActual = _configurationStepService.GetLatestStepVersion() == VersionTool.GetVersion(_model.Version);
			_logger.LogDebug($"IsModelActual isActual {isActual}");
			return isActual;
		}

		public Version GetCurrentVersion()
		{
			_logger.LogDebug("GetCurrentVersion");
			if (_model == null)
			{
				_logger.LogError("GetCurrentVersion IR model was not loaded.");
				throw new ArgumentException("IR model was not loaded. First call method LoadModel(SModel model)");
			}

			if (string.IsNullOrEmpty(_model.Version))
			{
				_logger.LogError("GetCurrentVersion IR motel has not specified version.");
				throw new ArgumentException("IR motel has not specified version. Model is corrupted");
			}

			_logger.LogInformation($"GetCurrentVersion current version {_model.Version}");
			return VersionTool.GetVersion(_model.Version);
		}

		public void LoadModel(SModel model)
		{
			_logger.LogInformation("LoadModel");

			if (model == null)
			{
				_logger.LogError("LoadModel parameter SModel is null");
				throw new ArgumentNullException("model");
			}

			_model = model;

			if (string.IsNullOrEmpty(_model.Version))
			{
				ISIntermediate openModel = _model.GetModelElement(false);
				if (openModel is null || openModel.IsEmpty())
				{
					IsModelEmpty = true;
					_logger.LogDebug($"LoadModel model is empty");
					return;
				}

				string version = openModel.GetElementValue("Version");

				if (string.IsNullOrEmpty(version))
				{
					_logger.LogError("LoadModel model not contains version element");
					throw new InvalidOperationException("LoadModel model not contains version element");
				}

				_model.Version = version;
			}

			IsModelEmpty = false;
			_logger.LogInformation($"LoadModel in version {_model.Version}");
		}
	}
}
