using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatica.BimApiLink.Persistence;
using IdeaStatica.BimApiLink.Scoping;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;

namespace IdeaStatica.BimApiLink
{
	public abstract class BimApiApplication : ApplicationBIM
	{
		private readonly IProject _project;
		private readonly IProjectStorage _projectStorage;
		private readonly IBimApiImporter _bimApiImporter;

		protected override string ApplicationName { get; }

		protected BimApiApplication(
			string applicationName,
			IProject project,
			IProjectStorage projectStorage,
			IBimApiImporter bimApiImporter)
		{
			ApplicationName = applicationName;

			_project = project;
			_projectStorage = projectStorage;
			_bimApiImporter = bimApiImporter;
			projectStorage.Load();
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

		protected override ModelBIM ImportActive(CountryCode countryCode, RequestedItemsType requestedType)
		{
			using (CreateScope(countryCode))
			{
				try
				{
					return ImportSelection(countryCode, requestedType);
				}
				finally
				{
					ImportFinished();
				}
			}
		}

		protected override List<ModelBIM> ImportSelection(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			using (CreateScope(countryCode))
			{
				try
				{
					return Synchronize(countryCode, items);
				}
				finally
				{
					ImportFinished();
				}
			}
		}

		protected abstract void Select(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members);

		protected abstract ModelBIM ImportSelection(CountryCode countryCode, RequestedItemsType requestedType);

		protected abstract List<ModelBIM> Synchronize(CountryCode countryCode, List<BIMItemsGroup> items);

		private void ImportFinished()
			=> _projectStorage.Save();

		private BimLinkScope CreateScope(CountryCode countryCode)
		{
			return new BimLinkScope(new BimApiImporterCacheAdapter(_bimApiImporter), countryCode);
		}
	}
}