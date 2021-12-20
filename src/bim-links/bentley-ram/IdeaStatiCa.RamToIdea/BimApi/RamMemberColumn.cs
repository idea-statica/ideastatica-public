using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Model;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberColumn : AbstractRamMember
	{
		public override int UID => _column.lUID;

		public override MemberType MemberType => MemberType.Column;

		protected override RamMemberProperties Properties { get; }

		private readonly IColumn _column;

		public RamMemberColumn(IObjectFactory objectFactory, IRamSectionProvider sectionProvider, INodes nodes, IColumn column)
			: base(objectFactory, sectionProvider, nodes)
		{
			_column = column;

			Properties = new RamMemberProperties()
			{
				Label = _column.strSectionLabel,
				MaterialType = _column.eMaterial,
				MaterialUID = _column.lMaterialID,
				MemberUID = _column.lUID,
				Rotation = _column.dOrientation,
				SectionID = _column.lSectionID,
				SectionLabel = _column.strSectionLabel
			};
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