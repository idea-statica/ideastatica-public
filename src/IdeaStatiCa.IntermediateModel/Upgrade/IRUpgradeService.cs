using IdeaStatiCa.IntermediateModel.Exceptions;
using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IntermediateModel.Tools;

namespace IdeaStatiCa.IntermediateModel.Upgrade
{
	public partial class IRUpgradeService : IIRUpgradeService
	{
		private SModel _model;

		public IRUpgradeService()
		{
			RegisterUpgradeSteps();
		}

		public bool IsModelActual()
		{
			return _upgradeSteps.Keys.Max() == VersionTools.GetVersion(_model.Version);
		}

		public Version GetCurrentVersion()
		{
			if (_model == null)
			{
				throw new ArgumentException("IR motel not load. First call method LoadModel(SModel model)");
			}

			if (string.IsNullOrEmpty(_model.Version))
			{
				throw new ArgumentException("IR motel has not specified version. Model is corrupted");
			}

			return VersionTools.GetVersion(_model.Version);
		}

		public void LoadModel(SModel model)
		{
			_model = model;
			if (string.IsNullOrEmpty(_model.Version))
			{
				var versions = _model.GetElements("Version");
				foreach (var version in versions)
				{
					if (version is SObject sObject && sObject.Properties?.First().Value is SPrimitive sPrimitive)
					{
						_model.Version = sPrimitive.Value;
					}
				}
			}

		}

		public void Upgrade()
		{
			var versionBeforeUpgrade = this.GetCurrentVersion();
			foreach (var step in _upgradeSteps.Values)
			{
				//skip older versions
				if (step.GetVersion() > versionBeforeUpgrade)
				{
					try
					{
						step.DoUpStep(_model);
					}
					catch (Exception ex)
					{
						throw new UpgradeStepException(step.GetVersion(), ex);
					}
				}

			}
		}
	}
}
