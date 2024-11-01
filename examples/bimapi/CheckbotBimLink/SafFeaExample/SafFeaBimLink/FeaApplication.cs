using IdeaRS.OpenModel;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.SAF2IOM;
using IdeaStatiCa.SAF2IOM.BimApi;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SafFeaBimLink
{
	internal class SafFeaApplication : ApplicationBIMAsync
	{
		//Update Logger 
		private readonly IPluginLogger _logger;

		//Update Application Name
		protected override string ApplicationName => "Saf-Fea-IDEA_StatiCa";

		//DO NOT CHANGE PERSISTENCY STORAGE
		private const string PersistencyStorage = "bimapi-data.json";
		private readonly string _persistencyStoragePath;
		private readonly string _workingDirectory;
		private readonly ISAFConverter _converter;

		private readonly ModelClient _FeaModelApi;
		private readonly IProject _project;
		private readonly IFilePersistence _persistence;
		private readonly Saf2BimApiModelConverter _saf2BimApiModelConverter;

		private readonly IProgressMessaging _progressMessaging;

		public SafFeaApplication(ISafDataSource feaModelApi, ISAFConverter safConverter, IFilePersistence persistence, IProject project,
			string workingDirectory, IProgressMessaging progressMessaging, IPluginLogger logger) : base(logger)
		{
			_FeaModelApi = new ModelClient(feaModelApi);
			_converter = safConverter;
			_project = project;
			_persistence = persistence;
			_workingDirectory = workingDirectory;
			_saf2BimApiModelConverter = new Saf2BimApiModelConverter();
			_persistencyStoragePath = Path.Combine(workingDirectory, PersistencyStorage);
			LoadData();
			_progressMessaging = progressMessaging;
			_logger = logger;
		}

		protected override Task ActivateInBIMAsync(List<BIMItemId> items)
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Imports the actively selected items within the open Fea Model.
		/// </summary>
		/// <param name="countryCode"></param>
		/// <param name="requestedType"></param>
		/// <returns></returns>
		protected override async Task<ModelBIM> ImportActiveAsync(CountryCode countryCode, RequestedItemsType requestedType)
		{
			try
			{
				ModelBIM modelBIM;

				_progressMessaging?.InitProgressDialog();
				_progressMessaging?.SendMessage(MessageSeverity.Info, "Waiting for BIM application");

				if (requestedType == RequestedItemsType.Connections)
				{
					_logger.LogDebug("Importing connections");

					(string Path, IReadOnlyCollection<Guid> Selected) response = await _FeaModelApi.GetSelection(false);

					if (response.Selected?.Count == 0)
					{
						_logger.LogDebug("No items selected.");
						return null;
					}

					_progressMessaging?.SendMessage(MessageSeverity.Info, "Converting SAF to IOM");
					SAFModel safModel = ConvertSafFile(response.Path, countryCode);

					_progressMessaging?.SetStage(1, 0, "SAF converted, importing connections");
					modelBIM = _converter.ImportConnections(safModel, countryCode);
				}
				else
				{
					_logger.LogDebug("Importing substructure");

					(string Path, IReadOnlyCollection<Guid> Selected) response = await _FeaModelApi.GetSelection(true);

					if (response.Selected.Count == 0)
					{
						_logger.LogDebug("No items selected.");
						return null;
					}

					_progressMessaging?.SendMessage(MessageSeverity.Info, "Converting SAF to IOM");
					SAFModel safModel = ConvertSafFile(response.Path, countryCode);
					safModel.Selection = response.Selected;
					_progressMessaging?.SetStage(1, 0, "SAF converted, importing members");
					modelBIM = _converter.ImportMember(safModel, countryCode);
				}
#if DEBUG
				SaveModelBIM(modelBIM, Path.Combine(_workingDirectory, "iom_debug.xml"));
#endif

				return modelBIM;
			}
			catch (Exception e)
			{
				_logger.LogWarning(e.Message);
				return null;
			}
			finally
			{
				SaveData();
			}
		}

		/// <summary>
		/// Re-imports or Syncs Based on the Checkbot Selection.
		/// </summary>
		/// <param name="countryCode"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		protected override async Task<List<ModelBIM>> ImportSelectionAsync(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			try
			{
				_logger.LogDebug("Sync of imported objects");

				_progressMessaging?.InitProgressDialog();
				_progressMessaging?.SetStage(1, 0, "Sync of imported objects");

				string path = await _FeaModelApi.GetObjects(GetGuidsToImport(items));
				SAFModel safModel = ConvertSafFile(path, countryCode);

				_progressMessaging?.SetStage(1, 0, $"Processing {items.Count} models");
				List<ModelBIM> models = items
					.Select(x => _converter.Import(safModel, x.Items, countryCode))
					.ToList();

#if DEBUG
				_progressMessaging?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.SavingData);
				for (int i = 0; i < models.Count; i++)
				{
					_progressMessaging?.SetStageLocalised(i + 1, models.Count, LocalisedMessage.SavingData);
					SaveModelBIM(models[i], Path.Combine(_workingDirectory, $"iom_debug_sync{i}.xml"));
				}
#endif
				return models;
			}
			catch (Exception e)
			{
				_logger.LogWarning(e.Message);
				return null;
			}
			finally
			{
				_progressMessaging?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.SavingData);
				SaveData();
			}
		}

		private IEnumerable<Guid> GetGuidsToImport(List<BIMItemsGroup> items)
		{
			return items
				.SelectMany(x => x.Items)
				.Select(x => _project.GetPersistenceToken(x.Id))
				.OfType<PersistenceToken>()
				.Select(x => x.SafId);
		}

		private void LoadData()
		{
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

		private void SaveData()
		{
			_progressMessaging?.SetStageLocalised(1, 0, LocalisedMessage.SavingData);
			using (FileStream fs = new FileStream(_persistencyStoragePath, FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter streamWriter = new StreamWriter(fs))
				{
					_persistence.Save(streamWriter);
				}
			}
		}

		private SAFModel ConvertSafFile(string path, CountryCode countryCode)
		{
			try
			{
				_progressMessaging?.SetStage(1, 0, "Converting SAF file");
				return _saf2BimApiModelConverter.Convert(path, countryCode);
			}
			catch (Exception e)
			{
				_progressMessaging?.SendMessage(MessageSeverity.Error, "Failed to convert SAF");
				_logger.LogError("Failed to convert SAF", e);// CreateSafAttachment(path));
				throw;
			}
		}

		private static void SaveModelBIM(ModelBIM modelBim, string path)
		{
			SaveXML(modelBim.Model, path);
			SaveXML(modelBim.Results, path + "R");
			SaveXML(modelBim.Messages, path + "M");
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

		//private static Action<IIdeaAttachmentAppender> CreateSafAttachment(string path)
		//{
		//	return appender =>
		//	{
		//		appender.AppendAttachment(
		//			"saf.xlsx",
		//			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
		//			new FileStream(path, FileMode.Open, FileAccess.Read));
		//	};
		//}
	}
}
