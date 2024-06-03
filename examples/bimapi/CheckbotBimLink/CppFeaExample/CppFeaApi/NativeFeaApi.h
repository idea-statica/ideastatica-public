#include "CppFeaApiDll.h"
#include "NativeFeaNode.h"
#pragma once

class DLLEXPORT NativeFeaApi
{
public:
	NativeFeaApi();
	~NativeFeaApi();

	NativeFeaNode* pNodes;

	int GetNodeCount();
};

