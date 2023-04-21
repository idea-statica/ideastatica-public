﻿using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Persistence;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;
using System.Threading.Tasks;

namespace IdeaStatiCa.BimApiLink
{
	public class CadBimLink : BimLink
	{
		public CadBimLink(string applicationName, string projectPath) : base(applicationName, projectPath)
		{
		}
		public static BimLink Create(string applicationName, string checkbotProjectPath) => new CadBimLink(applicationName, checkbotProjectPath);

		protected override IApplicationBIM Create(
			IPluginLogger logger,
			IBimApiImporter bimApiImporter,
			string projectPath,
			BimImporterConfiguration bimImporterConfiguration,
			IProgressMessaging remoteApp,
			IBimResultsProvider resultsProvider,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IModel feaModel,
			IBimUserDataSource userDataSource,
			TaskScheduler taskScheduler)
		{
			JsonPersistence jsonPersistence = new JsonPersistence(logger);
			JsonProjectStorage projectStorage = new JsonProjectStorage(jsonPersistence, projectPath);
			Project project = new Project(logger, jsonPersistence);
			ProjectAdapter projectAdapter = new ProjectAdapter(project, bimApiImporter);
			CadModelAdapter cadModelAdapter = new CadModelAdapter(bimApiImporter, feaModel as ICadModel, remoteApp, ApplicationName);
			IBimImporter bimImporter = BimImporter.BimImporter.Create(
				cadModelAdapter,
				projectAdapter,
				logger,
				null,
				bimImporterConfiguration,
				remoteApp,
				resultsProvider);
			return CreateApplicationInstace(logger, bimApiImporter, pluginHook, scopeHook, userDataSource, projectStorage, projectAdapter, bimImporter, taskScheduler);
		}

		protected virtual IApplicationBIM CreateApplicationInstace(
			IPluginLogger logger,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IBimUserDataSource userDataSource,
			IProjectStorage projectStorage,
			IProject projectAdapter,
			IBimImporter bimImporter,
			TaskScheduler taskScheduler)
		{
			return new CadApplication(
							ApplicationName,
							logger,
							projectAdapter,
							projectStorage,
							bimImporter,
							bimApiImporter,
							pluginHook,
							scopeHook,
							userDataSource,
							taskScheduler);
		}
	}
}
