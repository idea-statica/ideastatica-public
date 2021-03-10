using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
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
			CrossSectionImporter crossSectionImporter = new CrossSectionImporter();
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

		public OpenModelContainer ImportSelectedConnectionsToIom()
		{
			_ideaModel.GetSelection(out HashSet<IIdeaNode> nodes, out HashSet<IIdeaMember1D> members);

			foreach (IIdeaMember1D connectedMember in nodes.SelectMany(x => x.GetConnectedMembers()))
			{
				members.Add(connectedMember);
			}

			ImportContext importContext = new ImportContext();

			foreach (IIdeaMember1D member in members)
			{
				_memberConverter.Import(importContext, member);
			}

			return new OpenModelContainer()
			{
				OpenModel = importContext.OpenModel
			};
		}
	}
}