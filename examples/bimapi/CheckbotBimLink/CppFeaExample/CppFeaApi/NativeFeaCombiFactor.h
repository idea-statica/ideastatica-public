#pragma once
#include "CppFeaApiDll.h"


struct CPPFEAAPIDLL_EXPORT NativeFeaCombiFactor
{
public:
	NativeFeaCombiFactor();
	NativeFeaCombiFactor(int loadCaseId, double combiMultiplier);

	int LoadCaseId;
	double CombiMultiplier;
};

