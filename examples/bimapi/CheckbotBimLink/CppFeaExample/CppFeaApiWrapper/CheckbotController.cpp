#include "pch.h"
#include "CheckbotController.h"
#include "..\CppFeaApi\NativeFeaApi.h"

void CheckbotController::Run(NativeFeaApi* pFeaApi)
{
	pApi = pFeaApi;
}
