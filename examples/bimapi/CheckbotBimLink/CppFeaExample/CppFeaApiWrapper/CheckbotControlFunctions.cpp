#include "pch.h"
#include "CheckBotControlFunctions.h"
#include "CheckbotController.h"

CheckbotController* gCheckbotController = nullptr;

int RunCheckbot(NativeFeaApi* pApi)
{
	if (gCheckbotController != nullptr)
	{
		return 1;

	}

	gCheckbotController = new CheckbotController();
	gCheckbotController->Run(pApi);

	return 2;
}