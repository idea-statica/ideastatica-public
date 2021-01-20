namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representation of rectangular opening in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Opening))]
	public class OpeningRect : Opening
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OpeningRect()
		{
		}

		/// <summary>
		/// Width of wall
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		/// Height of wall
		/// </summary>
		public double Height { get; set; }
	}
}