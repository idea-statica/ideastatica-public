using IdeaRS.OpenModel;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.SAF2IOM;
using IdeaStatiCa.SAF2IOM.BimApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SafFeaExampleLink
{
	/// <summary>
	/// Answers Checkbot's requests. For each one it asks the FEA application for a SAF file, reads the file with
	/// IdeaStatiCa.SAF2IOM, and lets BimImporter build the IDEA StatiCa model from it.
	/// </summary>
	internal class SafApplication : ApplicationBIM
	{
		/// <summary>
		/// Maps the SAF id of every imported object to the id Checkbot knows the object by. The file is kept in the
		/// Checkbot project folder, so a sync finds the objects again after Checkbot has been closed and opened.
		/// </summary>
		private const string IdMapFileName = "bimapi-data.json";

		/// <summary>
		/// The SAF file the FEA application writes for each request. It stays in the Checkbot project folder, so you
		/// can open it to see what was imported last. While Excel has it open, the next request cannot overwrite it.
		/// </summary>
		private const string SafFileName = "export.xlsx";

		private readonly ISafExport _safExport;
		private readonly string _safFilePath;
		private readonly string _idMapPath;
		private readonly JsonPersistence _idMap;
		private readonly Project _project;
		private readonly Saf2BimApiModelConverter _safConverter = new Saf2BimApiModelConverter();

		public SafApplication(string projectPath, ISafExport safExport, IPluginLogger logger) : base(logger)
		{
			_safExport = safExport;
			_safFilePath = Path.Combine(projectPath, SafFileName);
			_idMapPath = Path.Combine(projectPath, IdMapFileName);
			_idMap = new JsonPersistence(logger);
			_project = new Project(logger, _idMap);

			LoadIdMap();
		}

		/// <summary>
		/// The name Checkbot shows for the linked application.
		/// </summary>
		protected override string ApplicationName => "SAF FEA Example";

		/// <summary>
		/// Checkbot asks the FEA application to select these objects, to show the user where they are. The example
		/// application has no view to select them in.
		/// </summary>
		public override void ActivateInBIM(List<BIMItemId> items)
		{
		}

		/// <summary>
		/// Answers Checkbot's import: the objects the user has selected in the FEA application.
		/// </summary>
		protected override ModelBIM ImportActive(CountryCode countryCode, RequestedItemsType requestedType)
		{
			_safExport.ExportSelection(_safFilePath);
			return Import(countryCode, new[] { requestedType }).Single();
		}

		/// <summary>
		/// Answers Checkbot's sync: the objects imported before, as they are now in the FEA application. Checkbot groups
		/// them by what they were imported as, and gets one model for each group.
		/// </summary>
		protected override List<ModelBIM> ImportSelection(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			List<Guid> safIds = items
				.SelectMany(group => group.Items)
				.Select(item => FindSafId(item.Id))
				.Where(safId => safId.HasValue)
				.Select(safId => safId.Value)
				.Distinct()
				.ToList();

			_safExport.ExportObjects(_safFilePath, safIds);
			return Import(countryCode, items.Select(group => group.Type).Distinct());
		}

		/// <summary>
		/// The SAF id of the object Checkbot knows by <paramref name="iomId"/>, or null when it has none. Nodes and members
		/// have one. Connections and Checkbot's own groups do not: they are not in the SAF file.
		/// </summary>
		private Guid? FindSafId(int iomId)
		{
			try
			{
				return (_project.GetPersistenceToken(iomId) as PersistenceToken)?.SafId;
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}

		private List<ModelBIM> Import(CountryCode countryCode, IEnumerable<RequestedItemsType> requestedTypes)
		{
			SAFModel safModel = _safConverter.Convert(_safFilePath, countryCode);
			IBimImporter importer = BimImporter.Create(
				safModel,
				_project,
				ideaLoggerBase,
				configuration: new SAFBimImporterConfiguration(),
				remoteApp: Progress);

			List<ModelBIM> models = requestedTypes.Select(type => Import(importer, type, countryCode)).ToList();
			SaveIdMap();
			return models;
		}

		/// <summary>
		/// Connections are the joints where the members meet, for IDEA StatiCa Connection. Members are imported on their
		/// own for IDEA StatiCa Member.
		/// </summary>
		private static ModelBIM Import(IBimImporter importer, RequestedItemsType requestedType, CountryCode countryCode)
		{
			switch (requestedType)
			{
				case RequestedItemsType.Connections:
					return importer.ImportConnections(countryCode);
				case RequestedItemsType.Substructure:
					return importer.ImportMembers(countryCode);
				default:
					throw new NotSupportedException($"The SAF FEA example does not import {requestedType}.");
			}
		}

		private void LoadIdMap()
		{
			if (File.Exists(_idMapPath))
			{
				using (StreamReader reader = File.OpenText(_idMapPath))
				{
					_idMap.Load(reader);
				}
			}
		}

		private void SaveIdMap()
		{
			using (StreamWriter writer = File.CreateText(_idMapPath))
			{
				_idMap.Save(writer);
			}
		}
	}
}
