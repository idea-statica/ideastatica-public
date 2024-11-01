#pragma once
#include "SafProviderDll.h"

#include <string>
#include <iostream>

/// <summary>
/// It represents the native FEA model
/// </summary>
class SAFPROVIDERDLL_EXPORTS NativeFeaApi
{
private:


	std::wstring projectPath;

public:
	NativeFeaApi();
	~NativeFeaApi();


	/// <summary>
	/// Get path to the FEA project on the disk. (Checkbot will use it to store its data)
	/// </summary>
	/// <returns></returns>
	std::wstring GetProjectPath();


	void SetProjectPath(std::wstring path);


	/// <summary>
	/// Name of the FEA
	/// </summary>
	/// <returns></returns>
	std::wstring GetFeaName();
};

