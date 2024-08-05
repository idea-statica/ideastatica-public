namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConOperation
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public bool IsActive { get; set; }
		public bool IsImported { get; set; }
	}
}