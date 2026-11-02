using IdeaRS.OpenModel.Result;

namespace IdeaRS.OpenModel.Concrete.Load
{
	/// <summary>
	/// Prestress loading
	/// </summary>
	public class PrestressLoading
	{
		/// <summary>
		/// Time
		/// </summary>
		public double Time { get; set; }

		/// <summary>
		/// Primary internal force in section
		/// </summary>
		public ResultOfInternalForces PrimaryForces { get; set; }

		/// <summary>
		/// Secondary internal force in section
		/// </summary>
		public ResultOfInternalForces SecondaryForces { get; set; }
	}
}