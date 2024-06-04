#pragma once
#include "CppFeaApiDll.h"

class CPPFEAAPIDLL_EXPORT NativeFeaMember
{
public:
	NativeFeaMember();
	NativeFeaMember(int id, int beginNode, int endNode, int cssId);

	int Id;
	int BeginNode;
	int EndNode;
	int CrossSectionId;
};