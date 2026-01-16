namespace ConnectionIomGenerator.Model
{
	public record LoadImpulse
	{
		public required int MemberId { get; init; }
		public required int Position { get; init; }
		public double N { get; init; }
		public double Vy { get; init; }
		public  double Vz { get; init; }
		public  double Mx { get; init; }
		public  double My { get; init; }
		public  double Mz { get; init; }
	}
}
