using System.Collections.Generic;
using System.Linq;

namespace FeaExampleApi
{
	/// <summary>
	/// Reads the loads of the example FEA model. In a real link, this is the API of your application.
	/// </summary>
	public interface IFeaLoadsApi
	{
		/// <summary>
		/// The load group with the given id.
		/// </summary>
		FeaLoadGroup GetLoadGroup(int id);

		/// <summary>
		/// The load case with the given id.
		/// </summary>
		FeaLoadCase GetLoadCase(int id);

		/// <summary>
		/// Every load case in the model.
		/// </summary>
		IEnumerable<FeaLoadCase> GetAllLoadCases();

		/// <summary>
		/// The combination with the given id.
		/// </summary>
		FeaCombination GetCombination(int id);

		/// <summary>
		/// Every combination in the model.
		/// </summary>
		IEnumerable<FeaCombination> GetAllCombinations();
	}

	/// <summary>
	/// Serves fixed loads: self-weight and dead load in a permanent group, snow in a variable group, and two ultimate
	/// and two serviceability combinations of them.
	/// </summary>
	public class FeaLoadsApi : IFeaLoadsApi
	{
		private readonly List<FeaLoadGroup> _loadGroups = new List<FeaLoadGroup>
		{
			new FeaLoadGroup(1, "LG1", FeaLoadGroupType.Permanent),
			new FeaLoadGroup(2, "LG2", FeaLoadGroupType.Variable),
		};

		private readonly List<FeaLoadCase> _loadCases = new List<FeaLoadCase>
		{
			new FeaLoadCase(1, "Self-weight", FeaLoadCaseType.SelfWeight, loadGroupId: 1),
			new FeaLoadCase(2, "Dead load", FeaLoadCaseType.DeadLoad, loadGroupId: 1),
			new FeaLoadCase(3, "Snow", FeaLoadCaseType.Snow, loadGroupId: 2),
		};

		private readonly List<FeaCombination> _combinations = new List<FeaCombination>
		{
			new FeaCombination(1, "ULS-CO1", FeaLimitState.Ultimate, new[]
			{
				new FeaCombinationFactor(1, 1.35),
				new FeaCombinationFactor(2, 1.35),
			}),
			new FeaCombination(2, "ULS-CO2", FeaLimitState.Ultimate, new[]
			{
				new FeaCombinationFactor(1, 1.35),
				new FeaCombinationFactor(2, 1.35),
				new FeaCombinationFactor(3, 1.5),
			}),
			new FeaCombination(3, "SLS-CO3", FeaLimitState.Serviceability, new[]
			{
				new FeaCombinationFactor(1, 1.0),
				new FeaCombinationFactor(2, 1.0),
			}),
			new FeaCombination(4, "SLS-CO4", FeaLimitState.Serviceability, new[]
			{
				new FeaCombinationFactor(1, 1.0),
				new FeaCombinationFactor(2, 1.0),
				new FeaCombinationFactor(3, 1.0),
			}),
		};

		public FeaLoadGroup GetLoadGroup(int id) => _loadGroups.Single(group => group.Id == id);

		public FeaLoadCase GetLoadCase(int id) => _loadCases.Single(loadCase => loadCase.Id == id);

		public IEnumerable<FeaLoadCase> GetAllLoadCases() => _loadCases;

		public FeaCombination GetCombination(int id) => _combinations.Single(combination => combination.Id == id);

		public IEnumerable<FeaCombination> GetAllCombinations() => _combinations;
	}
}
