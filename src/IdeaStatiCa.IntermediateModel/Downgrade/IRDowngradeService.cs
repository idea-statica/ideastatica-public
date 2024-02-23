using IdeaStatiCa.IntermediateModel.Exceptions;
using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IntermediateModel.Tools;

namespace IdeaStatiCa.IntermediateModel.Downgrade
{
	public partial class IRDowngradeService : IIRDowngradeService
	{
		private SModel _model;

		public IRDowngradeService()
		{
			RegisterDowngradeSteps();
		}

		public bool IsModelActual()
		{
			return _downgradeSteps.Keys.Max() == VersionTools.GetVersion(_model.Version);
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

		public void Downgrade(string version)
		{
			var versionToDowngrade = VersionTools.GetVersion(version);
			this.Downgrade(versionToDowngrade);
		}

		public void Downgrade(Version version)
		{
			var versionBeforeDowngrade = this.GetCurrentVersion();
			foreach (var keyValuePair in _downgradeSteps.Reverse())
			{
				var step = keyValuePair.Value;
				//skip newer versions and also skip downgrade steps lower as required version
				if (step.GetVersion() <= versionBeforeDowngrade && step.GetVersion() >= version)
				{
					try
					{
						//downgrade version information
						step.DowngradeVersion(_model);

						//if its not last step for fixing version number do down step
						if (step.GetVersion() > version)
						{
							step.DoDownStep(_model);
						}
					}
					catch (Exception ex)
					{
						throw new DowngradeStepException(step.GetVersion(), ex);
					}
				}

			}
		}

		public IEnumerable<Version> GetVersionsToDowngrade()
		{
			var currentVersion = this.GetCurrentVersion();
			List<Version> availableVersionsForDowngrade = new List<Version>();
			foreach (var keyValuePair in _downgradeSteps.Reverse())
			{
				var step = keyValuePair.Value;
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
