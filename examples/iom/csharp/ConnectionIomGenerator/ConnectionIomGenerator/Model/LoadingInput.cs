namespace ConnectionIomGenerator.Model
{
	public record LoadingInput
	{
		public required List<LoadCase> LoadCases { get; set; }
	}
}
