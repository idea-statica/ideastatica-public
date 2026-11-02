using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
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
		private readonly IImporter<IIdeaConnectionPoint> _connectionImporter;
		private readonly IImporter<IIdeaLoadCase> _loadCaseImporter;
		private readonly IImporter<IIdeaLoadGroup> _loadGroupImporter;
		private readonly IImporter<IIdeaCombiInput> _combiInputImporter;
		private readonly IImporter<IIdeaTaper> _taperImporter;
		private readonly IImporter<IIdeaSpan> _spanImporter;

		private readonly IImporter<IIdeaConnectedMember> _connectedMemberImporter;
		private readonly IImporter<IIdeaPlate> _plateImporter;
		private readonly IImporter<IIdeaConnectedMember> _beamImporter;
		private readonly IImporter<IIdeaMember1D> _beamInConnectedPartsImporter;
		private readonly IImporter<IIdeaWeld> _weldImporter;
		private readonly IImporter<IIdeaBoltGrid> _boltGridImporter;
		private readonly IImporter<IIdeaAnchorGrid> _anchorGridImporter;
		private readonly IImporter<IIdeaPinGrid> _pinGridImporter;
		private readonly IImporter<IIdeaCut> _cutImporter;
		private readonly IImporter<IIdeaConcreteBlock> _concreteBlockImporter;
		private readonly IImporter<IIdeaFoldedPlate> _foldedPlateImporter;

		private readonly IImporter<IIdeaMember2D> _member2DImporter;
		private readonly IImporter<IIdeaElement2D> _element2DImporter;
		private readonly IImporter<IIdeaPolyLine3D> _polyLine3DImporter;
		private readonly IImporter<IIdeaRegion3D> _region3DImporter;
		private readonly IImporter<IIdeaLoadOnSurface> _loadOnSurfaceImporter;

		private readonly IImporter<IIdeaBoltAssembly> _boltAssemblyImporter;
		private readonly IImporter<IIdeaPin> _pinImporter;


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
			_connectedMemberImporter = new ConnectedMemberImporter(logger);
			_plateImporter = new PlateImporter(logger);
			_beamImporter = new BeamImporter(logger);
			_beamInConnectedPartsImporter = new BeamInConnectedPartsImporter(logger);
			_boltGridImporter = new BoltGridImporter(logger);
			_weldImporter = new WeldImporter(logger);
			_cutImporter = new CutImporter(logger);
			_concreteBlockImporter = new ConcreteBlockImporter(logger);
			_anchorGridImporter = new AnchorGridImporter(logger);
			_foldedPlateImporter = new FoldedPlateImporter(logger);
			_member2DImporter = new Member2DImporter(logger);
			_element2DImporter = new Element2DImporter(logger);
			_polyLine3DImporter = new PolyLine3DImporter(logger);
			_region3DImporter = new Region3DImporter(logger);
			_boltAssemblyImporter = new BoltAssemblyImporter(logger);
			_pinGridImporter = new PinGridImporter(logger);
			_pinImporter = new PinImporter(logger);
			_loadOnSurfaceImporter = new LoadOnSurfaceImporter(logger);
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

				case IIdeaConnectedMember connectedMember:
					return _connectedMemberImporter.Import(ctx, connectedMember);

				case IIdeaMember1D member:
					return _memberImporter.Import(ctx, member);

				case IIdeaConnectionPoint connection:
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

				case IIdeaMember2D member2D:
					return _member2DImporter.Import(ctx, member2D);

				case IIdeaElement2D element2D:
					return _element2DImporter.Import(ctx, element2D);

				case IIdeaPolyLine3D polyLine3D:
					return _polyLine3DImporter.Import(ctx, polyLine3D);

				case IIdeaRegion3D region3D:
					return _region3DImporter.Import(ctx, region3D);

				case IIdeaBoltAssembly boltAssembly:
					return _boltAssemblyImporter.Import(ctx, boltAssembly);
				
				case IIdeaPin pin:
					return _pinImporter.Import(ctx, pin);

				case IIdeaLoadOnSurface loadOnSurface:
					return _loadOnSurfaceImporter.Import(ctx, loadOnSurface);
			}

			throw new ArgumentException($"Unsupported object type '{obj.GetType()}'");
		}

		public object Import(IImportContext ctx, IIdeaObject obj, ConnectionData connectionData)
		{
			switch (obj)
			{

				case IIdeaNegativePlate nPlate:
					return _plateImporter.Import(ctx, nPlate, connectionData);
				case IIdeaPlate plate:
					return _plateImporter.Import(ctx, plate, connectionData);
				case IIdeaFoldedPlate foldedPlate:
					return _foldedPlateImporter.Import(ctx, foldedPlate, connectionData);
				case IIdeaConnectedMember member:
					return _beamImporter.Import(ctx, member, connectionData);
				case IIdeaAnchorGrid anchorGrid:
					return _anchorGridImporter.Import(ctx, anchorGrid, connectionData);
				case IIdeaPinGrid pinGrid:
					return _pinGridImporter.Import(ctx, pinGrid, connectionData);
				case IIdeaBoltGrid boltGrid:
					return _boltGridImporter.Import(ctx, boltGrid, connectionData);
				case IIdeaConcreteBlock concreteBlock:
					return _concreteBlockImporter.Import(ctx, concreteBlock, connectionData);
				case IIdeaWeld weld:
					return _weldImporter.Import(ctx, weld, connectionData);
				case IIdeaCut cut:
					return _cutImporter.Import(ctx, cut, connectionData);
				case IIdeaMember1D member:
					return _beamInConnectedPartsImporter.Import(ctx, member, connectionData);

			}

			throw new ArgumentException($"Unsupported object type '{obj.GetType()}'");
		}
	}
}