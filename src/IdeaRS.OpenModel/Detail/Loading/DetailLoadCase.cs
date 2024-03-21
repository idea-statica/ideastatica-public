using IdeaRS.OpenModel.Loading;
using System.Collections.Generic;

namespace IdeaRS.OpenModel.Detail.Loading
{

	/// <summary>
	/// Load case
	/// </summary>
	public class DetailLoadCase : CalculationCase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public DetailLoadCase()
		{
			Load = new List<LoadBase>();
		}

		/// <summary>
		/// Type of load case
		/// </summary>
		public LoadCaseType LoadCaseType { get; set; }

		/// <summary>
		/// Load
		/// </summary>
		public List<LoadBase> Load { get; set; }
	}
}
