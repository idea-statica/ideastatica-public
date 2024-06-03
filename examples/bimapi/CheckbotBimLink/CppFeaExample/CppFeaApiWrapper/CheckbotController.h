#include "CppFeaApiWrapperDll.h"
#include "..\CppFeaApi\NativeFeaApi.h"

#pragma once

ref class CheckbotController
{
private:
	static CheckbotController^ _instance;
	NativeFeaApi* pApi;

public:
	static CheckbotController^ Run(NativeFeaApi* pFeaApi);

	
};
