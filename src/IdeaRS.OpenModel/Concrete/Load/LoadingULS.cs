using IdeaRS.OpenModel.Result;

namespace IdeaRS.OpenModel.Concrete.Load
{
	/// <summary>
	/// Loading ULS
	/// </summary>
	public class LoadingULS
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LoadingULS()
		{
			//InternalForces = new ResultOfInternalForces();
			//InternalForcesBegin = new ResultOfInternalForces();
			//InternalForcesEnd = new ResultOfInternalForces();
			//InternalForces2ndOrder = new ResultOfInternalForces();
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
		/// Internal forces at the beginnig
		/// </summary>
		public ResultOfInternalForces InternalForcesBegin { get; set; }

		/// <summary>
		/// Internal forces at the end
		/// </summary>
		public ResultOfInternalForces InternalForcesEnd { get; set; }

		/// <summary>
		/// Internal forces of 2nd order effect
		/// </summary>
		public ResultOfInternalForces InternalForces2ndOrder { get; set; }

		/// <summary>
		/// Internal forces of imperfection effect
		/// </summary>
		public ResultOfInternalForces InternalForcesImperfection { get; set; }
	}
}