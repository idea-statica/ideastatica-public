using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Hooks;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatica.BimApiLink.Persistence;
using IdeaStatica.BimApiLink.Scoping;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatica.BimApiLink
{
	public abstract class BimApiApplication : ApplicationBIM
	{
		private readonly IProject _project;
		private readonly IProjectStorage _projectStorage;
		private readonly IBimApiImporter _bimApiImporter;
		private readonly IPluginHook _pluginHook;
		private readonly IPluginHookNoScope _pluginHookNoScope;
		private readonly IBimUserDataSource _userDataSource;

		protected override string ApplicationName { get; }

		protected BimApiApplication(
			string applicationName,
			IProject project,
			IProjectStorage projectStorage,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IPluginHookNoScope pluginHookNoScope,
			IBimUserDataSource userDataSource)
		{
			ApplicationName = applicationName;

			_project = project;
			_projectStorage = projectStorage;
			_bimApiImporter = bimApiImporter;
			_pluginHook = pluginHook;
			_pluginHookNoScope = pluginHookNoScope;
			_userDataSource = userDataSource;

			_projectStorage.Load();
		}

		public override void ActivateInBIM(List<BIMItemId> items)
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
			_pluginHookNoScope.PreImport(countryCode);
			try
			{
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
			}
			finally
			{
				_pluginHookNoScope.PostImport(countryCode);
			}
		}

		protected override List<ModelBIM> ImportSelection(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			_pluginHookNoScope.PreImport(countryCode);
			try
			{
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
			}
			finally
			{
				_pluginHookNoScope.PostImport(countryCode);
			}
		}

		protected abstract void Select(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members);

		protected abstract ModelBIM ImportSelection(CountryCode countryCode, RequestedItemsType requestedType);

		protected abstract List<ModelBIM> Synchronize(CountryCode countryCode, List<BIMItemsGroup> items);

		private void ImportFinished()
			=> _projectStorage.Save();

		private BimLinkScope CreateScope(CountryCode countryCode)
		{
			object userData = _userDataSource.GetUserData();

			return new BimLinkScope(new BimApiImporterCacheAdapter(_bimApiImporter), countryCode, userData);
		}
	}
}