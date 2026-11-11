#pragma once

#include "FeaObjects.h"

#include <vector>

namespace CppFeaExampleApi
{
	/// <summary>
	/// Reads the loads of the example FEA model. In a real link, this is the API of your application.
	/// </summary>
	class IFeaLoadsApi
	{
	public:
		virtual ~IFeaLoadsApi() = default;

		/// <summary>
		/// The load group with the given id.
		/// </summary>
		virtual FeaLoadGroup GetLoadGroup(int id) const = 0;

		/// <summary>
		/// The load case with the given id.
		/// </summary>
		virtual FeaLoadCase GetLoadCase(int id) const = 0;

		/// <summary>
		/// Every load case in the model.
		/// </summary>
		virtual std::vector<FeaLoadCase> GetAllLoadCases() const = 0;

		/// <summary>
		/// The combination with the given id.
		/// </summary>
		virtual FeaCombination GetCombination(int id) const = 0;

		/// <summary>
		/// Every combination in the model.
		/// </summary>
		virtual std::vector<FeaCombination> GetAllCombinations() const = 0;
	};

	/// <summary>
	/// Serves fixed loads: self-weight and dead load in a permanent group, snow in a variable group, and two ultimate
	/// and two serviceability combinations of them.
	/// </summary>
	class FeaLoadsApi final : public IFeaLoadsApi
	{
	public:
		FeaLoadsApi();

		FeaLoadGroup GetLoadGroup(int id) const override;
		FeaLoadCase GetLoadCase(int id) const override;
		std::vector<FeaLoadCase> GetAllLoadCases() const override;
		FeaCombination GetCombination(int id) const override;
		std::vector<FeaCombination> GetAllCombinations() const override;

	private:
		std::vector<FeaLoadGroup> _loadGroups;
		std::vector<FeaLoadCase> _loadCases;
		std::vector<FeaCombination> _combinations;
	};
}
