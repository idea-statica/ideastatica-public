#pragma once
#include "CppFeaApiWrapperDll.h"
#include "..\CppFeaApi\NativeFeaApi.h"
#include <string>

extern "C" {
  CPPFEAAPIWRAPPER_EXPORT int RunCheckbot(NativeFeaApi* pApi, std::wstring checkBotPath);
}