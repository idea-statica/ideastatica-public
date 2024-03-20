using IdeaRS.OpenModel.Loading;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Detail.Loading
{
	/// <summary>
	/// Combination type
	/// </summary>
	public enum CombinationType
	{
		/// <summary>
		/// ULS
		/// </summary>
		ULS,

		/// <summary>
		/// SLS
		/// </summary>
		SLS
	}

	/// <summary>
	/// combination item
	/// </summary>
	public class CombinationItem
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CombinationItem()
		{
		}

		/// <summary>
		/// combination id
		/// </summary>
		public ReferenceElement LoadCase { get; set; }

		/// <summary>
		/// coefficient of combination
		/// </summary>
		[DataMember]
		public double Coefficient { get; set; }

		/// <summary>
		/// ratio of long-term part of loads in combination
		/// </summary>
		[DataMember]
		public double LongTermRatio { get; set; }
	}


	/// <summary>
	/// Load case
	/// </summary>
	public class DetailCombination : CalculationCase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public DetailCombination()
		{
			Items = new List<CombinationItem>();
		}

		/// <summary>
		/// Type of load case
		/// </summary>
		public CombinationType CombinationType { get; set; }

		/// <summary>
		/// Load
		/// </summary>
		/// <summary>
		/// Combi Items in combination
		/// </summary>
		public System.Collections.Generic.List<CombinationItem> Items { get; set; }
	}
}
