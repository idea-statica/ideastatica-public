#include "pch.h"
#include "NativeFeaLoads.h"

NativeFeaLoads::NativeFeaLoads()
{
	loadCases = std::map<int, NativeFeaLoadCase*>();

	loadCases.insert(std::make_pair(1, new NativeFeaLoadCase(1, L"Selfweight", 1, LoadCase_Type_Selfweight, LC_ActionType_Pernament)));
	loadCases.insert(std::make_pair(2, new NativeFeaLoadCase(2, L"Dead load", 1, LoadCase_Type_Dead, LC_ActionType_Pernament)));
	loadCases.insert(std::make_pair(3, new NativeFeaLoadCase(3, L"Snow", 2, LoadCase_Type_Snow, LC_ActionType_Variable)));

	loadGroups = std::map<int, NativeFeaLoadGroup*>();
	loadGroups.insert(std::make_pair(1, new NativeFeaLoadGroup(1, L"LG1", LoadGroup_Category_Permanent)));
	loadGroups.insert(std::make_pair(2, new NativeFeaLoadGroup(2, L"LG2", LoadGroup_Category_Variable)));

	loadCombinations = std::map<int, NativeFeaLoadCombination*>();

	{
		std::vector<NativeFeaCombiFactor>* comb_LC_Factors = new std::vector<NativeFeaCombiFactor>();
		comb_LC_Factors->emplace_back(1, 1.35);
		comb_LC_Factors->emplace_back(2, 1.35);
		comb_LC_Factors->emplace_back(3, 1.50);

		loadCombinations.insert(std::make_pair(1, new NativeFeaLoadCombination(1, L"ULS-CO1", Combi_Category_ULS, Combi_Type_Linear, comb_LC_Factors)));
	}

	{
		std::vector<NativeFeaCombiFactor>* comb_LC_Factors = new std::vector<NativeFeaCombiFactor>();
		comb_LC_Factors->emplace_back(1, 1.35);
		comb_LC_Factors->emplace_back(2, 1.35);

		loadCombinations.insert(std::make_pair(2, new NativeFeaLoadCombination(2, L"ULS-CO2", Combi_Category_ULS, Combi_Type_Linear, comb_LC_Factors)));
	}

	{
		std::vector<NativeFeaCombiFactor>* comb_LC_Factors = new std::vector<NativeFeaCombiFactor>();
		comb_LC_Factors->emplace_back(1, 1.1);
		comb_LC_Factors->emplace_back(2, 1.1);

		loadCombinations.insert(std::make_pair(3, new NativeFeaLoadCombination(3, L"SLS-CO3", Combi_Category_SLS, Combi_Type_Linear, comb_LC_Factors)));
	}

	{
		std::vector<NativeFeaCombiFactor>* comb_LC_Factors = new std::vector<NativeFeaCombiFactor>();
		comb_LC_Factors->emplace_back(1, 1.1);
		comb_LC_Factors->emplace_back(2, 1.1);
		comb_LC_Factors->emplace_back(3, 0.9);

		loadCombinations.insert(std::make_pair(4, new NativeFeaLoadCombination(4, L"SLS-CO4", Combi_Category_SLS, Combi_Type_Linear, comb_LC_Factors)));
	}
}

NativeFeaLoads::~NativeFeaLoads()
{
	for (const auto& pair : loadCases) {
		delete pair.second;
	}

	for (const auto& pair : loadGroups) {
		delete pair.second;
	}

	for (const auto& pair : loadCombinations) {
		delete pair.second;
	}
}

std::vector<int> NativeFeaLoads::GetLoadCasesIdentifiers()
{
	std::vector<int> keys;
	for (const auto& pair : loadCases) {
		keys.push_back(pair.first);
	}

	return keys;
}

std::vector<int> NativeFeaLoads::GetLoadGroupsIdentifiers()
{
	std::vector<int> keys;
	for (const auto& pair : loadGroups) {
		keys.push_back(pair.first);
	}

	return keys;
}

std::vector<int> NativeFeaLoads::GetLoadCombinationsIdentifiers()
{
	std::vector<int> keys;
	for (const auto& pair : loadCombinations) {
		keys.push_back(pair.first);
	}

	return keys;
}

NativeFeaLoadCase* NativeFeaLoads::GetLoadCase(int id)
{
	return loadCases[id];
}

NativeFeaLoadGroup* NativeFeaLoads::GetLoadGroup(int id)
{
	return loadGroups[id];
}

NativeFeaLoadCombination* NativeFeaLoads::GetLoadCombination(int id)
{
	return loadCombinations[id];
}