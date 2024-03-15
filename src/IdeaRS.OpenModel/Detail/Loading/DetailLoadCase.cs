using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Detail.Loading
{
	/// <summary>
	/// Type of load case
	/// </summary>
	public enum DetailLoadCaseType
	{
		/// <summary>
		/// Permanent
		/// </summary>
		Permanent,

		/// <summary>
		/// Variable
		/// </summary>
		Variable
	}

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
		public DetailLoadCaseType LoadCaseType { get; set; }

		/// <summary>
		/// Load
		/// </summary>
		public List<LoadBase> Load { get; set; }
	}
}
