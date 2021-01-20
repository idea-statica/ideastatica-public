using IdeaRS.OpenModel.Result;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// OpenModelContainer is used to keep structural data and results of a finite element analysis in one place.
	/// The main reason is easier moving (passing) pass the instance of OpenModel and corresponding instace of OpenModelResults.
	/// </summary>
	public class OpenModelContainer
	{
		/// <summary>
		/// Open Model - structural data
		/// </summary>
		public OpenModel OpenModel { get; set; }

		/// <summary>
		/// Open Model Result - results of a finite element analysis
		/// </summary>
		public OpenModelResult OpenModelResult { get; set; }
	}
}
