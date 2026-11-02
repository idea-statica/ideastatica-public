using System.Collections.Generic;

namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// Concrete Check results
	/// </summary>
	public class ConcreteCheckResults
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConcreteCheckResults()
		{
			CheckResults = new List<ConcreteCheckResult>();
		}

		/// <summary>
		/// All results
		/// </summary>
		public List<ConcreteCheckResult> CheckResults { get; set; }

		/// <summary>
		/// Overal check
		/// </summary>
		public ConcreteCheckResultOverall Overall { get; set; }
	}
}