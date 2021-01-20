namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Standard check section extreme
	/// </summary>
	public class StagedCheckSectionExtreme : CheckSectionExtreme
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public StagedCheckSectionExtreme()
			: base()
		{
			Time = 0.0;
		}

		/// <summary>
		/// Time
		/// </summary>
		public double Time { get; set; }
	}
}