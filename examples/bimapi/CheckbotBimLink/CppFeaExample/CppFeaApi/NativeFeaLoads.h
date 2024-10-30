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
	const static int Combi_Category_ULS = 0;
	const static int Combi_Category_SLS = 1;

	const static int Combi_Type_Linear = 0;

	const static int LC_ActionType_Pernament = 0;
	const static int LC_ActionType_Variable = 0;

	const static int LoadCase_Type_Selfweight = 0;
	const static int LoadCase_Type_Dead = 1;
	const static int LoadCase_Type_Snow = 2;

	const static int LoadGroup_Category_Permanent = 0;
	const static int LoadGroup_Category_Variable = 1;

	NativeFeaLoads();
	~NativeFeaLoads();

	std::vector<int> GetLoadCasesIdentifiers();
	std::vector<int> GetLoadGroupsIdentifiers();
	std::vector<int> GetLoadCombinationsIdentifiers();
	NativeFeaLoadCase* GetLoadCase(int id);
	NativeFeaLoadGroup* GetLoadGroup(int id);
	NativeFeaLoadCombination* GetLoadCombination(int id);
};

