using IdeaRS.OpenModel;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdeaApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdeaApp.Services
{
	public class RamFeaApplication : ApplicationBIMAsync
	{
		IProjectInfo _projectInfo;
		IProject _project;
		IProgressMessaging _progressMessaging;
		IPluginLogger _logger;
		IProjectService _projectService;

		IFilePersistence _persistence;

		public RamFeaApplication(IProjectInfo projectInfo, IProject project, IFilePersistence persistence, IProjectService projectService, IProgressMessaging progressMessaging, IPluginLogger logger) : base(logger)
		{
			_persistence = persistence;
			_projectService = projectService;
			_projectInfo = projectInfo;
			_project = project;
			_progressMessaging = progressMessaging;
			_logger = logger;
		}

		protected override string ApplicationName => "RAM-Fea-IDEA_StatiCa";

		protected override async Task ActivateInBIMAsync(List<BIMItemId> items)
		{
			_logger.LogDebug("RamFeaApplication.ActivateInBIMAsync called");
			await Task.CompletedTask;
		}

		protected override Task<ModelBIM> ImportActiveAsync(CountryCode countryCode, RequestedItemsType requestedType)
		{
			try
			{
				_logger.LogDebug("RamFeaApplication.ImportActiveAsync called");
				var modelBIM = _projectService.GetModel(_projectInfo, _project, countryCode);

				return Task.FromResult(modelBIM);
			}
			catch (Exception e)
			{
				_logger.LogError("ImportActiveAsync failed", e);
				throw;
			}
		}

		protected override Task<List<ModelBIM>> ImportSelectionAsync(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			_logger.LogDebug("RamFeaApplication.ImportSelectionAsync called");
			throw new NotImplementedException();
		}
	}
}
