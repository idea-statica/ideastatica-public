using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
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

		private readonly IPluginLogger _logger;
		private readonly IIdeaModel _ideaModel;
		private readonly IProject _project;
		private readonly IGeometryProvider _geometryProvider;
		private readonly IBimObjectImporter _bimObjectImporter;
		private readonly IProgressMessaging _remoteApp;

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
			IGeometryProvider geometryProvider, BimImporterConfiguration configuration)
		{
			return Create(ideaModel, project, logger, geometryProvider, configuration, null, null);
		}

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
				BimObjectImporter.Create(logger, configuration, resultsProvider, remoteApp),
				remoteApp);
		}

		internal BimImporter(
			IIdeaModel ideaModel,
			IProject project,
			IPluginLogger logger,
			IGeometryProvider geometryProvider,
			IBimObjectImporter bimObjectImporter,
			IProgressMessaging remoteApp = null)
		{
			_ideaModel = ideaModel ?? throw new ArgumentNullException(nameof(ideaModel));
			_project = project ?? throw new ArgumentNullException(nameof(project));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_geometryProvider = geometryProvider ?? throw new ArgumentNullException(nameof(geometryProvider));
			_bimObjectImporter = bimObjectImporter ?? throw new ArgumentNullException(nameof(bimObjectImporter));
			_remoteApp = remoteApp;
		}

		/// <inheritdoc cref="IBimImporter.ImportConnections"/>
		/// <remarks>Nodes are marked as a connection by following rules:<br/>
		///  - nodes specified in <see cref="IIdeaModel.GetSelection"/> are connections,<br/>
		///  - nodes with two or more connecting member are connections.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSelection"/> returns null out arguments.</exception>
		public ModelBIM ImportConnections(CountryCode countryCode)
		{
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ImportingConnections);

			InitImport(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers, out ISet<IIdeaConnectionPoint> connectionPoints);
			IGeometry geometry = _geometryProvider.GetGeometry();

			List<Connection> connections = new List<Connection>();

			if (connectionPoints != null)
			{
				foreach (var connectionPoint in connectionPoints)
				{
					connections.Add(Connection.FromConnectionPoint(connectionPoint));
				}
			}

			foreach (KeyValuePair<IIdeaNode, HashSet<IIdeaMember1D>> keyValue in GetConnections(selectedMembers, geometry))
			{
				if (selectedNodes.Contains(keyValue.Key) || keyValue.Value.Count >= 2)
				{
					connections.Add(Connection.FromNodeAndMembers(keyValue.Key, keyValue.Value));
				}
			}

			IEnumerable<IIdeaObject> objects = selectedNodes
				.Cast<IIdeaObject>()
				.Concat(selectedMembers);

			return CreateModelBIM(objects, connections, countryCode);
		}

		/// <inheritdoc cref="IBimImporter.ImportSingleConnection"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSelection"/> returns null out arguments.</exception>
		public ModelBIM ImportSingleConnection(CountryCode countryCode)
		{
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ImportingConnections);
			InitImport(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers, out IIdeaConnectionPoint connectionPoint);
			IGeometry geometry = _geometryProvider.GetGeometry();

			List<Connection> connections = new List<Connection>();

			if (connectionPoint != null)
			{
				connections.Add(Connection.FromConnectionPoint(connectionPoint));
			}

			IEnumerable<IIdeaObject> objects = selectedNodes
				.Cast<IIdeaObject>()
				.Concat(selectedMembers);

			return CreateModelBIM(objects, connections, countryCode);
		}

		/// <inheritdoc cref="IBimImporter.ImportMember"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSelection"/> returns null out arguments.</exception>
		public ModelBIM ImportMembers(CountryCode countryCode)
		{
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ImportingMembers);
			InitImport(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers, out ISet<IIdeaConnectionPoint> connectionPoints);
			IGeometry geometry = _geometryProvider.GetGeometry();

			List<IBimItem> bimItems = new List<IBimItem>();
			var adjacentNodes = new HashSet<IIdeaNode>(_ideaObjectComparer);
			if (connectionPoints != null)
			{
				foreach (var connectionPoint in connectionPoints)
				{
					bimItems.Add(Connection.FromConnectionPoint(connectionPoint));
				}
			}

			int j = 1;
			foreach (IIdeaMember1D selectedMember in selectedMembers)
			{
				_remoteApp?.SetStageLocalised(j, selectedMembers.Count, LocalisedMessage.Member);
				bimItems.Add(new Member(selectedMember));

				foreach (IIdeaNode node in geometry.GetNodesOnMember(selectedMember))
				{
					adjacentNodes.Add(node);
				}
				j++;
			}

			foreach (var node in adjacentNodes)
			{
				bimItems.Add(Connection.FromNodeAndMembers(node, new HashSet<IIdeaMember1D>(geometry.GetConnectedMembers(node))));
			}

			IEnumerable<IIdeaObject> objects = selectedNodes
				.Cast<IIdeaObject>()
				.Concat(selectedMembers);

			return CreateModelBIM(objects, bimItems, countryCode);
		}

		/// <inheritdoc cref="IBimImporter.ImportSelected"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSelection"/> returns null out arguments.</exception>
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
				IGeometry geometry = _geometryProvider.GetGeometry();

				List<IIdeaObject> objects = group.Items
					.Select(x => _project.GetBimObject(x.Id))
					.ToList();

				IIdeaNode node = objects
					.OfType<IIdeaNode>()
					.First();

				IEnumerable<IIdeaConnectionPoint> connectionPoints = objects.OfType<IIdeaConnectionPoint>();
				List<Connection> connections = new List<Connection>();

				if (connectionPoints.Any())
				{
					var cp = connectionPoints.FirstOrDefault();
					if (cp != null)
					{
						connections.Add(Connection.FromConnectionPoint(cp));
						//process connection
						if (_ideaModel is IIdeaConnectionModel connectionModel)
						{
							connectionModel.ProcessConnection(cp);
						}
					}
				}
				else
				{
					IEnumerable<IIdeaMember1D> members = objects.OfType<IIdeaMember1D>();
					var con = GetConnections(members, geometry)
						.First(x => x.Key.Id == node.Id);

					connections.Add(Connection.FromNodeAndMembers(node, con.Value));
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

		private void InitImport(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out ISet<IIdeaConnectionPoint> connectionPoints)
		{
			_remoteApp?.InitProgressDialog();
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ModelImport);
			_ideaModel.GetSelection(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers, out ISet<IIdeaConnectionPoint> selectedConnectionPoints);

			if (selectedNodes == null)
			{
				throw new InvalidOperationException("Out argument 'nodes' in GetSelection cannot be null.");
			}

			if (selectedMembers == null)
			{
				throw new InvalidOperationException("Out argument 'members' in GetSelection cannot be null.");
			}

			nodes = selectedNodes;
			members = selectedMembers;
			connectionPoints = selectedConnectionPoints;
		}

		private void InitImport(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out IIdeaConnectionPoint connectionPoint)
		{
			_ideaModel.GetSelection(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers, out IIdeaConnectionPoint selectedConnectionPoint);

			if (selectedNodes == null)
			{
				throw new InvalidOperationException("Out argument 'nodes' in GetSelection cannot be null.");
			}

			if (selectedMembers == null)
			{
				throw new InvalidOperationException("Out argument 'members' in GetSelection cannot be null.");
			}

			nodes = selectedNodes;
			members = selectedMembers;
			connectionPoint = selectedConnectionPoint;
		}

		private Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> GetConnections(IEnumerable<IIdeaMember1D> members, IGeometry geometry)
		{
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ModelImport);

			Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> connections =
				new Dictionary<IIdeaNode, HashSet<IIdeaMember1D>>(new IIdeaObjectComparer());

			foreach (IIdeaMember1D member in members)
			{
				var nodes = geometry.GetNodesOnMember(member).ToList();
				for (int i = 0; i < nodes.Count; i++)
				{
					IIdeaNode node = nodes[i];
					_remoteApp?.SetStageLocalised(i + 1, nodes.Count, LocalisedMessage.ImportingConnections); //  "Creating Connection"

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

			ModelBIM modelBIM = _bimObjectImporter.Import(objects.Concat(_ideaModel.GetLoads()), bimItems, _project, countryCode);
			modelBIM.Model.OriginSettings = _ideaModel.GetOriginSettings();

			return modelBIM;
		}
	}
}