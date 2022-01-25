using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Geometry;
using IdeaStatiCa.RamToIdea.Model;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberColumn : AbstractRamMember
	{
		public override int UID => _column.lUID;

		public override MemberType MemberType => MemberType.Column;

		protected override RamMemberProperties Properties { get; }

		private readonly IColumn _column;

		public RamMemberColumn(IObjectFactory objectFactory, ISectionFactory sectionProvider, IResultsFactory resultsFactory, IGeometry geometry,
			ISegmentFactory segmentFactory, IColumn column)
			: base(objectFactory, sectionProvider, resultsFactory, geometry, segmentFactory)
		{
			_column = column;

			Properties = new RamMemberProperties()
			{
				Label = _column.lLabel,
				MaterialType = _column.eMaterial,
				MaterialUID = _column.lMaterialID,
				MemberUID = _column.lUID,
				Rotation = _column.dOrientation,
				SectionID = _column.lSectionID,
				SectionLabel = _column.strSectionLabel,
				Story = _column.lStoryID,
				CanBeSubdivided = _column.eFramingType == EFRAMETYPE.MemberIsLateral,
			};

			Init();
		}

		public override IEnumerable<IIdeaResult> GetResults()
		{
			return ResultsFactory.GetResultsForColumn(_column);
		}

		protected override (SCoordinate, SCoordinate) GetStartEndCoordinates()
		{
			SCoordinate start = new SCoordinate();
			SCoordinate end = new SCoordinate();

			_column.GetEndCoordinates(ref start, ref end);

			return (start, end);
		}
	}
}