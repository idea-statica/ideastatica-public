#include "pch.h"
#include "CheckbotController.h"
#include "..\CppFeaApi\NativeFeaApi.h"
#include "CheckBotControlFunctions.h"


extern "C" __declspec(dllexport) int RunCheckbot(NativeFeaApi * pApi)
{
	CheckbotController::Run(pApi);
  return 1;
}

CheckbotController^ CheckbotController::Run(NativeFeaApi* pFeaApi)
{
	if (_instance != nullptr)
	{
		throw gcnew System::Exception("CheckbotController is already running");
	}

	_instance = gcnew CheckbotController();
	_instance->pApi = pFeaApi;
	return _instance;
}
