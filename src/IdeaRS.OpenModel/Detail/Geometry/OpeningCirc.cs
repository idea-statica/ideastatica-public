namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representation of circular opening in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Opening))]
	public class OpeningCirc : Opening
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OpeningCirc()
		{
		}

		/// <summary>
		/// Diameter of opening
		/// </summary>
		public double Diameter { get; set; }
	}
}