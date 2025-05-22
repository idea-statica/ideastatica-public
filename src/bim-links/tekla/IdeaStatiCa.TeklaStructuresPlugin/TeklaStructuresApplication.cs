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
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
		IProjectStorage _projectStorage;
		private string _projectPath;
		private const string PersistencyStorage = "bimapi-data.json";

		public TeklaStructuresApplication(
			string applicationName,
			string projectPath,
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
			_projectStorage = projectStorage;
			_projectPath = projectPath;
		}


		public override bool IsDataUpToDate()
		{
			var isValid = _projectStorage.IsValid();
			var projectStoragePath = Path.Combine(_projectPath, PersistencyStorage);

			if (isValid && File.Exists(projectStoragePath))
			{
				//check if project is created in old link IdeaStatiCa.TeklaCBFEM202X
				if (System.IO.File.ReadAllText(projectStoragePath).Contains("IdeaStatiCa.TeklaCBFEM", StringComparison.CurrentCultureIgnoreCase))
				{
					return false;
				}
			}

			return isValid;

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

			return modelBIM;
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
					return;

				var model = new TS.Model();
				if (!model.GetConnectionStatus())
				{
					_logger.LogInformation("Tekla model is not available or remoting is not enabled.");
					return;
				}

				var selected = objects.SelectMany(obj =>
				{
					try
					{
						if (obj is IIdeaPersistentObject persistentObject)
						{
							if (persistentObject is IIdeaConnectionPoint connectionPoint)
							{
								var cpObjects = new List<object>();
								cpObjects.AddRange(connectionPoint.Plates);
								cpObjects.AddRange(connectionPoint.FoldedPlates);
								cpObjects.AddRange(connectionPoint.Welds);
								cpObjects.AddRange(connectionPoint.BoltGrids);
								cpObjects.AddRange(connectionPoint.AnchorGrids);
								cpObjects.AddRange(connectionPoint.ConnectedMembers.Select(cm => cm.IdeaMember));
								cpObjects.AddRange(connectionPoint.Cuts);

								return cpObjects.Select(item =>
								{
									var id = new Identifier(item.ToString());
									return model.SelectModelObject(id) as object;
								}).Where(mo => mo != null);
							}
							else if (persistentObject is IIdeaNode)
							{
								// Skip nodes
								return Enumerable.Empty<object>();
							}
							else
							{
								var token = persistentObject.Token as IIdentifier;
								if (token?.GetId() is string idStr)
								{
									var id = new Identifier(idStr);
									var mo = model.SelectModelObject(id);
									if (mo != null)
										return new[] { mo };
								}
							}
						}
					}
					catch
					{
						// Skip on any error for this object
					}
					return Enumerable.Empty<object>();
				}).ToList();

				if (selected.Any())
				{
					ModelObjectSelector selector = new ModelObjectSelector();
					selector.Select(new ArrayList(selected));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Tekla ActivateInBIM failed with error", ex);
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
