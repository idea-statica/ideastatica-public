using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Geometry;
using IdeaStatiCa.RamToIdea.Model;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberBeam : AbstractRamMember
	{
		public override int UID => _beam.lUID;

		public override MemberType MemberType => MemberType.Beam;

		protected override RamMemberProperties Properties { get; }

		private readonly IGeometry _geometry;
		private readonly IBeam _beam;

		public RamMemberBeam(IObjectFactory objectFactory, ISectionFactory sectionProvider, IResultsFactory resultsFactory, IGeometry geometry,
			ISegmentFactory segmentFactory, IBeam beam)
			: base(objectFactory, sectionProvider, resultsFactory, geometry, segmentFactory)
		{
			_geometry = geometry;
			_beam = beam;

			Properties = new RamMemberProperties()
			{
				Label = _beam.lLabel,
				MaterialType = _beam.eMaterial,
				MaterialUID = _beam.lMaterialID,
				MemberUID = _beam.lUID,
				Rotation = 0,
				SectionID = _beam.lSectionID,
				SectionLabel = _beam.strSectionLabel,
				Story = _beam.lStoryID,
				CanBeSubdivided = true,
			};

			Init();
		}

		public override IEnumerable<IIdeaResult> GetResults()
		{
			return ResultsFactory.GetResultsForBeam(_beam, this);
		}

		protected override (SCoordinate, SCoordinate) GetStartEndCoordinates()
		{
			SCoordinate start = new SCoordinate();
			SCoordinate end = new SCoordinate();

			_beam.GetCoordinates(EBeamCoordLoc.eBeamEnds, ref start, ref end);

			return (start, end);
		}

		protected override Line CreateLine()
		{
			Line line = base.CreateLine();

			SCoordinate start = new SCoordinate();
			SCoordinate end = new SCoordinate();
			_beam.GetCoordinates(EBeamCoordLoc.eBeamSupports, ref start, ref end);

			if (_beam.eFramingType == EFRAMETYPE.MemberIsLateral)
			{
				_geometry.AddNodeToLine(line, start);
				_geometry.AddNodeToLine(line, end);
			}

			return line;
		}
	}
}