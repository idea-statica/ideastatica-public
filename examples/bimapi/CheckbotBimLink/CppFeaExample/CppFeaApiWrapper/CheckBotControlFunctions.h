#include "CppFeaApiWrapperDll.h"
#include "..\CppFeaApi\NativeFeaApi.h"

#pragma once

extern "C" {
	DLLEXPORT int RunCheckbot(NativeFeaApi* pApi);
}