namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representation of rectangular opening in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Opening))]
	public class OpeningRectOffsets : Opening
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OpeningRectOffsets()
		{
		}

		/// <summary>
		/// Left
		/// </summary>
		public double X1 { get; set; }

		/// <summary>
		/// Right
		/// </summary>
		public double X2 { get; set; }

		/// <summary>
		/// Bottom
		/// </summary>
		public double Y1 { get; set; }

		/// <summary>
		/// Top
		/// </summary>
		public double Y2 { get; set; }

		/// <summary>
		/// top left X offset
		/// </summary>
		public double OffsetX1 { get; set; }

		/// <summary>
		/// top right x offset
		/// </summary>
		public double OffsetX2 { get; set; }

		/// <summary>
		/// right bottom y offset
		/// </summary>
		public double OffsetY1 { get; set; }

		/// <summary>
		/// right top y offset
		/// </summary>
		public double OffsetY2 { get; set; }
	}
}