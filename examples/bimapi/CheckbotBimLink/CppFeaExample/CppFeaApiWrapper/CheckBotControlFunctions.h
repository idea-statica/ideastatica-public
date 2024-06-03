#pragma once
#include "CppFeaApiWrapperDll.h"
#include "..\CppFeaApi\NativeFeaApi.h"

extern "C" {
  CPPFEAAPIWRAPPER_EXPORT int RunCheckbot(NativeFeaApi* pApi);
}