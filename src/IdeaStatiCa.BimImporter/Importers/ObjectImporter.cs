using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.ImportedObjects;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class ObjectImporter : IImporter<IIdeaObject>
	{
		private readonly IImporter<IIdeaNode> _nodeImporter;
		private readonly IImporter<IIdeaMaterial> _materialImporter;
		private readonly IImporter<IIdeaCrossSection> _crossSectionImporter;
		private readonly IImporter<IIdeaSegment3D> _segmentImporter;
		private readonly IImporter<IIdeaElement1D> _elementImporter;
		private readonly IImporter<IIdeaMember1D> _memberImporter;
		private readonly IImporter<Connection> _connectionImporter;
		private readonly IImporter<IIdeaLoadCase> _loadCaseImporter;
		private readonly IImporter<IIdeaLoadGroup> _loadGroupImporter;

		public ObjectImporter(
			IImporter<IIdeaNode> nodeImporter,
			IImporter<IIdeaMaterial> materialImporter,
			IImporter<IIdeaCrossSection> crossSectionImporter,
			IImporter<IIdeaSegment3D> segmentImporter,
			IImporter<IIdeaElement1D> elementImporter,
			IImporter<IIdeaMember1D> memberImporter,
			IImporter<IIdeaLoadCase> loadCaseImporter,
			IImporter<IIdeaLoadGroup> loadGroupImporter,
			IImporter<Connection> connectionImporter)
		{
			_nodeImporter = nodeImporter;
			_materialImporter = materialImporter;
			_crossSectionImporter = crossSectionImporter;
			_segmentImporter = segmentImporter;
			_elementImporter = elementImporter;
			_memberImporter = memberImporter;
			_loadCaseImporter = loadCaseImporter;
			_loadGroupImporter = loadGroupImporter;
			_connectionImporter = connectionImporter;
		}

		public OpenElementId Import(IImportContext ctx, IIdeaObject obj)
		{
			switch (obj)
			{
				case IIdeaNode node:
					return _nodeImporter.Import(ctx, node);

				case IIdeaMaterial material:
					return _materialImporter.Import(ctx, material);

				case IIdeaCrossSection css:
					return _crossSectionImporter.Import(ctx, css);

				case IIdeaSegment3D segment:
					return _segmentImporter.Import(ctx, segment);

				case IIdeaElement1D element:
					return _elementImporter.Import(ctx, element);

				case IIdeaMember1D member:
					return _memberImporter.Import(ctx, member);

				case Connection connection:
					return _connectionImporter.Import(ctx, connection);

				case IIdeaLoadCase loadCase:
					return _loadCaseImporter.Import(ctx, loadCase);

				case IIdeaLoadGroup loadGroup:
					return _loadGroupImporter.Import(ctx, loadGroup);
			}

			throw new ArgumentException($"Unsupported object type {obj.GetType()}");
		}
	}
}
