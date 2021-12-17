using IdeaStatiCa.RamToIdea.Factories;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberHorizontalBrace : AbstractRamMember
	{
		public override int UID => _brace.lUID;

		public override MemberType MemberType => MemberType.HorizontalBrace;

		protected override int Label => _brace.lLabel;

		protected override EMATERIALTYPES MaterialType => _brace.eMaterial;

		protected override int MaterialUID => _brace.lMaterialID;

		protected override string CrossSectionName => _brace.strSectionLabel;

		protected override int CrossSectionUID => _brace.lSectionID;

		private readonly IHorizBrace _brace;

		public RamMemberHorizontalBrace(IObjectFactory objectFactory, INodes nodes, IHorizBrace brace)
			: base(objectFactory, nodes)
		{
			_brace = brace;
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