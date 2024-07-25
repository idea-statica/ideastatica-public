using System.Collections.Generic;
using System.Linq;

namespace BimApiLinkFeaExample.FeaExampleApi
{
	public interface IFeaLoadsApi
	{
		IFeaLoadCase GetLoadCase(int id);
		IFeaLoadGroup GetLoadGroup(int id);
		IFeaLoadCombination GetLoadCombination(int id);
		IEnumerable<int> GetLoadCasesIds();
		IEnumerable<int> GetLoadGroupsIds();
		IEnumerable<int> GetLoadCombinationsIds();
	}

	internal class FeaLoadsApi : IFeaLoadsApi
	{
		private List<IFeaLoadCase> _loadCases = InitialiazeLoadCases();
		private List<IFeaLoadGroup> _loadGroups = InitializeLoadGroups();
		private List<IFeaLoadCombination> _loadCombinations = InitializeLoadCombinations();
		
		public IFeaLoadCase GetLoadCase(int id) => _loadCases.FirstOrDefault(x => x.Id == id);

		public IEnumerable<int> GetLoadCasesIds() => _loadCases.Select(x => x.Id);

		public IFeaLoadCombination GetLoadCombination(int id) => _loadCombinations.FirstOrDefault(x => x.Id == id);		

		public IEnumerable<int> GetLoadCombinationsIds() => _loadCombinations.Select(x => x.Id);

		public IFeaLoadGroup GetLoadGroup(int id) => _loadGroups.FirstOrDefault(x => x.Id == id);		

		public IEnumerable<int> GetLoadGroupsIds() => _loadGroups.Select(x => x.Id);

		private static List<IFeaLoadCombination> InitializeLoadCombinations()
		{
			return new List<IFeaLoadCombination>
			{
				new FeaLoadCombination()
				{
					Id = 1,
					Name = "ULS-CO1",
					Category = Category.ULS,
					Type = Type.Linear,
					CombiFactors = new List<CombiFactor>()
					{
						new CombiFactor(1, 1.35),
						new CombiFactor(2, 1.35)
					}
				},
				new FeaLoadCombination()
				{
					Id = 2,
					Name = "ULS-CO2",
					Category = Category.ULS,
					Type = Type.Linear,
					CombiFactors = new List<CombiFactor>()
					{
						new CombiFactor(1, 1.35),
						new CombiFactor(2, 1.35),
						new CombiFactor(3, 1.5)
					}
				},
				new FeaLoadCombination()
				{
					Id = 3,
					Name = "SLS-CO3",
					Category = Category.SLS,
					Type = Type.Linear,
					CombiFactors = new List<CombiFactor>()
					{
						new CombiFactor(1, 1.1),
						new CombiFactor(2, 1.1)
					}
				},
				new FeaLoadCombination()
				{
					Id = 4,
					Name = "SLS-CO4",
					Category = Category.SLS,
					Type = Type.Linear,
					CombiFactors = new List<CombiFactor>()
					{
						new CombiFactor(1, 1.0),
						new CombiFactor(2, 1.0),
						new CombiFactor(3, 0.9)
					}
				},
			};
		}

		private static List<IFeaLoadCase> InitialiazeLoadCases()
		{
			return new List<IFeaLoadCase>()
			{
				new FeaLoadCase()
				{
					Id = 1,
					Name = "Selfweight",
					ActionType = ActionType.Permanent,
					LoadCaseType = TypeOfLoadCase.Selfweight,
					LoadGroupId = 1,
				},
				new FeaLoadCase()
				{
					Id = 2,
					Name = "Dead load",
					ActionType = ActionType.Permanent,
					LoadCaseType = TypeOfLoadCase.DeadLoad,
					LoadGroupId = 1,
				},
				new FeaLoadCase()
				{
					Id = 3,
					Name = "Snow",
					ActionType = ActionType.Variable,
					LoadCaseType = TypeOfLoadCase.Snow,
					LoadGroupId = 2,
				}
			};
		}

		private static List<IFeaLoadGroup> InitializeLoadGroups()
		{
			return new List<IFeaLoadGroup>()
			{
				new FeaLoadGroup()
				{
					Id = 1,
					Name = "LG1",
					LoadGroupCategory = LoadGroupCategory.Permanent
				},
				new FeaLoadGroup()
				{
					Id = 2,
					Name = "LG2",
					LoadGroupCategory = LoadGroupCategory.Variable
				}
			};
		}
	}
}
