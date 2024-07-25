using System.Collections.Generic;

namespace BimApiLinkFeaExample.FeaExampleApi
{
	public enum Category
	{ 
		ULS,
		SLS,
		ALS
	}

	public enum Type
	{ 
		Linear,
		Nonlinear,
		Envelope
	}
	
	public class CombiFactor
	{ 
		public CombiFactor(int loadCaseId, double combiMultiplier) 
		{
			LoadCaseId = loadCaseId;
			CombiMultiplier = combiMultiplier;
		}

		/// <summary>
		/// Id of load case in combination
		/// </summary>
		public int LoadCaseId { get; }

		/// <summary>
		/// Value of multiplier factor for the load case
		/// in the combination
		/// </summary>
		public double CombiMultiplier { get; }
	}

	public interface IFeaLoadCombination
	{
		int Id { get; }
		string Name { get; }
		Category Category { get; }
		Type Type { get; }		
		IEnumerable<CombiFactor> CombiFactors { get; }
	}

	internal class FeaLoadCombination : IFeaLoadCombination
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Category Category { get; set; }
		public Type Type { get; set; }
		public IEnumerable<CombiFactor> CombiFactors { get; set; }
	}
}
