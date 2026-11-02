using IdeaRS.OpenModel.Result;
using System.Collections.Generic;

namespace IdeaRS.OpenModel.Concrete.Load
{
	/// <summary>
	/// Cross-section coponent loading
	/// </summary>
	public class CssComponentLoading
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CssComponentLoading()
		{
			Loading = new List<CssComponentTimeLoading>();
		}

		/// <summary>
		/// Id of cross-section component
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Loading
		/// </summary>
		public List<CssComponentTimeLoading> Loading { get; set; }
	}

	/// <summary>
	/// Cross-section coponent loading
	/// </summary>
	public class CssComponentTimeLoading
	{
		/// <summary>
		/// Time
		/// </summary>
		public double Time { get; set; }

		/// <summary>
		/// Loading
		/// </summary>
		public SectionResultBase Loading { get; set; }
	}
}