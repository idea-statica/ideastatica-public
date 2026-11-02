using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	public enum SearchOption
	{
		Ignore,
		Must,
		MustNot
	}

	/// <summary>
	/// Represents the parameters used to search for connection libraries in a structural engineering context.
	/// </summary>
	/// <remarks>This class provides various options to filter the search results based on specific criteria such as
	/// membership in predefined, personal, or company sets, and the presence of certain structural features like bolts,
	/// welds, anchors, and more. Each feature can be set to be included, excluded, or ignored in the search.</remarks>
	public class ConConnectionLibrarySearchParameters
	{
		public ConConnectionLibrarySearchParameters()
		{
			InPredefinedSet = true;
			InPersonalSet = true;
			InCompanySet = true;

			Members = new List<int>();

			HasBolts = SearchOption.Ignore;
			HasWelds = SearchOption.Ignore;
			HasAnchor = SearchOption.Ignore;
			HasClipAngles = SearchOption.Ignore;
			IsMoment = SearchOption.Ignore;
			IsShear = SearchOption.Ignore;
			IsTruss = SearchOption.Ignore;
			IsParametric = SearchOption.Ignore;
		}

		public List<int> Members { get; set; }

		public bool InPredefinedSet { get; set; }
		public bool InCompanySet { get; set; }
		public bool InPersonalSet { get; set; }

		public SearchOption HasBolts { get; set; }
		public SearchOption HasWelds { get; set; }
		public SearchOption HasAnchor { get; set; }
		public SearchOption HasClipAngles { get; set; }
		public SearchOption IsMoment { get; set; }
		public SearchOption IsShear { get; set; }
		public SearchOption IsTruss { get; set; }
		public SearchOption IsParametric { get; set; }
	}
}
