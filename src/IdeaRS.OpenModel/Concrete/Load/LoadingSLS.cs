using IdeaRS.OpenModel.Result;

namespace IdeaRS.OpenModel.Concrete.Load
{
	/// <summary>
	/// Loading SLS
	/// </summary>
	public class LoadingSLS
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LoadingSLS()
		{
			//InternalForces = new ResultOfInternalForces();
			//InternalForcesImperfection = new ResultOfInternalForces();
		}

		/// <summary>
		/// Internal force in section (permanent for staged section)
		/// </summary>
		public ResultOfInternalForces InternalForces { get; set; }

		/// <summary>
		/// Variable internal force in staged section only
		/// </summary>
		public ResultOfInternalForces InternalForcesVariable { get; set; }

		/// <summary>
		/// Internal forces of imperfection effect
		/// </summary>
		public ResultOfInternalForces InternalForcesImperfection { get; set; }
	}
}