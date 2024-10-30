#pragma once
#include "CppFeaApiDll.h"
#include "NativeFeaCombiFactor.h"
#include <string>
#include <iostream>
#include <vector>


class CPPFEAAPIDLL_EXPORT NativeFeaLoadCombination
{
public:
	NativeFeaLoadCombination();
	NativeFeaLoadCombination(int id, std::wstring name, int category, int type, std::vector<NativeFeaCombiFactor>* combiFactors);
	~NativeFeaLoadCombination();

	int Id;
	std::wstring Name;
	int Category;
	int Type;
	std::vector<NativeFeaCombiFactor>* CombiFactors;
};

