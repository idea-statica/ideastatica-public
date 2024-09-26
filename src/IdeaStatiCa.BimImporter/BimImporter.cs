using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Extensions;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IdeaStatiCa.BimImporter
{
	/// <inheritdoc cref="IBimImporter"/>
	public class BimImporter : IBimImporter
	{
		private static readonly IIdeaObjectComparer _ideaObjectComparer = new IIdeaObjectComparer();

		public readonly IIdeaModel IdeaModel;
  
		private readonly IPluginLogger _logger;		
		private readonly IProject _project;
		private readonly IGeometryProvider _geometryProvider;
		private readonly IBimObjectImporter _bimObjectImporter;
		private readonly IProgressMessaging _remoteApp;
		private readonly BimImporterConfiguration _configuration;

		/// <summary>
		///Creates instance of <see cref="BimImporter"/>.
		/// </summary>
		/// <param name="ideaModel">Model for importing</param>
		/// <param name="project"><see cref="IProject"/> instance for id mapping persistence</param>
		/// <param name="logger">Logger</param>
		/// <param name="geometryProvider">Geometry provider</param>
		/// <param name="configuration">Importer configuration</param>
		/// <returns>Instance of <see cref="BimImporter"/></returns>
		/// <exception cref="ArgumentNullException">Throws when some argument is null.</exception>
		public static IBimImporter Create(IIdeaModel ideaModel, IProject project, IPluginLogger logger,
			IGeometryProvider geometryProvider, BimImporterConfiguration configuration) => Create(ideaModel, project, logger, geometryProvider, configuration, null, null);

		public static IBimImporter Create(
			IIdeaModel ideaModel,
			IProject project,
			IPluginLogger logger = null,
			IGeometryProvider geometryProvider = null,
			BimImporterConfiguration configuration = null,
			IProgressMessaging remoteApp = null,
			IBimResultsProvider resultsProvider = null)
		{
			logger = logger ?? new NullLogger();
			geometryProvider = geometryProvider ?? new DefaultGeometryProvider(logger, ideaModel);
			configuration = configuration ?? new BimImporterConfiguration();
			resultsProvider = resultsProvider ?? new DefaultResultsProvider();

			return new BimImporter(
				ideaModel,
				project,
				logger,
				geometryProvider,
				configuration,
				BimObjectImporter.Create(logger, configuration, resultsProvider, remoteApp),
				remoteApp);
		}

		internal BimImporter(
			IIdeaModel ideaModel,
			IProject project,
			IPluginLogger logger,
			IGeometryProvider geometryProvider,
			BimImporterConfiguration configuration,
			IBimObjectImporter bimObjectImporter,
			IProgressMessaging remoteApp = null)
		{
			IdeaModel = ideaModel ?? throw new ArgumentNullException(nameof(ideaModel));
			_project = project ?? throw new ArgumentNullException(nameof(project));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_geometryProvider = geometryProvider ?? throw new ArgumentNullException(nameof(geometryProvider));
			_configuration = configuration;
			_bimObjectImporter = bimObjectImporter ?? throw new ArgumentNullException(nameof(bimObjectImporter));
			_remoteApp = remoteApp;
		}

		/// <inheritdoc cref="IBimImporter.ImportConnections"/>
		/// <remarks>Nodes are marked as a connection by following rules:<br/>
		///  - nodes specified in <see cref="IIdeaModel.GetSingleSelection"/> are connections,<br/>
		///  - nodes with two or more connecting member are connections.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSingleSelection"/> returns null arguments.</exception>
		public ModelBIM ImportConnections(CountryCode countryCode)
		{
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ImportingConnections);

			BulkSelection selection = InitBulkImport();
			return ProcessSelectedModel(countryCode, selection);
		}

		private ModelBIM ProcessSelectedModel(CountryCode countryCode, BulkSelection selection)
		{
			List<Connection> connections = new List<Connection>();

			if (selection.ConnectionPoints != null)
			{
				foreach (IIdeaConnectionPoint connectionPoint in selection.ConnectionPoints)
				{
					connections.Add(Connection.FromConnectionPoint(connectionPoint));
				}
			}

			bool skipAutoCreationOfConnection = connections.Count > 0;

			if (!skipAutoCreationOfConnection)
			{
				foreach (KeyValuePair<IIdeaNode, HashSet<IIdeaMember1D>> keyValue in GetConnections(selection.Members))
				{
					if (selection.Nodes.Contains(keyValue.Key) || keyValue.Value.Count >= 2)
					{
						Connection newConnection = Connection.FromNodeAndMembers(keyValue.Key, keyValue.Value);

						if (!connections.Exists(
							 c =>
								(newConnection.ReferencedObject as IIdeaConnectionPoint).Node.IsAlmostEqual(
									 (c.ReferencedObject as IIdeaConnectionPoint).Node, _configuration.GeometryPrecision)
							))
						{
							connections.Add(Connection.FromNodeAndMembers(keyValue.Key, keyValue.Value));
						}
					}
				}
			}

			IEnumerable<IIdeaObject> objects = selection.Nodes
				.Cast<IIdeaObject>()
				.Concat(selection.Members);

			if (selection.Members2D != null)
			{
				objects = objects.Concat(selection.Members2D);
			}

			return CreateModelBIM(objects, connections, countryCode);
		}

		/// <inheritdoc cref="IBimImporter.ImportWholeModel"/>
		/// <remarks>Nodes are marked as a connection by following rules:<br/>
		///  - nodes specified in <see cref="IIdeaModel.GetSingleSelection"/> are connections,<br/>
		///  - nodes with two or more connecting member are connections.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSingleSelection"/> returns null arguments.</exception>
		public ModelBIM ImportWholeModel(CountryCode countryCode)
		{
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ImportingConnections);
			BulkSelection selection = InitImportOfWholeModel();
			return ProcessSelectedModel(countryCode, selection);
		}

		/// <inheritdoc cref="IBimImporter.ImportSingleConnection"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSingleSelection"/> returns null arguments.</exception>
		public ModelBIM ImportSingleConnection(CountryCode countryCode)
		{
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ImportingConnections);
			SingleSelection selection = InitSingleImport();

			List<Connection> connections = new List<Connection>();

			if (selection.ConnectionPoint != null)
			{
				connections.Add(Connection.FromConnectionPoint(selection.ConnectionPoint));
			}

			IEnumerable<IIdeaObject> objects = selection.Nodes
				.Cast<IIdeaObject>()
				.Concat(selection.Members);

			return CreateModelBIM(objects, connections, countryCode);
		}

		/// <inheritdoc cref="IBimImporter.ImportMember"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSingleSelection"/> returns null arguments.</exception>
		public ModelBIM ImportMembers(CountryCode countryCode)
		{
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ImportingMembers);
			BulkSelection selection = InitBulkImport();
			IGeometry geometry = _geometryProvider.GetGeometry();

			List<IBimItem> bimItems = new List<IBimItem>();
			HashSet<IIdeaNode> adjacentNodes = new HashSet<IIdeaNode>(_ideaObjectComparer);
			if (selection.ConnectionPoints != null)
			{
				foreach (IIdeaConnectionPoint connectionPoint in selection.ConnectionPoints)
				{
					bimItems.Add(Connection.FromConnectionPoint(connectionPoint));
				}
			}

			int j = 1;
			foreach (IIdeaMember1D selectedMember in selection.Members)
			{
				_remoteApp?.SetStageLocalised(j, selection.Members.Count, LocalisedMessage.Member);
				bimItems.Add(new Member(selectedMember));

				foreach (IIdeaNode node in geometry.GetNodesOnMember(selectedMember))
				{
					adjacentNodes.Add(node);
				}
				j++;
			}

			foreach (IIdeaNode node in adjacentNodes)
			{
				bimItems.Add(Connection.FromNodeAndMembers(node, new HashSet<IIdeaMember1D>(geometry.GetConnectedMembers(node))));
			}

			IEnumerable<IIdeaObject> objects = selection.Nodes
				.Cast<IIdeaObject>()
				.Concat(selection.Members);

			return CreateModelBIM(objects, bimItems, countryCode);
		}

		/// <inheritdoc cref="IBimImporter.ImportSelected"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSingleSelection"/> returns null arguments.</exception>
		public List<ModelBIM> ImportSelected(List<BIMItemsGroup> selected, CountryCode countryCode)
		{
			if (selected is null)
			{
				throw new ArgumentNullException(nameof(selected));
			}

			_logger.LogTrace($"Importing of '{selected.Count}' selected items.");

			return selected.Select(x => ImportGroup(x, countryCode)).ToList();
		}

		/// <inheritdoc cref="IBimImporter.Import"/>
		/// <exception cref="ArgumentNullException">Throws when argument is null.</exception>
		public ModelBIM Import(IEnumerable<IIdeaObject> objects, CountryCode countryCode)
		{
			if (objects is null)
			{
				throw new ArgumentNullException(nameof(objects));
			}

			return CreateModelBIM(objects, Enumerable.Empty<IBimItem>(), countryCode);
		}

		/// <summary>
		/// Imports items from <paramref name="group"/> into <see cref="ModelBIM"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException">If any argument is null.</exception>
		/// <exception cref="NotImplementedException">If the group type is not a connection or a substructure.</exception>
		private ModelBIM ImportGroup(BIMItemsGroup group, CountryCode countryCode)
		{
			if (group is null)
			{
				throw new ArgumentNullException(nameof(group));
			}

			_remoteApp?.InitProgressDialog();
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ImportingGroups);

			_logger.LogTrace($"Importing of bim items group, id '{group.Id}', type '{group.Type}', items count '{group.Items}'.");

			if (group.Type == RequestedItemsType.Connections || group.Type == RequestedItemsType.SingleConnection)
			{
				List<IIdeaObject> objects = group.Items
					.Select(x => _project.GetBimObject(x.Id))
					.Where(x => x != null)
					.ToList();

				IIdeaNode node = objects
					.OfType<IIdeaNode>()
					.First();

				IEnumerable<IIdeaConnectionPoint> connectionPoints = objects.OfType<IIdeaConnectionPoint>();
				List<Connection> connections = new List<Connection>();

				if (connectionPoints.Any())
				{
					IIdeaConnectionPoint cp = connectionPoints.FirstOrDefault();
					if (cp != null)
					{
						connections.Add(Connection.FromConnectionPoint(cp));
						//process connection
						if (IdeaModel is IIdeaConnectionModel connectionModel)
						{
							connectionModel.ProcessConnection(cp);
						}
					}
				}
				else
				{
					IEnumerable<IIdeaMember1D> members = objects.OfType<IIdeaMember1D>();

					// get all possible connections from current members
					Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> allPossibleConnections = GetConnections(members);
					if (allPossibleConnections.Count > 0)
					{
						// find first connection that matches current node
						KeyValuePair<IIdeaNode, HashSet<IIdeaMember1D>> con = allPossibleConnections.FirstOrDefault(x => x.Key.Id == node.Id);
						if (con.Value != null && con.Key != null)
						{
							connections.Add(Connection.FromNodeAndMembers(node, con.Value));
						}
					}
				}

				return CreateModelBIM(objects, connections, countryCode);
			}
			else if (group.Type == RequestedItemsType.Substructure)
			{
				IEnumerable<Member> bimItems = group.Items
					.Where(x => x.Type == BIMItemType.Member)
					.Select(x => _project.GetBimObject(x.Id))
					.Where(x => x != null)
					.Cast<IIdeaMember1D>()
					.Select(x => new Member(x));

				return CreateModelBIM(Enumerable.Empty<IIdeaObject>(), bimItems, countryCode);
			}
			else
			{
				_remoteApp?.CancelMessage();
				_logger.LogError($"BIMItemsGroup type '{group.Type}' is not supported.");
				throw new NotImplementedException($"BIMItemsGroup type '{group.Type}' is not supported.");
			}
		}

		private BulkSelection InitBulkImport()
		{
			_remoteApp?.InitProgressDialog();
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ModelImport);
			BulkSelection selection = IdeaModel.GetBulkSelection();

			CheckNodesAndMembers(selection);

			return selection;
		}

		private BulkSelection InitImportOfWholeModel()
		{
			_remoteApp?.InitProgressDialog();
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ModelImport);
			BulkSelection selection = IdeaModel.GetWholeModel();
			CheckNodesAndMembers(selection);

			return selection;
		}

		private static void CheckNodesAndMembers(Selection selection)
		{
			if (selection.Nodes == null)
			{
				throw new InvalidOperationException("Argument 'nodes' in GetSelection cannot be null.");
			}

			if (selection.Members == null)
			{
				throw new InvalidOperationException("Argument 'members' in GetSelection cannot be null.");
			}
		}

		private SingleSelection InitSingleImport()
		{
			_remoteApp?.InitProgressDialog();
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ModelImport);
			SingleSelection selection = IdeaModel.GetSingleSelection();

			CheckNodesAndMembers(selection);

			return selection;
		}

		private Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> GetConnections(IEnumerable<IIdeaMember1D> members)
		{
			Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> connections =
				new Dictionary<IIdeaNode, HashSet<IIdeaMember1D>>(new IIdeaObjectComparer());

			foreach (IIdeaMember1D member in members)
			{
				foreach (IIdeaNode node in GetNodesOnMember(member))
				{
					if (!connections.TryGetValue(node, out HashSet<IIdeaMember1D> memberSet))
					{
						memberSet = new HashSet<IIdeaMember1D>();
						connections.Add(node, memberSet);
					}

					memberSet.Add(member);
				}
			}

			return connections;
		}

		/// <summary>
		/// Creates and fills in <see cref="ModelBIM"/> from given <paramref name="objects"/> and <paramref name="bimItems"/>.
		/// Loads from <see cref="IIdeaModel"/> are concatenated to <paramref name="objects"/>.
		/// </summary>
		private ModelBIM CreateModelBIM(IEnumerable<IIdeaObject> objects, IEnumerable<IBimItem> bimItems, IdeaRS.OpenModel.CountryCode countryCode)
		{
			Debug.Assert(objects != null);
			Debug.Assert(bimItems != null);

			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.FinishingImport);

			ModelBIM modelBIM = _bimObjectImporter.Import(objects.Concat(IdeaModel.GetLoads()), bimItems, _project, countryCode);
			modelBIM.Model.OriginSettings = IdeaModel.GetOriginSettings();

			return modelBIM;
		}

		private static IEnumerable<IIdeaNode> GetNodesOnMember(IIdeaMember1D member)
		{
			foreach (IIdeaSegment3D segment in member.Elements.Select(x => x.Segment))
			{
				yield return segment.StartNode;

				if (segment is IIdeaArcSegment3D arcSegment)
				{
					yield return arcSegment.ArcPoint;
				}

				yield return segment.EndNode;
			}
		}
	}
}
