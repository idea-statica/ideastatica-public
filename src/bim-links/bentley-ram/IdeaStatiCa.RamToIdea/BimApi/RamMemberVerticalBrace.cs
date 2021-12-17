using IdeaStatiCa.RamToIdea.Factories;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberVerticalBrace : AbstractRamMember
	{
		public override int UID => _brace.lUID;

		public override MemberType MemberType => MemberType.VerticalBrace;

		protected override int Label => _brace.lLabel;

		private readonly IVerticalBrace _brace;

		public RamMemberVerticalBrace(IObjectFactory objectFactory, INodes nodes, IVerticalBrace brace)
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