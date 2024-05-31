namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	/// <summary>
	/// Class to perform updating of an active/non-active item.
	/// </summary>
	public class ConItem
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public bool Active { get; set; }
	}
}
