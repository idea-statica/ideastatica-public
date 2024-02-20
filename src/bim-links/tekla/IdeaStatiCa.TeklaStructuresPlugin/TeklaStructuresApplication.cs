using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Persistence;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using MoreLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekla.Structures;
using Tekla.Structures.Model.UI;
using TS = Tekla.Structures.Model;
namespace IdeaStatiCa.TeklaStructuresPlugin
{
	public class TeklaStructuresApplication : CadApplication
	{
		private readonly IProject _project;
		protected readonly IBimImporter _bimImporter;
		TaskScheduler _taskScheduler;
		IPluginLogger _logger;

		public TeklaStructuresApplication(
			string applicationName,
			IPluginLogger logger,
			IProject project,
			IProjectStorage projectStorage,
			IBimImporter bimImporter,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IBimUserDataSource userDataSource,
			TaskScheduler taskScheduler)
			: base(applicationName, logger, project, projectStorage, bimImporter, bimApiImporter, pluginHook, scopeHook, userDataSource, taskScheduler)
		{
			_logger = logger;
			_bimImporter = bimImporter;
			_project = project;
			_taskScheduler = taskScheduler;
		}

		protected override ModelBIM ImportSelection(CountryCode countryCode, RequestedItemsType requestedType)
		{
			ModelBIM modelBIM = null;
			switch (requestedType)
			{
				case RequestedItemsType.Connections:
				case RequestedItemsType.Substructure:
					modelBIM = _bimImporter.ImportConnections(countryCode);
					break;

				case RequestedItemsType.SingleConnection:
					modelBIM = _bimImporter.ImportSingleConnection(countryCode);
					break;
				case RequestedItemsType.WholeModel:
					return _bimImporter.ImportWholeModel(countryCode);

				default:
					throw new NotImplementedException();
			}

			//ask for welds
			if (modelBIM != null)
			{
				modelBIM.Model.OriginSettings.ImportRecommendedWelds = AskImportRecommendedWelds();
			}
			return modelBIM;
		}

		private bool AskImportRecommendedWelds()
		{
			string messageBoxText = IdeaStatiCa.TeklaStructuresResources.Properties.Resources.AddRecommendedWelds;
			string caption = IdeaStatiCa.TeklaStructuresResources.Properties.Resources.RecommendedWelds;
			System.Windows.MessageBoxButton button = System.Windows.MessageBoxButton.YesNo;
			System.Windows.MessageBoxImage icon = System.Windows.MessageBoxImage.Question;
			bool addRecommendedWelds;
			// Process message box results
			switch (System.Windows.MessageBox.Show(messageBoxText, caption, button, icon))
			{
				case System.Windows.MessageBoxResult.Yes:
					addRecommendedWelds = true;
					break;
				default:
					addRecommendedWelds = false;
					break;
			}

			return addRecommendedWelds;
		}

		protected override void ActivateMethod(List<BIMItemId> items)
		{
			using (CreateScope(CountryCode.None))
			{
				IEnumerable<IIdeaObject> tokens = items
					.Select(x =>
					{
						try
						{
							return _project.GetBimObject(x.Id);
						}
						catch
						{
							return null;
						}
					}).Where(x => x != null);

				Select(tokens);
			}
		}

		protected override void Select(IEnumerable<IIdeaObject> objects)
		{
			try
			{
				UnSelectAllItems();
				if (objects == null || !objects.Any())
				{
					return;
				}

				var selected = objects.SelectMany(i =>
				{
					try
					{
						if (i is IIdeaPersistentObject persistenObject)
						{

							if (persistenObject is IIdeaConnectionPoint connectionPoint)
							{
								var cpObjects = new List<object>();
								connectionPoint.Plates.ForEach(p => cpObjects.Add(p));
								connectionPoint.FoldedPlates.ForEach(fp => cpObjects.Add(fp));
								connectionPoint.Welds.ForEach(w => cpObjects.Add(w));
								connectionPoint.BoltGrids.ForEach(bg => cpObjects.Add(bg));
								connectionPoint.AnchorGrids.ForEach(ag => cpObjects.Add(ag));
								connectionPoint.ConnectedMembers.ForEach(cm => cpObjects.Add(cm.IdeaMember));
								connectionPoint.Cuts.ForEach(c => cpObjects.Add(c));

								return cpObjects.Select(t =>
								{
									var identifier = new Identifier(t.ToString());


									TS.Model model = new TS.Model();
									return model.SelectModelObject(identifier) as object;
								});
							}
							//skip nodes
							else if (persistenObject is IIdeaNode)
							{
								return new List<object>();
							}
							else
							{
								var identifier = new Identifier((persistenObject.Token as IIdentifier)?.GetId().ToString());

								TS.Model model = new TS.Model();
								return new List<object>() { model.SelectModelObject(identifier) as object };
							}
						}
						return new List<object>();
					}
					catch
					{
						return new List<object>();
					}

				}).Where(i => i != null);

				ArrayList objectsToSelect = new ArrayList(selected.ToArray());
				ModelObjectSelector MS = new ModelObjectSelector();
				MS.Select(objectsToSelect);
			}
			catch (Exception ex)
			{
				_logger.LogError("Tekla ActivateInBIM failed with error ", ex);
			}
		}

		private void UnSelectAllItems()
		{
			try
			{
				ArrayList objectsToSelectE = new ArrayList();
				ModelObjectSelector MSe = new ModelObjectSelector();
				MSe.Select(objectsToSelectE);
			}
			catch (Exception ex)
			{
				_logger.LogError("Tekla ActivateInBIM failed with error ", ex);
			}
		}


		protected override string ApplicationName => "Tekla Structures ";
	}
}
