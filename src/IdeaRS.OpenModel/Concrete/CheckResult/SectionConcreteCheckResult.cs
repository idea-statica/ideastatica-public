using System.Collections.Generic;

namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// ection concrete result
	/// </summary>
	public class SectionConcreteCheckResult
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SectionConcreteCheckResult()
		{
			ExtremeResults = new List<ConcreteCheckResults>();
		}

		/// <summary>
		/// Id Of section
		/// </summary>
		public int SectionId { get; set; }

		/// <summary>
		/// Extreme results
		/// </summary>
		public List<ConcreteCheckResults> ExtremeResults { get; set; }
	}
}