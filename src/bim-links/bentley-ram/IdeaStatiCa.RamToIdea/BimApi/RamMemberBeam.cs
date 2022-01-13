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

		private readonly IBeam _beam;

		public RamMemberBeam(IObjectFactory objectFactory, ISectionFactory sectionProvider, IResultsFactory resultsFactory, IGeometry geometry,
			ISegmentFactory segmentFactory, IBeam beam)
			: base(objectFactory, sectionProvider, resultsFactory, geometry, segmentFactory)
		{
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
				Story = _beam.lStoryID
			};
		}

		public override IEnumerable<IIdeaResult> GetResults()
		{
			return ResultsFactory.GetBeamResults(_beam, this);
		}

		protected override (SCoordinate, SCoordinate) GetStartEndCoordinates()
		{
			SCoordinate start = new SCoordinate();
			SCoordinate end = new SCoordinate();

			_beam.GetCoordinates(EBeamCoordLoc.eBeamEnds, ref start, ref end);

			return (start, end);
		}
	}
}