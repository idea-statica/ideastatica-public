﻿using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.ImportedObjects;
using IdeaStatiCa.Plugin;
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
		private readonly IImporter<ConnectionPoint> _connectionImporter;
		private readonly IImporter<IIdeaLoadCase> _loadCaseImporter;
		private readonly IImporter<IIdeaLoadGroup> _loadGroupImporter;
		private readonly IImporter<IIdeaCombiInput> _combiInputImporter;
		private readonly IImporter<IIdeaTaper> _taperImporter;
		private readonly IImporter<IIdeaSpan> _spanImporter;

		public ObjectImporter(IPluginLogger logger)
		{
			_nodeImporter = new NodeImporter(logger);
			_materialImporter = new MaterialImporter(logger);
			_crossSectionImporter = new CrossSectionImporter(logger);
			_segmentImporter = new SegmentImporter(logger);
			_elementImporter = new ElementImporter(logger);
			_memberImporter = new MemberImporter(logger);
			_loadCaseImporter = new LoadCaseImporter(logger);
			_loadGroupImporter = new LoadGroupImporter(logger);
			_combiInputImporter = new CombiInputImporter(logger);
			_connectionImporter = new ConnectionImporter(logger);
			_taperImporter = new TaperImporter(logger);
			_spanImporter = new SpanImporter(logger);
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

				case ConnectionPoint connection:
					return _connectionImporter.Import(ctx, connection);

				case IIdeaLoadCase loadCase:
					return _loadCaseImporter.Import(ctx, loadCase);

				case IIdeaLoadGroup loadGroup:
					return _loadGroupImporter.Import(ctx, loadGroup);

				case IIdeaCombiInput combiInput:
					return _combiInputImporter.Import(ctx, combiInput);

				case IIdeaTaper taper:
					return _taperImporter.Import(ctx, taper);

				case IIdeaSpan span:
					return _spanImporter.Import(ctx, span);
			}

			throw new ArgumentException($"Unsupported object type '{obj.GetType()}'");
		}
	}
}