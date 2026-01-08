namespace ConnectionIomGenerator.Model
{
	public record ConnectionInput
	{
		public required string Material {  get; set; }

		public required List<Member> Members {  get; set; }

		public static ConnectionInput GetDefaultECEN()
		{
			var res = new ConnectionInput()
			{
				Material = "S 355",
				Members = new List<Member>
				{
					new Member() {Id = 1, Name = "COL", CrossSection = "HEA260", Direction = 0, Pitch = 90, Rotation = 0, IsContinuous = true},
					new Member() {Id = 2, Name = "M1", CrossSection = "IPE260", Direction = 0, Pitch = 0, Rotation = 0, IsContinuous = false}
				}
			};

			return res;
		}
	}
}
