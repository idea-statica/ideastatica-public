using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Geometry;
using IdeaStatiCa.RamToIdea.Model;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberHorizontalBrace : AbstractRamMember
	{
		public override int UID => _brace.lUID;

		public override MemberType MemberType => MemberType.HorizontalBrace;

		protected override RamMemberProperties Properties { get; }

		private readonly IHorizBrace _brace;

		public RamMemberHorizontalBrace(IObjectFactory objectFactory, ISectionFactory sectionProvider, IResultsFactory resultsFactory, IGeometry geometry,
			ISegmentFactory segmentFactory, IHorizBrace brace)
			: base(objectFactory, sectionProvider, resultsFactory, geometry, segmentFactory)
		{
			_brace = brace;

			Properties = new RamMemberProperties()
			{
				Label = _brace.lLabel,
				MaterialType = _brace.eMaterial,
				MaterialUID = _brace.lMaterialID,
				MemberUID = _brace.lUID,
				Rotation = 0,
				SectionID = _brace.lSectionID,
				SectionLabel = _brace.strSectionLabel,
				Story = _brace.lStoryID,
				CanBeSubdivided = false,
			};

			Init();
		}

		public override IEnumerable<IIdeaResult> GetResults()
		{
			return ResultsFactory.GetResultsForHorizontalBrace(_brace, this);
		}

		protected override (SCoordinate, SCoordinate) GetStartEndCoordinates()
		{
			SCoordinate start = new SCoordinate();
			SCoordinate end = new SCoordinate();

			_brace.GetEndCoordinates(ref start, ref end);

			return (start, end);
		}
	}
}