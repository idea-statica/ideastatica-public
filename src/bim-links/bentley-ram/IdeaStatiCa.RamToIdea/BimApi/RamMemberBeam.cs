using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Model;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberBeam : AbstractRamMember
	{
		public override int UID => _beam.lUID;

		public override MemberType MemberType => MemberType.Beam;

		protected override RamMemberProperties Properties { get; }

		private readonly IBeam _beam;

		public RamMemberBeam(IObjectFactory objectFactory, ISectionFactory sectionProvider, INodes nodes, IBeam beam)
			: base(objectFactory, sectionProvider, nodes)
		{
			_beam = beam;

			Properties = new RamMemberProperties()
			{
				Label = _beam.strSectionLabel,
				MaterialType = _beam.eMaterial,
				MaterialUID = _beam.lMaterialID,
				MemberUID = _beam.lUID,
				Rotation = 0,
				SectionID = _beam.lSectionID,
				SectionLabel = _beam.strSectionLabel
			};
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