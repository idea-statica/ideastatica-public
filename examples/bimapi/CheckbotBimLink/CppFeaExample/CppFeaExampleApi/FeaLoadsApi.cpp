#include "FeaLoadsApi.h"

#include "FindById.h"

namespace CppFeaExampleApi
{
	FeaLoadsApi::FeaLoadsApi()
		: _loadGroups{
			{ 1, L"LG1", FeaLoadGroupType::Permanent },
			{ 2, L"LG2", FeaLoadGroupType::Variable },
		}
		, _loadCases{
			// Id, name, type, load group
			{ 1, L"Self-weight", FeaLoadCaseType::SelfWeight, 1 },
			{ 2, L"Dead load", FeaLoadCaseType::DeadLoad, 1 },
			{ 3, L"Snow", FeaLoadCaseType::Snow, 2 },
		}
		, _combinations{
			// Id, name, limit state, { load case, factor }
			{ 1, L"ULS-CO1", FeaLimitState::Ultimate, { { 1, 1.35 }, { 2, 1.35 } } },
			{ 2, L"ULS-CO2", FeaLimitState::Ultimate, { { 1, 1.35 }, { 2, 1.35 }, { 3, 1.5 } } },
			{ 3, L"SLS-CO3", FeaLimitState::Serviceability, { { 1, 1.0 }, { 2, 1.0 } } },
			{ 4, L"SLS-CO4", FeaLimitState::Serviceability, { { 1, 1.0 }, { 2, 1.0 }, { 3, 1.0 } } },
		}
	{
	}

	FeaLoadGroup FeaLoadsApi::GetLoadGroup(int id) const
	{
		return FindById(_loadGroups, id);
	}

	FeaLoadCase FeaLoadsApi::GetLoadCase(int id) const
	{
		return FindById(_loadCases, id);
	}

	std::vector<FeaLoadCase> FeaLoadsApi::GetAllLoadCases() const
	{
		return _loadCases;
	}

	FeaCombination FeaLoadsApi::GetCombination(int id) const
	{
		return FindById(_combinations, id);
	}

	std::vector<FeaCombination> FeaLoadsApi::GetAllCombinations() const
	{
		return _combinations;
	}
}
