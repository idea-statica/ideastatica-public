using System.Collections.Generic;

namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// Overal check item
	/// </summary>
	public class ConcreteCheckResultOverallItem
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConcreteCheckResultOverallItem()
		{
		}

		/// <summary>
		/// Check result type
		/// </summary>
		public CheckResultType ResultType { get; set; }

		/// <summary>
		/// check result passed/failed
		/// </summary>
		public CheckResult Result { get; set; }

		/// <summary>
		/// calculated limited value, calculated as strain to limit strain
		/// </summary>
		public double CheckValue { get; set; }
	}

	/// <summary>
	/// Concrete check result
	/// </summary>
	public class ConcreteCheckResultOverall
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConcreteCheckResultOverall()
		{
			Checks = new List<ConcreteCheckResultOverallItem>();
		}

		/// <summary>
		/// All check by the type
		/// </summary>
		public List<ConcreteCheckResultOverallItem> Checks { get; set; }
	}
}