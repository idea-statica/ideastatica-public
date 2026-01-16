namespace ConnectionIomGenerator.Model
{
	public record LoadCase
	{
		public int Id { get; init; }
		public required string Name {get; init;}

		public required List<LoadImpulse> LoadImpulses {get; init;}
	}
}
