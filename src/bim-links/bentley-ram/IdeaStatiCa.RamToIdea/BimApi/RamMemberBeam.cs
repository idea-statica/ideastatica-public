using IdeaStatiCa.RamToIdea.Factories;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberBeam : AbstractRamMember
	{
		public override int UID => _beam.lUID;

		public override MemberType MemberType => MemberType.Beam;

		protected override int Label => _beam.lLabel;

		private readonly IBeam _beam;

		public RamMemberBeam(IObjectFactory objectFactory, INodes nodes, IBeam beam)
			: base(objectFactory, nodes)
		{
			_beam = beam;
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