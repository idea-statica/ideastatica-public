using IdeaStatiCa.RamToIdea.Factories;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberHorizontalBrace : AbstractRamMember
	{
		public override int UID => _brace.lUID;

		public override MemberType MemberType => MemberType.HorizontalBrace;

		protected override int Label => _brace.lLabel;

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