using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Persistence;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yjk
{
	public class YjkApplication : BimApiApplication
	{
		private readonly IBimImporter _bimImporter;
		private readonly IProject _project;

		public YjkApplication(
			string applicationName,
			IPluginLogger logger,
			IProject project,
			IProjectStorage projectStorage,
			IBimImporter bimImporter,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IBimUserDataSource userDataSource,
			TaskScheduler taskScheduler,
			bool highlightSelection = true)
			: base(applicationName, logger, project, projectStorage, bimApiImporter, pluginHook, scopeHook, userDataSource, taskScheduler, highlightSelection)
		{
			_bimImporter = bimImporter;
			_project = project;
		}

		protected override ModelBIM ImportSelection(CountryCode countryCode, RequestedItemsType requestedType)
		{
			switch (requestedType)
			{
				case RequestedItemsType.Connections:
					return _bimImporter.ImportConnections(countryCode);

				case RequestedItemsType.Substructure:
					return _bimImporter.ImportMembers(countryCode);

				case RequestedItemsType.Members2D:
					return _bimImporter.ImportMembers2D(countryCode);

				default:
					throw new NotImplementedException();
			}
		}

		protected override List<ModelBIM> Synchronize(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			//throw new NotImplementedException("Sync is not yet supported, this feature is coming soon!");
			List<IIdeaObject> objects = new List<IIdeaObject>();
			foreach(var group in items)
			{
				foreach (var item in group.Items)
				{
					try
					{
						var bimObj = _project.GetBimApiId(item.Id);
						var a = _project.GetIomId(bimObj);
						var c = _project.GetPersistenceToken(a);
						if (bimObj != null)
						{
							//objects.Add(bimObj);
						}

						
					}
					catch (Exception ex)
					{
						// Log item.Id here to find the culprit
						Console.WriteLine($"Error on Id: {item.Id} - {ex.Message}");
					}
				}
			}



			return _bimImporter.ImportSelected(items, countryCode);
		}

		protected override void Select(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members)
		{
			// no nothing
		}
	}
}
