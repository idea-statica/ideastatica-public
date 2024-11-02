#pragma once
#include "SafProviderDll.h"

#include <string>
#include <iostream>

class SAFPROVIDERDLL_EXPORTS SafProviderBase
{
public:
	// Define a pure virtual function by assigning 0 to it
	virtual void GetSafData() = 0;

	/// <summary>
/// Get path to the FEA project on the disk. (Checkbot will use it to store its data)
/// </summary>
/// <returns></returns>
	virtual std::wstring GetProjectPath() = 0;


	virtual void SetProjectPath(std::wstring path) = 0;


	/// <summary>
	/// Name of the FEA
	/// </summary>
	/// <returns></returns>
	virtual std::wstring GetFeaName() = 0;


	// Virtual destructor for proper cleanup
	virtual ~SafProviderBase() = default;
};

