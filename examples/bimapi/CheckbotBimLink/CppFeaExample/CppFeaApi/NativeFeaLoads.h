#pragma once
#include "CppFeaApiDll.h"
#include "NativeFeaLoadCase.h"
#include "NativeFeaLoadGroup.h"
#include "NativeFeaLoadCombination.h"
#include <iostream>
#include <vector>
#include <map>


class CPPFEAAPIDLL_EXPORT NativeFeaLoads
{
private:
	std::map<int, NativeFeaLoadCase*> loadCases;
	std::map<int, NativeFeaLoadGroup*> loadGroups;
	std::map<int, NativeFeaLoadCombination*> loadCombinations;


public:
	NativeFeaLoads();
	~NativeFeaLoads();

	std::vector<int> GetLoadCasesIdentifiers();
	std::vector<int> GetLoadGroupsIdentifiers();
	std::vector<int> GetLoadCombinationsIdentifiers();
	NativeFeaLoadCase* GetLoadCase(int id);
	NativeFeaLoadGroup* GetLoadGroup(int id);
	NativeFeaLoadCombination* GetLoadCombination(int id);
};

