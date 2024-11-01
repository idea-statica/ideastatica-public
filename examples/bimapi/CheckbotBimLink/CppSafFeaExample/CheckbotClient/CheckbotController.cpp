#include "pch.h"
#include "CheckbotController.h"
#include "..\SafProvider\NativeFeaApi.h"
#include "CheckBotControlFunctions.h"

#include "Model.h"


using namespace Autofac;
using namespace Autofac::Extensions::DependencyInjection;
using namespace IdeaStatiCa::BimApiLink;

/// <summary>
/// Function to run the checkbot
/// </summary>
/// <param name="pApi">NativeFeaApi represents the model from the source FEA</param>
/// <param name="checkBotPath">The path to Checkbot.exe (IDEA StatiCa setup)</param>
/// <returns>Returns 1 if success</returns>
extern "C" __declspec(dllexport) int RunCheckbot(NativeFeaApi * pApi, std::wstring checkBotPath)
{
	try
	{
		String^ checkbotPath = gcnew System::String(checkBotPath.c_str());

		//CppFeaApiWrapper::CheckbotController::Run(checkbotPath, pApi);
		return 1;
	}
	catch (System::Exception^ ex)
	{

		return 0;
	}
}

/// <summary>
/// Release Checkbot
/// </summary>
/// <returns></returns>
extern "C" __declspec(dllexport) int ReleaseCheckbot()
{
	try
	{
		//CppFeaApiWrapper::CheckbotController::Release();
		return 1;
	}
	catch (System::Exception^ ex)
	{

		return 0;
	}
}


