using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter
{
	/// <inheritdoc cref="IBimImporter"/>
	public class BimImporter : IBimImporter
	{
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
			return Create(ideaModel, project, logger, new DefaultGeometryProvider(logger, ideaModel));
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
		public static IBimImporter Create(IIdeaModel ideaModel, IProject project, IPluginLogger logger, IGeometryProvider geometryProvider)
		{
			return new BimImporter(ideaModel,
				project,
				logger,
				geometryProvider,
				BimObjectImporter.Create(logger));
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

			IEnumerable<IIdeaObject> objects = _ideaModel.GetLoads()
				.Concat<IIdeaObject>(selectedNodes)
				.Concat(selectedMembers);

			return CreateModelBIM(objects, connections);
		}

		/// <inheritdoc cref="IBimImporter.ImportMember"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSelection"/> returns null out arguments.</exception>
		public ModelBIM ImportMembers()
		{
			InitImport(out _, out ISet<IIdeaMember1D> selectedMembers);
			IGeometry geometry = _geometryProvider.GetGeometry();

			List<IBimItem> bimItems = new List<IBimItem>();

			foreach (IIdeaMember1D selectedMember in selectedMembers)
			{
				bimItems.Add(new Member(selectedMember));

				foreach (IIdeaNode node in geometry.GetNodesOnMember(selectedMember))
				{
					bimItems.Add(Connection.FromNodeAndMembers(node, geometry.GetConnectedMembers(node).ToHashSet()));
				}
			}

			IEnumerable<IIdeaObject> objects = _ideaModel.GetLoads()
				.Concat<IIdeaObject>(selectedMembers);

			return CreateModelBIM(objects, bimItems);
		}

		public List<ModelBIM> ImportSelected(List<BIMItemsGroup> selected)
		{
			if (selected is null)
			{
				throw new ArgumentNullException(nameof(selected));
			}

			return selected.Select(x => ImportGroup(x)).ToList();
		}

		public ModelBIM Import(IEnumerable<IIdeaObject> objects)
		{
			if (objects is null)
			{
				throw new ArgumentNullException(nameof(objects));
			}

			return CreateModelBIM(objects.Concat(_ideaModel.GetLoads()), Enumerable.Empty<IBimItem>());
		}

		private ModelBIM ImportGroup(BIMItemsGroup group)
		{
			if (group is null)
			{
				throw new ArgumentNullException(nameof(group));
			}

			return Import(group.Items
				.Select(x => _project.GetBimObject(x.Id))
				.Concat(_ideaModel.GetLoads()));
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

		private ModelBIM CreateModelBIM(IEnumerable<IIdeaObject> objects, IEnumerable<IBimItem> bimItems)
		{
			ModelBIM modelBIM = _bimObjectImporter.Convert(objects, bimItems, _project);
			modelBIM.Model.OriginSettings = _ideaModel.GetOriginSettings();
			return modelBIM;
		}
	}
}