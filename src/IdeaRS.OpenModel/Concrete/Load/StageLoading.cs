using IdeaRS.OpenModel.Result;

namespace IdeaRS.OpenModel.Concrete.Load
{
	/// <summary>
	/// Staged loading
	/// </summary>
	public class StageLoading
	{
		/// <summary>
		/// Time
		/// </summary>
		public double Time { get; set; }

		/// <summary>
		/// Primary internal force in section
		/// </summary>
		public ResultOfInternalForces PermanentForces { get; set; }
	}
}