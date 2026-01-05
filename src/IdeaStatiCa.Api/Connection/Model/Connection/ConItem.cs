namespace IdeaStatiCa.Api.Connection.Model
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
		public int Id { get; set; }

		/// <summary>
		/// User defined name of the operation
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// True if operation is active
		/// </summary>
		public bool Active { get; set; }
	}
}
