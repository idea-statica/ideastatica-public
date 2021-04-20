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
    public class BimImporter : IBimImporter
    {
        private readonly IIdeaModel _ideaModel;
        private readonly IImporter<IIdeaObject> _importer;
        private readonly IProject _project;
        private readonly IGeometry _geometry;

        public static IBimImporter Create(IIdeaModel ideaModel, IProject project)
        {
            return Create(ideaModel, project, new Geometry());
        }

        public static IBimImporter Create(IIdeaModel ideaModel, IProject project, IGeometry geometry)
        {
            NodeImporter nodeImporter = new NodeImporter();
            MaterialImporter materialImporter = new MaterialImporter();
            CrossSectionImporter crossSectionImporter = new CrossSectionImporter();
            SegmentImporter segmentImporter = new SegmentImporter();
            ElementImporter elementImporter = new ElementImporter();
            MemberImporter memberImporter = new MemberImporter();
            ConnectionImporter connectionImporter = new ConnectionImporter();

            return new BimImporter(ideaModel, project, new ObjectImporter(
                nodeImporter,
                materialImporter,
                crossSectionImporter,
                segmentImporter,
                elementImporter,
                memberImporter,
                connectionImporter),
                geometry);
        }

        internal BimImporter(IIdeaModel ideaModel, IProject project, IImporter<IIdeaObject> importer, IGeometry geometry)
        {
            _ideaModel = ideaModel;
            _project = project;
            _importer = importer;
            _geometry = geometry;
        }

        public ModelBIM ImportConnections()
        {
            InitImport(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers);

            selectedMembers.UnionWith(selectedNodes.SelectMany(x => _geometry.GetConnectedMembers(x)));

            ImportContext importContext = new ImportContext(_importer, _project);
            List<BIMItemId> bimItems = new List<BIMItemId>();

            foreach (KeyValuePair<IIdeaNode, HashSet<IIdeaMember1D>> keyValue in GetConnections(_geometry, selectedMembers))
            {
                if (keyValue.Value.Count < 2)
                {
                    continue;
                }

                ImportConnection(importContext, bimItems, keyValue.Key, keyValue.Value);
            }

            importContext.OpenModel.OriginSettings = _ideaModel.GetOriginSettings();

            return new ModelBIM()
            {
                Items = bimItems,
                Messages = new IdeaRS.OpenModel.Message.OpenMessages(),
                Model = importContext.OpenModel,
                Project = "",
                Results = new IdeaRS.OpenModel.Result.OpenModelResult()
            };
        }

        public ModelBIM ImportMembers()
        {
            InitImport(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers);

            ImportContext importContext = new ImportContext(_importer, _project);
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
                Results = new IdeaRS.OpenModel.Result.OpenModelResult()
            };
        }

        public List<ModelBIM> ImportSelected(List<BIMItemsGroup> selected)
        {
            return selected.Select(x => ImportGroup(x)).ToList();
        }

        private ModelBIM ImportGroup(BIMItemsGroup group)
        {
            ImportContext importContext = new ImportContext(_importer, _project);

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
                Results = new IdeaRS.OpenModel.Result.OpenModelResult()
            };
        }

        private void InitImport(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members)
        {
            _ideaModel.GetSelection(out ISet<IIdeaNode> selectedNodes, out ISet<IIdeaMember1D> selectedMembers);

            if (selectedNodes == null)
            {
                throw new Exception("Out argument 'nodes' in GetSelection cannot be null.");
            }

            if (selectedMembers == null)
            {
                throw new Exception("Out argument 'members' in GetSelection cannot be null.");
            }

            nodes = selectedNodes;
            members = selectedMembers;
            _geometry.Build(_ideaModel);
        }

        private Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> GetConnections(IGeometry geometry,
            IEnumerable<IIdeaMember1D> members)
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