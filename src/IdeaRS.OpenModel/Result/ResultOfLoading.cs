using System.Collections.Generic;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Result Of Loading
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class ResultOfLoading : Loading
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOfLoading()
		{
			Items = new List<ResultOfLoadingItem>();
		}

		/// <summary>
		/// Items od loading
		/// </summary>
		public List<ResultOfLoadingItem> Items { get; set; }
	}

	/// <summary>
	/// Item of Result of Loading
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class ResultOfLoadingItem
	{
		/// <summary>
		/// Loading
		/// </summary>
		public Loading Loading { get; set; }

		/// <summary>
		/// Coefficient of loading
		/// </summary>
		public double Coefficient { get; set; }
	}
}