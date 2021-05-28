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
		private readonly IGeometry _geometry;
		private readonly IResultImporter _resultImporter;

		/// <summary>
		/// Creates instance of <see cref="BimImporter"/> with default <see cref="IGeometry"/> implementation.
		/// </summary>
		/// <param name="ideaModel">Model for importing</param>
		/// <param name="project"><see cref="IProject"/> instace for id mapping persistency</param>
		/// <param name="logger">Logger</param>
		/// <returns>Instance of <see cref="BimImporter"/></returns>
		/// <exception cref="ArgumentNullException">Throws when some argument is null.</exception>
		public static IBimImporter Create(IIdeaModel ideaModel, IProject project, IPluginLogger logger)
		{
			return Create(ideaModel, project, new Geometry(logger), logger);
		}

		/// <summary>
		///Creates instance of <see cref="BimImporter"/>.
		/// </summary>
		/// <param name="ideaModel">Model for importing</param>
		/// <param name="project"><see cref="IProject"/> instace for id mapping persistency</param>
		/// <param name="geometry"><see cref="IGeometry"/> instance</param>
		/// <param name="logger">Logger</param>
		/// <returns>Instance of <see cref="BimImporter"/></returns>
		/// <exception cref="ArgumentNullException">Throws when some argument is null.</exception>
		public static IBimImporter Create(IIdeaModel ideaModel, IProject project, IGeometry geometry, IPluginLogger logger)
		{

			return new BimImporter(ideaModel,
				project,
				new ObjectImporter(logger),
				geometry,
				logger,
				new ResultImporter(logger));
		}

		internal BimImporter(IIdeaModel ideaModel, IProject project, IImporter<IIdeaObject> importer, IGeometry geometry
			, IPluginLogger logger, IResultImporter resultImporter)
		{
			_ideaModel = ideaModel ?? throw new ArgumentNullException(nameof(ideaModel));
			_project = project ?? throw new ArgumentNullException(nameof(project));
			_importer = importer ?? throw new ArgumentNullException(nameof(importer));
			_geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_resultImporter = resultImporter ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc cref="IBimImporter.ImportConnections"/>
		/// <exception cref="InvalidOperationException">Throws if <see cref="IIdeaModel.GetSelection"/> returns null out arguments.</exception>
		public ModelBIM ImportConnections()
		{
			InitImport(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers);

			selectedMembers.UnionWith(selectedNodes.SelectMany(x => _geometry.GetConnectedMembers(x)));

			ImportContext importContext = new ImportContext(_importer, _resultImporter, _project, _logger);
			List<BIMItemId> bimItems = new List<BIMItemId>();

			foreach (KeyValuePair<IIdeaNode, HashSet<IIdeaMember1D>> keyValue in GetConnections(selectedMembers))
			{
				if (keyValue.Value.Count < 2)
				{
					continue;
				}

				ImportConnection(importContext, bimItems, keyValue.Key, keyValue.Value);
			}

			importContext.OpenModel.OriginSettings = _ideaModel.GetOriginSettings();
			ISet<IIdeaLoadCase> lcs = _ideaModel.ImportLoadCases();
			foreach (var lc in lcs)
			{
				ReferenceElement refConnection = importContext.Import(lc);
			}
			ISet<IIdeaCombiInput> com = _ideaModel.ImportCombiInput();
			foreach (var cm in com)
			{
				ReferenceElement refConnection = importContext.Import(cm);
			}
			//_ideaModel.ImportLoadCases();
			//_ideaModel.ImportResults();
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
			InitImport(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers);

			ImportContext importContext = new ImportContext(_importer, _resultImporter, _project, _logger);
			List<BIMItemId> bimItems = new List<BIMItemId>();

			foreach (IIdeaMember1D selectedMember in selectedMembers)
			{
				ReferenceElement refMember = importContext.Import(selectedMember);

				bimItems.Add(new BIMItemId()
				{
					Id = refMember.Id,
					Type = BIMItemType.Member
				});

				foreach (IIdeaNode node in _geometry.GetNodesOnMember(selectedMember))
				{
					ImportConnection(importContext, bimItems, node, _geometry.GetConnectedMembers(node).ToHashSet());
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

		private ModelBIM ImportGroup(BIMItemsGroup group)
		{
			ImportContext importContext = new ImportContext(_importer, _resultImporter, _project, _logger);

			foreach (BIMItemId item in group.Items)
			{
				importContext.Import(_project.GetBimObject(item.Id));
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
			_geometry.Build(_ideaModel);
		}

		private Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> GetConnections(IEnumerable<IIdeaMember1D> members)
		{
			Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> connections =
				new Dictionary<IIdeaNode, HashSet<IIdeaMember1D>>(new IIdeaObjectComparer());

			foreach (IIdeaMember1D member in members)
			{
				foreach (IIdeaNode node in _geometry.GetNodesOnMember(member))
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
