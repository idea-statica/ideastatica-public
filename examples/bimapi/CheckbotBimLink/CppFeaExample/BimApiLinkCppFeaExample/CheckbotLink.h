#pragma once

#include "CppFeaExampleApi/FeaApi.h"

#include <string>

#ifdef BIMAPILINKCPPFEAEXAMPLE_EXPORTS
#define BIMAPILINKCPPFEAEXAMPLE_API __declspec(dllexport)
#else
#define BIMAPILINKCPPFEAEXAMPLE_API __declspec(dllimport)
#endif

namespace BimApiLinkCppFeaExample
{
	/// <summary>
	/// Starts IDEA StatiCa Checkbot linked to the FEA application, and returns at once. Checkbot then asks the link
	/// for the model until the user closes Checkbot. One Checkbot runs at a time. The link writes its log to
	/// %TEMP%\IdeaStatiCa\Logs\BimApiLinkCppFeaExample.log.
	/// </summary>
	/// <param name="checkbotPath">Full path to IdeaCheckbot.exe of the installed IDEA StatiCa.</param>
	/// <param name="projectPath">
	/// Full path to a writable folder for the Checkbot project, one per FEA model, for example next to the model file.
	/// It is created if it does not exist, and the next run with the same folder opens the project again.
	/// </param>
	/// <param name="fea">
	/// The FEA application to read the model and its results from. Keep it until the application ends: Checkbot can
	/// ask for the model at any time.
	/// </param>
	/// <returns>An empty string when Checkbot has started; otherwise why it has not.</returns>
	BIMAPILINKCPPFEAEXAMPLE_API std::wstring StartCheckbot(
		const std::wstring& checkbotPath,
		const std::wstring& projectPath,
		const CppFeaExampleApi::IFeaApi& fea);
}
