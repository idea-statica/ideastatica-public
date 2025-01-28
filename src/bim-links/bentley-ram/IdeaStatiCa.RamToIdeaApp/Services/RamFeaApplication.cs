using IdeaRS.OpenModel;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdeaApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdeaApp.Services
{
	/// <summary>
	/// Responsible for creating of <see cref="ModelBIM"/> from Ram model."
	/// Callback functions from Checkbot to import active and selected items from Ram
	/// </summary>
	public class RamFeaApplication : ApplicationBIMAsync
	{
		private readonly IProjectInfo _projectInfo;
		private readonly IProject _project;
		private readonly IProgressMessaging _progressMessaging;
		private readonly IPluginLogger _logger;
		private readonly IProjectService _projectService;
		private readonly string _persistencyStoragePath;

		IFilePersistence _persistence;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="projectInfo">Location of RAM database and Checkbot project on the disk</param>
		/// <param name="project"></param>
		/// <param name="persistence"></param>
		/// <param name="projectService"></param>
		/// <param name="progressMessaging"></param>
		/// <param name="logger"></param>
		public RamFeaApplication(IProjectInfo projectInfo, IProject project, IFilePersistence persistence, IProjectService projectService, IProgressMessaging progressMessaging, IPluginLogger logger) : base(logger)
		{
			_persistence = persistence;
			_projectService = projectService;
			_projectInfo = projectInfo;
			_project = project;
			_progressMessaging = progressMessaging;
			_logger = logger;
			_persistencyStoragePath = Path.Combine(projectInfo.ProjectWorkingDir, PersistencyStorage);
			LoadMapping();
		}

		/// <summary>
		/// Name of the BIM application which will be shown in IdeaCheckbot
		/// </summary>
		protected override string ApplicationName => "RAM-Fea-IDEA_StatiCa";

		// Name of the json file where the mapping between BIM items and FEA items is stored
		//DO NOT CHANGE PERSISTENCY STORAGE (taken from RamImportService.cs)
		private const string PersistencyStorage = "id-mapping.json";

		/// <summary>
		/// Callback for Checkbot to activate selected items in BIM application
		/// Not implemented for RAM
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		protected override async Task ActivateInBIMAsync(List<BIMItemId> items)
		{
			_logger.LogDebug("RamFeaApplication.ActivateInBIMAsync called");
			await Task.CompletedTask;
		}

		/// <summary>
		/// Callback for Checkbot to import active items from BIM application
		/// </summary>
		/// <param name="countryCode">The country code requested by Checkbot</param>
		/// <param name="requestedType">RAM supports only connections</param>
		/// <returns></returns>
		protected override Task<ModelBIM> ImportActiveAsync(CountryCode countryCode, RequestedItemsType requestedType)
		{
			ModelBIM result = null;
			try
			{
				_logger.LogDebug($"RamFeaApplication.ImportActiveAsync : countryCode = {countryCode}, requestedType = {requestedType}");

				_progressMessaging?.InitProgressDialog();
				_progressMessaging?.SendMessage(MessageSeverity.Info, "Waiting for BIM application");

				if (requestedType == RequestedItemsType.Connections)
				{
					_progressMessaging?.SendMessage(MessageSeverity.Info, "Converting Ram Model to IOM");
					result = _projectService.GetModel(_projectInfo, _project, countryCode);
				}
				else
				{
					_logger.LogDebug("RamFeaApplication.ImportActiveAsync : Member import is not implemented");
				}

				return Task.FromResult(result);
			}
			catch (Exception e)
			{
				_logger.LogError("ImportActiveAsync failed", e);
				throw;
			}
			finally
			{
				SaveMapping();
			}
		}

		/// <summary>
		/// TODO: Implement import of selected items from RAM
		/// </summary>
		/// <param name="countryCode"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		protected override Task<List<ModelBIM>> ImportSelectionAsync(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			_logger.LogDebug("RamFeaApplication.ImportSelectionAsync called");
			throw new NotImplementedException();
		}

		private void LoadMapping()
		{
			_logger.LogDebug($"RamFeaApplication.LoadMapping {_persistencyStoragePath}");
			if (File.Exists(_persistencyStoragePath))
			{
				using (FileStream fs = new FileStream(_persistencyStoragePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (StreamReader streamReader = new StreamReader(fs))
					{
						_persistence.Load(streamReader);
					}
				}
			}
		}

		private void SaveMapping()
		{
			_logger.LogDebug($"RamFeaApplication.SaveMapping {_persistencyStoragePath}");
			_progressMessaging?.SetStageLocalised(1, 0, LocalisedMessage.SavingData);
			using (FileStream fs = new FileStream(_persistencyStoragePath, FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter streamWriter = new StreamWriter(fs))
				{
					_persistence.Save(streamWriter);
				}
			}
		}
	}
}
