#include "CppFeaApiWrapperDll.h"
#include "..\CppFeaApi\NativeFeaApi.h"

#pragma once

class CheckbotController
{
public:
	void Run(NativeFeaApi* pFeaApi);

	NativeFeaApi* pApi;
};
