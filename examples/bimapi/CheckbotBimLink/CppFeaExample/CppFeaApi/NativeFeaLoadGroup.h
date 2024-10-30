#pragma once
#include "CppFeaApiDll.h"
#include <string>
#include <iostream>

class CPPFEAAPIDLL_EXPORT NativeFeaLoadGroup
{
public:
	NativeFeaLoadGroup();
	NativeFeaLoadGroup(int id, std::wstring name, int loadGroupCategory);

	int Id;
	std::wstring Name;
	int LoadGroupCategory;
};

