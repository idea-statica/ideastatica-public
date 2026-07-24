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
		/// True if the item is active. Defaults to true (GUI parity): the desktop always creates active
		/// items, so a POST that omits <c>active</c> deserializes without touching this property and stays
		/// active, instead of the bool default (false) that silently stored an inactive no-op (#35385).
		/// </summary>
		public bool Active { get; set; } = true;
	}
}
