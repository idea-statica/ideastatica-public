namespace IdeaRS.OpenModel.Concrete.Load
{
	/// <summary>
	/// Fatigue loading
	/// </summary>
	public class FatigueLoading
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public FatigueLoading()
		{
			//MaxLoading = new LoadingULS();
			//MinLoading = new LoadingULS();
		}

		/// <summary>
		/// Max. cyclic loading
		/// </summary>
		public LoadingULS MaxLoading { get; set; }

		/// <summary>
		/// Min. cyclic loading
		/// </summary>
		public LoadingULS MinLoading { get; set; }
	}
}