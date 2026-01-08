namespace ConnectionIomGenerator.Model
{
	public record Member
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string CrossSection { get; set; }
		public double Direction { get; set; }
		public double Pitch { get; set; }
		public double Rotation { get; set; }
		public bool IsContinuous { get; set; }
	}
}
