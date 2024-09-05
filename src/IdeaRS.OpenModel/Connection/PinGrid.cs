namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Data of the pin grid
	/// </summary>
	public class PinGrid : FastenerGridBase
	{
		/// <summary>
		/// Replaceable pin
		/// </summary>
		public bool IsReplaceable { get; set; }

		/// <summary>
		/// Pin
		/// </summary>
		public ReferenceElement Pin { get; set; }
	}
}
