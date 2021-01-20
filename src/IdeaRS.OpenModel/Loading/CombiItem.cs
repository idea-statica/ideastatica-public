namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Combi item
	/// </summary>
	[OpenModelClass("CI.StructModel.Loading.CombiItem")]
	public class CombiItem : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CombiItem()
		{
		}

		/// <summary>
		/// Id of CombiItem
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Load case
		/// </summary>
		public ReferenceElement LoadCase { get; set; }

		/// <summary>
		/// Coefficient
		/// </summary>
		public System.Double Coeff { get; set; }

		/// <summary>
		/// Inserted another combination
		/// </summary>
		public ReferenceElement Combination { get; set; }
	}
}