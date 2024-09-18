using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Persistence;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimApiLink.Scoping;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.BimApiLink
{
	public abstract class BimApiApplication : ApplicationBIM
	{
		private readonly IProject _project;
		private readonly IProjectStorage _projectStorage;
		private readonly IBimApiImporter _bimApiImporter;
		private readonly IPluginHook _pluginHook;
		private readonly IScopeHook _scopeHook;
		private readonly IBimUserDataSource _userDataSource;
		private readonly TaskScheduler _taskScheduler;
		private readonly IPluginLogger _logger;
		private readonly bool _highlightSelection;

		protected override string ApplicationName { get; }

		protected BimApiApplication(
			string applicationName,
			IPluginLogger pluginLogger,
			IProject project,
			IProjectStorage projectStorage,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IBimUserDataSource userDataSource,
			TaskScheduler taskScheduler,
			bool highlightSelection = true):
			base(pluginLogger)
		{
			ApplicationName = applicationName;

			_logger = pluginLogger;
			_project = project;
			_projectStorage = projectStorage;
			_bimApiImporter = bimApiImporter;
			_pluginHook = pluginHook;
			_scopeHook = scopeHook;
			_userDataSource = userDataSource;
			_taskScheduler = taskScheduler;
			_highlightSelection = highlightSelection;
			_projectStorage.Load();
		}

		public override void ActivateInBIM(List<BIMItemId> items)
		{
			if (_highlightSelection) 
			{
				Task task = Task.Factory.StartNew(() =>
				{
					ActivateMethod(items);
				},
				CancellationToken.None,
				TaskCreationOptions.None,
				_taskScheduler);

				task.GetAwaiter().GetResult();
			}					
		}

		protected virtual void ActivateMethod(List<BIMItemId> items)
		{
			using (CreateScope(CountryCode.None))
			{
				List<IIdeaPersistenceToken> tokens = items
					.Where(x => x.Type != BIMItemType.BIMItemsGroup)
					.Select(x => _project.GetPersistenceToken(x.Id))
					.ToList();

				IEnumerable<Identifier<IIdeaNode>> nodes = tokens
					.OfType<Identifier<IIdeaNode>>();

				IEnumerable<Identifier<IIdeaMember1D>> members = tokens
					.OfType<Identifier<IIdeaMember1D>>();

				Select(nodes, members);
			}
		}

		public override bool IsDataUpToDate()
			=> _projectStorage.IsValid();

		protected override ModelBIM ImportActive(CountryCode countryCode, RequestedItemsType requestedType)
		{
			Task<ModelBIM> task = Task.Factory.StartNew(() =>
			{
				_logger.LogEventInformation(new BimConnectionsImportEvent(
					   ApplicationName,
					   Enum.GetName(typeof(RequestedItemsType), requestedType),
					   Enum.GetName(typeof(CountryCode), countryCode))
					);
				using (CreateScope(countryCode))
				{
					_pluginHook.EnterImport(countryCode);
					_pluginHook.EnterImportSelection(requestedType);

					try
					{
						return ImportSelection(countryCode, requestedType);
					}
					finally
					{
						ImportFinished();

						_pluginHook.ExitImportSelection(requestedType);
						_pluginHook.ExitImport(countryCode);
					}
				}
			},
			CancellationToken.None,
			TaskCreationOptions.None,
			_taskScheduler);

			return task.GetAwaiter().GetResult();
		}

		protected override List<ModelBIM> ImportSelection(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			Task<List<ModelBIM>> task = Task.Factory.StartNew(() =>
			{
				_logger.LogEventInformation(new BimConnectionsSynchronizeEvent(
					   ApplicationName,
					   Enum.GetName(typeof(CountryCode), countryCode))
					);
				using (CreateScope(countryCode))
				{
					_pluginHook.EnterImport(countryCode);
					try
					{
						return Synchronize(countryCode, items);
					}
					finally
					{
						ImportFinished();

						_pluginHook.ExitImport(countryCode);
					}
				}
			},
			CancellationToken.None,
			TaskCreationOptions.None,
			_taskScheduler);

			return task.GetAwaiter().GetResult();
		}

		protected abstract void Select(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members);

		protected abstract ModelBIM ImportSelection(CountryCode countryCode, RequestedItemsType requestedType);

		protected abstract List<ModelBIM> Synchronize(CountryCode countryCode, List<BIMItemsGroup> items);

		private void ImportFinished()
			=> _projectStorage.Save();

		protected BimLinkScope CreateScope(CountryCode countryCode)
		{
			_scopeHook.PreScope();

			object userData = _userDataSource.GetUserData();

			return new BimLinkScope(
				new BimApiImporterCacheAdapter(_bimApiImporter),
				_scopeHook,
				countryCode,
				userData);
		}
	}
}