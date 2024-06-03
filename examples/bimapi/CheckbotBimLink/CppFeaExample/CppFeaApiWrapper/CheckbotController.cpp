#include "pch.h"
#include "CheckbotController.h"
#include "..\CppFeaApi\NativeFeaApi.h"
#include "CheckBotControlFunctions.h"

extern "C" __declspec(dllexport) int RunCheckbot(NativeFeaApi * pApi)
{
  return pApi->GetNodeCount();
}

void CheckbotController::Run(NativeFeaApi* pFeaApi)
{
	pApi = pFeaApi;
}
