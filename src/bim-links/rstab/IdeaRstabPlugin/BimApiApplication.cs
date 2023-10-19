using IdeaRS.OpenModel;
using IdeaRstabPlugin.Factories;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IdeaRstabPlugin
{
	public abstract class BimApiApplication : ApplicationBIM
	{
		private static readonly IPluginLogger ideaLogger = LoggerProvider.GetLogger("ideastatica.IdeaRstabPlugin.bimapiapplication");

		private const string PersistencyStorage = "bimapi-data.json";

		private readonly IBimImporter _bimImporter;
		private readonly JsonPersistence _jsonPersistence;
		private readonly Project _project;
		private readonly string _persistencyStoragePath;
		private readonly string _workingDirectory;

		protected event Action<CountryCode, RequestedItemsType> ImportStarted;

		protected event Action<IEnumerable<IIdeaObject>> SychronizationStarted;

		protected event Action ImportFinished;

		public BimApiApplication(
			IPluginLogger pluginLogger,
			IIdeaModel ideaModel,
			IObjectRestorer objectRestorer,
			IGeometryProvider geometryProvider,
			string workingDirectory):
			base(pluginLogger)
		{
			_workingDirectory = workingDirectory;
			_persistencyStoragePath = Path.Combine(workingDirectory, PersistencyStorage);

			_jsonPersistence = new JsonPersistence(pluginLogger);
			if (File.Exists(_persistencyStoragePath))
			{
				using (FileStream fs = new FileStream(_persistencyStoragePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (StreamReader streamReader = new StreamReader(fs))
					{
						_jsonPersistence.Load(streamReader);
					}
				}
			}

			BimImporterConfiguration bimImporterConfiguration = new BimImporterConfiguration()
			{
				ResultSectionPositionPrecision = ResultsFactory.ResultsPrecision,
				LCSPrecision = 1e-5
			};

			_project = new Project(pluginLogger, _jsonPersistence, objectRestorer);
			_bimImporter = BimImporter.Create(ideaModel, _project, pluginLogger, geometryProvider, bimImporterConfiguration, null, null);

			ImportFinished += OnImportFinished;
		}

		private void OnImportFinished()
		{
			using (FileStream fs = new FileStream(_persistencyStoragePath, FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter streamWriter = new StreamWriter(fs))
				{
					_jsonPersistence.Save(streamWriter);
				}
			}
		}

		public override void ActivateInBIM(List<BIMItemId> items)
		{
			try
			{
				if (items == null || items.Count == 0)
				{
					Deselect();
					return;
				}

				Select(items.Where(x => x.Type != BIMItemType.BIMItemsGroup)
					.Select(x => _project.GetBimObject(x.Id)));
			}
			catch (Exception e)
			{
				ideaLogger.LogError("ActivateInBIM failed", e);
			}
		}

		protected override ModelBIM ImportActive(CountryCode countryCode, RequestedItemsType requestedType)
		{
			ImportStarted?.Invoke(countryCode, requestedType);
			try
			{
				ModelBIM modelBIM = null;

				if (requestedType == RequestedItemsType.Connections)
				{
					modelBIM = _bimImporter.ImportConnections(countryCode);
				}
				else
				{
					modelBIM = _bimImporter.ImportMembers(countryCode);
				}

#if DEBUG
				SaveModelBIM(modelBIM, Path.Combine(_workingDirectory, "iom_debug.xml"));
#endif

				return modelBIM;
			}
			catch (Exception e)
			{
				ideaLogger.LogError("ImportActive failed", e);
				return null;
			}
			finally
			{
				ImportFinished?.Invoke();
			}
		}

		protected override List<ModelBIM> ImportSelection(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			ImportStarted?.Invoke(countryCode, RequestedItemsType.Connections);
			SychronizationStarted?.Invoke(items
				.SelectMany(x => x.Items)
				.Select(x => _project.GetBimObject(x.Id)));

			try
			{
				return _bimImporter.ImportSelected(items, countryCode);
			}
			catch (Exception e)
			{
				ideaLogger.LogError("ImportSelection failed", e);
				return null;
			}
			finally
			{
				ImportFinished?.Invoke();
			}
		}

		protected abstract void Select(IEnumerable<IIdeaObject> objects);

		protected abstract void Deselect();

		private static void SaveModelBIM(ModelBIM modelBim, string path)
		{
			SaveXML(modelBim.Model, path);
			SaveXML(modelBim.Results, path);
			SaveXML(modelBim.Messages, path);
		}

		private static void SaveXML<T>(T data, string path)
		{
			if (data == null)
			{
				return;
			}

			XmlSerializer xs = new XmlSerializer(typeof(T));
			using (XmlTextWriter writer = new XmlTextWriter(path, Encoding.Unicode))
			{
				writer.Formatting = Formatting.Indented;
				xs.Serialize(writer, data);
			}
		}
	}
}