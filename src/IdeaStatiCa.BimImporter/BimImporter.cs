using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
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

		private readonly IImporter<IIdeaMember1D> _memberConverter;

		public static IBimImporter Create(IIdeaModel ideaModel)
		{
			NodeImporter nodeImporter = new NodeImporter();
			MaterialImporter materialImporter = new MaterialImporter();
			CrossSectionImporter crossSectionImporter = new CrossSectionImporter(materialImporter);
			SegmentImporter segmentImporter = new SegmentImporter(nodeImporter);
			ElementImporter elementImporter = new ElementImporter(crossSectionImporter, segmentImporter);
			MemberImporter memberImporter = new MemberImporter(elementImporter);

			return new BimImporter(ideaModel, memberImporter);
		}

		internal BimImporter(IIdeaModel ideaModel, IImporter<IIdeaMember1D> memberImporter)
		{
			_ideaModel = ideaModel;
			_memberConverter = memberImporter;
		}

		public ModelBIM ImportSelectedConnectionsToIom()
		{
			ImportContext importContext = new ImportContext();

			_ideaModel.GetSelection(out HashSet<IIdeaNode> selectedNodes, out HashSet<IIdeaMember1D> selectedMembers);

			if (selectedNodes == null)
			{
				throw new Exception("Out argument 'nodes' in GetSelection cannot be null.");
			}

			if (selectedMembers == null)
			{
				throw new Exception("Out argument 'members' in GetSelection cannot be null.");
			}

			selectedMembers.UnionWith(selectedNodes.SelectMany(x => x.GetConnectedMembers()));

			foreach (IIdeaMember1D member in selectedMembers)
			{
				_memberConverter.Import(importContext, member);
			}

			List<BIMItemId> bimItems = new List<BIMItemId>();

			foreach (KeyValuePair<IIdeaNode, HashSet<IIdeaMember1D>> keyValue in GetConnections(selectedMembers))
			{
				if (keyValue.Value.Count < 2)
				{
					continue;
				}

				ConnectionPoint connectionPoint = CreateConnectionPoint(importContext, keyValue.Key, keyValue.Value);
				bimItems.Add(new BIMItemId()
				{
					Id = connectionPoint.Id,
					Type = BIMItemType.Node
				});
			}

			return new ModelBIM()
			{
				Items = bimItems,
				Messages = new IdeaRS.OpenModel.Message.OpenMessages(),
				Model = importContext.OpenModel,
				Project = "",
				RequestedItems = new RequestedItemsType(),
				Results = new IdeaRS.OpenModel.Result.OpenModelResult()
			};
		}

		private Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> GetConnections(HashSet<IIdeaMember1D> selectedMembers)
		{
			Dictionary<IIdeaNode, HashSet<IIdeaMember1D>> connections =
				new Dictionary<IIdeaNode, HashSet<IIdeaMember1D>>(new IIdeaObjectComparer());

			foreach (IIdeaMember1D member in selectedMembers)
			{
				foreach (IIdeaNode node in GetAllNodes(member))
				{
					HashSet<IIdeaMember1D> memberSet;
					if (!connections.TryGetValue(node, out memberSet))
					{
						memberSet = new HashSet<IIdeaMember1D>();
						connections.Add(node, memberSet);
					}

					memberSet.Add(member);
				}
			}

			return connections;
		}

		private IEnumerable<IIdeaNode> GetAllNodes(IIdeaMember1D member)
		{
			foreach (IIdeaElement1D element in member.Elements)
			{
				yield return element.Segment.StartNode;
				yield return element.Segment.EndNode;
			}
		}

		private ConnectionPoint CreateConnectionPoint(ImportContext ctx, IIdeaNode node, HashSet<IIdeaMember1D> members)
		{
			ConnectionPoint connectionPoint = new ConnectionPoint()
			{
				Name = node.Name,
				ConnectedMembers = new List<ConnectedMember>(),
				Node = ctx.ReferenceElements[node.Id]
			};

			ctx.Add(connectionPoint);

			foreach (IIdeaMember1D member in members)
			{
				ReferenceElement memberRef = ctx.ReferenceElements[member.Id];
				ConnectedMember connectedMember = new ConnectedMember()
				{
					Id = memberRef.Id,
					MemberId = memberRef
				};

				connectionPoint.ConnectedMembers.Add(connectedMember);
			}

			return connectionPoint;
		}
	}
}