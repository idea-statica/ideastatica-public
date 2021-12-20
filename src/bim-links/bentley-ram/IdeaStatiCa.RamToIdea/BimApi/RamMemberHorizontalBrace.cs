using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Model;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberHorizontalBrace : AbstractRamMember
	{
		public override int UID => _brace.lUID;

		public override MemberType MemberType => MemberType.HorizontalBrace;

		protected override RamMemberProperties Properties { get; }

		private readonly IHorizBrace _brace;

		public RamMemberHorizontalBrace(IObjectFactory objectFactory, IRamSectionProvider sectionProvider, INodes nodes, IHorizBrace brace)
			: base(objectFactory, sectionProvider, nodes)
		{
			_brace = brace;

			Properties = new RamMemberProperties()
			{
				Label = _brace.strSectionLabel,
				MaterialType = _brace.eMaterial,
				MaterialUID = _brace.lMaterialID,
				MemberUID = _brace.lUID,
				Rotation = 0,
				SectionID = _brace.lSectionID,
				SectionLabel = _brace.strSectionLabel
			};
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