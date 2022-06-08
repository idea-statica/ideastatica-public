using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
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

		/// <summary>
		/// Creates instance of <see cref="BimImporter"/> with default <see cref="IGeometry"/> implementation.
		/// </summary>
		/// <param name="ideaModel">Model for importing</param>
		/// <param name="project"><see cref="IProject"/> instance for id mapping persistence</param>
		/// <param name="logger">Logger</param>
		/// <returns>Instance of <see cref="BimImporter"/></returns>
		/// <exception cref="ArgumentNullException">Throws when some argument is null.</exception>
		public static IBimImporter Create(IIdeaModel ideaModel, IProject project, IPluginLogger logger)
		{
			return Create(ideaModel, project, logger, new DefaultGeometryProvider(logger, ideaModel),
				new BimImporterConfiguration());
		}

		/// <summary>
		///Creates instance of <see cref="BimImporter"/>.
		/// </summary>
		/// <param name="ideaModel">Model for importing</param>
		/// <param name="project"><see cref="IProject"/> instance for id mapping persistence</param>
		/// <param name="geometry"><see cref="IGeometry"/> instance</param>
		/// <param name="logger">Logger</param>
		/// <param name="geometryProvider">Geometry provider</param>
		/// <returns>Instance of <see cref="BimImporter"/></returns>
		/// <exception cref="ArgumentNullException">Throws when some argument is null.</exception>
		public static IBimImporter Create(IIdeaModel ideaModel, IProject project, IPluginLogger logger,
			IGeometryProvider geometryProvider, BimImporterConfiguration configuration)
		{
			return new BimImporter(ideaModel,
				project,
				logger,
				geometryProvider,
				BimObjectImporter.Create(logger, configuration));
		}

		internal BimImporter(IIdeaModel ideaModel, IProject project, IPluginLogger logger, IGeometryProvider geometryProvider,
			IBimObjectImporter bimObjectImporter)
		{
			_ideaModel = ideaModel ?? throw new ArgumentNullException(nameof(ideaModel));
			_project = project ?? throw new ArgumentNullException(nameof(project));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_geometryProvider = geometryProvider ?? throw new ArgumentNullException(nameof(geometryProvider));
			_bimObjectImporter = bimObjectImporter ?? throw new ArgumentNullException(nameof(bimObjectImporter));
		}

		/// <inheritdoc cref="IBimImporter.ImportConnections"/>
		/// <remarks>Nodes are marked as a connection by following rules:<br/>
		///  - nodes specified in <see cref="IIdeaModel.GetSelection"/> are connections,<br/>
		///  - nodes with two or more connecting member are connections.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSelection"/> returns null out arguments.</exception>
		public ModelBIM ImportConnections()
		{
			InitImport(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers);
			IGeometry geometry = _geometryProvider.GetGeometry();

			List<Connection> connections = new List<Connection>();
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

			return CreateModelBIM(objects, connections);
		}

		/// <inheritdoc cref="IBimImporter.ImportMember"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSelection"/> returns null out arguments.</exception>
		public ModelBIM ImportMembers()
		{
			InitImport(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers);
			IGeometry geometry = _geometryProvider.GetGeometry();

			List<IBimItem> bimItems = new List<IBimItem>();
			HashSet<IIdeaNode> adjacentNodes = new HashSet<IIdeaNode>(_ideaObjectComparer);

			foreach (IIdeaMember1D selectedMember in selectedMembers)
			{
				bimItems.Add(new Member(selectedMember));

				foreach (IIdeaNode node in geometry.GetNodesOnMember(selectedMember))
				{
					adjacentNodes.Add(node);
				}
			}

			foreach (IIdeaNode node in adjacentNodes)
			{
				bimItems.Add(Connection.FromNodeAndMembers(node, new HashSet<IIdeaMember1D>(geometry.GetConnectedMembers(node))));
			}

			IEnumerable<IIdeaObject> objects = selectedNodes
				.Cast<IIdeaObject>()
				.Concat(selectedMembers);

			return CreateModelBIM(objects, bimItems);
		}

		/// <inheritdoc cref="IBimImporter.ImportSelected"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSelection"/> returns null out arguments.</exception>
		public List<ModelBIM> ImportSelected(List<BIMItemsGroup> selected)
		{
			if (selected is null)
			{
				throw new ArgumentNullException(nameof(selected));
			}

			_logger.LogTrace($"Importing of '{selected.Count}' selected items.");

			return selected.Select(x => ImportGroup(x)).ToList();
		}

		/// <inheritdoc cref="IBimImporter.Import"/>
		/// <exception cref="ArgumentNullException">Throws when argument is null.</exception>
		public ModelBIM Import(IEnumerable<IIdeaObject> objects)
		{
			if (objects is null)
			{
				throw new ArgumentNullException(nameof(objects));
			}

			return CreateModelBIM(objects, Enumerable.Empty<IBimItem>());
		}

		/// <summary>
		/// Imports items from <paramref name="group"/> into <see cref="ModelBIM"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException">If any argument is null.</exception>
		/// <exception cref="NotImplementedException">If the group type is not a connection or a substructure.</exception>
		private ModelBIM ImportGroup(BIMItemsGroup group)
		{
			if (group is null)
			{
				throw new ArgumentNullException(nameof(group));
			}

			_logger.LogTrace($"Importing of bim items group, id '{group.Id}', type '{group.Type}', items count '{group.Items}'.");

			if (group.Type == RequestedItemsType.Connections)
			{
				IGeometry geometry = _geometryProvider.GetGeometry();

				List<IIdeaObject> objects = group.Items
					.Select(x => _project.GetBimObject(x.Id))
					.ToList();

				IIdeaNode node = objects
					.OfType<IIdeaNode>()
					.First();

				IEnumerable<IIdeaMember1D> members = objects.OfType<IIdeaMember1D>();
				KeyValuePair<IIdeaNode, HashSet<IIdeaMember1D>> connection = GetConnections(members, geometry)
					.First(x => x.Key.Id == node.Id);

				return CreateModelBIM(objects, new IBimItem[]
				{
					Connection.FromNodeAndMembers(node, connection.Value)
				});
			}
			else if (group.Type == RequestedItemsType.Substructure)
			{
				IEnumerable<Member> bimItems = group.Items
					.Where(x => x.Type == BIMItemType.Member)
					.Select(x => _project.GetBimObject(x.Id))
					.Cast<IIdeaMember1D>()
					.Select(x => new Member(x));

				return CreateModelBIM(Enumerable.Empty<IIdeaObject>(), bimItems);
			}
			else
			{
				_logger.LogError($"BIMItemsGroup type '{group.Type}' is not supported.");
				throw new NotImplementedException($"BIMItemsGroup type '{group.Type}' is not supported.");
			}
		}

		private void InitImport(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members)
		{
			_ideaModel.GetSelection(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers);

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
		}

		private Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> GetConnections(IEnumerable<IIdeaMember1D> members, IGeometry geometry)
		{
			Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> connections =
				new Dictionary<IIdeaNode, HashSet<IIdeaMember1D>>(new IIdeaObjectComparer());

			foreach (IIdeaMember1D member in members)
			{
				foreach (IIdeaNode node in geometry.GetNodesOnMember(member))
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
		private ModelBIM CreateModelBIM(IEnumerable<IIdeaObject> objects, IEnumerable<IBimItem> bimItems)
		{
			Debug.Assert(objects != null);
			Debug.Assert(bimItems != null);

			ModelBIM modelBIM = _bimObjectImporter.Import(objects.Concat(_ideaModel.GetLoads()), bimItems, _project);
			modelBIM.Model.OriginSettings = _ideaModel.GetOriginSettings();
			return modelBIM;
		}
	}
}