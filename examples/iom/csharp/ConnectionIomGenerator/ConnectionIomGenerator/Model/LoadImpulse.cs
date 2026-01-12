namespace ConnectionIomGenerator.Model
{
	public record LoadImpulse
	{
		public required int MemberId { get; init; }
		public required int Position { get; init; }
		public required double N { get; init; }
		public required double Vy { get; init; }
		public required double Vz { get; init; }
		public required double Mx { get; init; }
		public required double My { get; init; }
		public required double Mz { get; init; }
	}
}
