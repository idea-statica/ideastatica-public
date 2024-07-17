namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	/// <summary>
	/// Class to perform updating of an active/non-active item.
	/// </summary>
	public class ConItem
	{
		public ConItem()
		{
			Id = 0;
		}

		public ConItem(int id)
		{
			Id = id;
		}

		// ID will be automatically generated
		public int Id { get; private set; }

		public string Name { get; set; }

		public bool Active { get; set; }
	}
}
