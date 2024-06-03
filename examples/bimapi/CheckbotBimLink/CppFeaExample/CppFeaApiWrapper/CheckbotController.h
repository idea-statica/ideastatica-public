#include "CppFeaApiWrapperDll.h"
#include "..\CppFeaApi\NativeFeaApi.h"

#pragma once

ref class CheckbotController
{
public:
	void Run(NativeFeaApi* pFeaApi);

	NativeFeaApi* pApi;
};
