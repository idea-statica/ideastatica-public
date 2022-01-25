using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	internal class Member
	{
		public SCoordinate Start { get; }

		public SCoordinate End { get; }

		public MemberType Type { get; }

		public bool IsGravityMember { get; }

		public Line Line { get; set; }

		public IColumn Column { get; }

		public IBeam Beam { get; }

		public IVerticalBrace VerticalBrace { get; }

		public IHorizBrace HorizontalBrace { get; }

		private Member(MemberType type, EFRAMETYPE frameType)
		{
			Type = type;
			IsGravityMember = frameType == EFRAMETYPE.MemberIsGravity;
		}

		public Member(IColumn column)
			: this(MemberType.Column, column.eFramingType)
		{
			Column = column;
			SCoordinate start = new SCoordinate();
			SCoordinate end = new SCoordinate();
			column.GetEndCoordinates(ref start, ref end);
			Start = start;
			End = end;
		}

		public Member(IBeam beam)
			: this(MemberType.Beam, beam.eFramingType)
		{
			Beam = beam;
			SCoordinate start = new SCoordinate();
			SCoordinate end = new SCoordinate();
			beam.GetCoordinates(EBeamCoordLoc.eBeamEnds, ref start, ref end);
			Start = start;
			End = end;
		}

		public Member(IVerticalBrace brace)
			: this(MemberType.VerticalBrace, EFRAMETYPE.MemberIsGravity)
		{
			VerticalBrace = brace;
			SCoordinate start = new SCoordinate();
			SCoordinate end = new SCoordinate();
			brace.GetEndCoordinates(ref start, ref end);
			Start = start;
			End = end;
		}

		public Member(IHorizBrace brace)
			: this(MemberType.HorizontalBrace, EFRAMETYPE.MemberIsGravity)
		{
			HorizontalBrace = brace;
			SCoordinate start = new SCoordinate();
			SCoordinate end = new SCoordinate();
			brace.GetEndCoordinates(ref start, ref end);
			Start = start;
			End = end;
		}
	}
}