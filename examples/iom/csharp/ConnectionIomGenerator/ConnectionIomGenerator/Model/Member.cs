namespace ConnectionIomGenerator.Model
{
	public record Member
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string CrossSection { get; set; }
		public float Direction { get; set; }
		public float Pitch { get; set; }
		public float Rotation { get; set; }
		public bool IsContinuous { get; set; }
	}
}
