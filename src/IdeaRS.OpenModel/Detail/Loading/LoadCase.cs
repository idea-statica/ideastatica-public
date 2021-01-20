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
	public enum LoadCaseType
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
	public class LoadCase : CalculationCase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LoadCase()
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
