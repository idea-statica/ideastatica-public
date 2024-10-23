#pragma once
#include "CppFeaApiDll.h"


class CPPFEAAPIDLL_EXPORT NativeFeaCombiFactor
{
public:
	NativeFeaCombiFactor();
	NativeFeaCombiFactor(int loadCaseId, double combiMultiplier);

	int LoadCaseId;
	double CombiMultiplier;
};

