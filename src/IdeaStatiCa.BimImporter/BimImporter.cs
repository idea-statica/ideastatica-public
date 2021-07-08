using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.ImportedObjects;
using IdeaStatiCa.BimImporter.Importers;
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
		private readonly IImporter<IIdeaObject> _importer;
		private readonly IProject _project;
		private readonly IResultImporter _resultImporter;
		private readonly IGeometryProvider _geometryProvider;

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
				new ObjectImporter(logger),
				logger,
				new ResultImporter(logger),
				geometryProvider);
		}

		internal BimImporter(IIdeaModel ideaModel, IProject project, IImporter<IIdeaObject> importer,
			IPluginLogger logger, IResultImporter resultImporter, IGeometryProvider geometryProvider)
		{
			_ideaModel = ideaModel ?? throw new ArgumentNullException(nameof(ideaModel));
			_project = project ?? throw new ArgumentNullException(nameof(project));
			_importer = importer ?? throw new ArgumentNullException(nameof(importer));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_resultImporter = resultImporter ?? throw new ArgumentNullException(nameof(logger));
			_geometryProvider = geometryProvider ?? throw new ArgumentNullException(nameof(geometryProvider));
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

			ImportContext importContext = new ImportContext(_importer, _resultImporter, _project, _logger);
			List<BIMItemId> bimItems = new List<BIMItemId>();

			foreach (IIdeaLoading load in _ideaModel.GetLoads())
			{
				importContext.Import(load);
			}

			foreach (KeyValuePair<IIdeaNode, HashSet<IIdeaMember1D>> keyValue in GetConnections(selectedMembers, geometry))
			{
				if (selectedNodes.Contains(keyValue.Key) || keyValue.Value.Count >= 2)
				{
					ImportConnection(importContext, bimItems, keyValue.Key, keyValue.Value);
				}
			}

			importContext.OpenModel.OriginSettings = _ideaModel.GetOriginSettings();

			return new ModelBIM()
			{
				Items = bimItems,
				Messages = new IdeaRS.OpenModel.Message.OpenMessages(),
				Model = importContext.OpenModel,
				Project = "",
				Results = importContext.OpenModelResult
			};
		}

		/// <inheritdoc cref="IBimImporter.ImportMember"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSelection"/> returns null out arguments.</exception>
		public ModelBIM ImportMembers()
		{
			InitImport(out _, out ISet<IIdeaMember1D> selectedMembers);
			IGeometry geometry = _geometryProvider.GetGeometry();

			ImportContext importContext = new ImportContext(_importer, _resultImporter, _project, _logger);
			List<BIMItemId> bimItems = new List<BIMItemId>();

			foreach (IIdeaLoading load in _ideaModel.GetLoads())
			{
				importContext.Import(load);
			}

			foreach (IIdeaMember1D selectedMember in selectedMembers)
			{
				ReferenceElement refMember = importContext.Import(selectedMember);

				bimItems.Add(new BIMItemId()
				{
					Id = refMember.Id,
					Type = BIMItemType.Member
				});

				foreach (IIdeaNode node in geometry.GetNodesOnMember(selectedMember))
				{
					ImportConnection(importContext, bimItems, node, geometry.GetConnectedMembers(node).ToHashSet());
				}
			}

			importContext.OpenModel.OriginSettings = _ideaModel.GetOriginSettings();

			return new ModelBIM()
			{
				Items = bimItems,
				Messages = new IdeaRS.OpenModel.Message.OpenMessages(),
				Model = importContext.OpenModel,
				Project = "",
				Results = importContext.OpenModelResult
			};
		}

		public List<ModelBIM> ImportSelected(List<BIMItemsGroup> selected)
		{
			return selected.Select(x => ImportGroup(x)).ToList();
		}

		public ModelBIM Import(IEnumerable<IIdeaObject> objects)
		{
			if (objects is null)
			{
				throw new ArgumentNullException(nameof(objects));
			}

			ImportContext importContext = new ImportContext(_importer, _resultImporter, _project, _logger);

			foreach (IIdeaObject obj in objects)
			{
				importContext.Import(obj);
			}

			return new ModelBIM()
			{
				Items = new List<BIMItemId>(),
				Messages = new IdeaRS.OpenModel.Message.OpenMessages(),
				Model = importContext.OpenModel,
				Project = "",
				Results = importContext.OpenModelResult
			};
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

		private void ImportConnection(ImportContext importContext, List<BIMItemId> bimItems,
			IIdeaNode node, ISet<IIdeaMember1D> members)
		{
			Connection connection = new Connection(node, members);
			ReferenceElement refConnection = importContext.Import(connection);

			bimItems.Add(new BIMItemId()
			{
				Id = refConnection.Id,
				Type = BIMItemType.Node
			});
		}
	}
}