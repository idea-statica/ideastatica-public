#pragma once
#include "CppFeaApiDll.h"
#include <string>
#include <iostream>

class CPPFEAAPIDLL_EXPORT NativeFeaLoadCase
{
public:
	NativeFeaLoadCase();
	NativeFeaLoadCase(int id, std::wstring name, int loadGroupId, int loadCaseType, int actionType);

	int Id;
	std::wstring Name;
	int LoadGroupId;
	int LoadCaseType;
	int ActionType;
};

