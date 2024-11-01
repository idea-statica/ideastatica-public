#pragma once
#include "CheckbotClientDll.h"
#include "..\SafProvider\NativeFeaApi.h"
#include <string>

extern "C" {
  /// <summary>
  /// Function to run the Checkbot
  /// </summary>
  /// <param name="pApi">NativeFeaApi represents the model from the source FEA</param>
  /// <param name="checkBotPath">The path to Checkbot.exe (IDEA StatiCa setup)</param>
  /// <returns>Returns 1 if success</returns>
  CHECKBOTCLIENTDLL_EXPORTS int RunCheckbot(NativeFeaApi* pApi, std::wstring checkBotPath);


  /// <summary>
  /// Release Checkbot
  /// </summary>
  /// <returns></returns>
  CHECKBOTCLIENTDLL_EXPORTS int ReleaseCheckbot();
}