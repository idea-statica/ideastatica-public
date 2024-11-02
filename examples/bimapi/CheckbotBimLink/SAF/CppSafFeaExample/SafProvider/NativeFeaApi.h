#pragma once
#include "SafProviderDll.h"
#include "SafProviderBase.h"

#include <string>
#include <iostream>

/// <summary>
/// It represents the native FEA model
/// </summary>
class SAFPROVIDERDLL_EXPORTS NativeFeaApi : public SafProviderBase
{
private:


	std::wstring projectDir;
	std::wstring projectName;

public:
	NativeFeaApi();
	~NativeFeaApi();


	/// <summary>
	/// Get path to the FEA project on the disk. (Checkbot will use it to store its data)
	/// </summary>
	/// <returns></returns>
	virtual std::wstring GetProjectPath() override;

	virtual std::wstring GetProjectName() override;


	virtual void SetProjectPath(std::wstring feaProjDir, std::wstring projectName) override;


	/// <summary>
	/// Name of the FEA
	/// </summary>
	/// <returns></returns>
	virtual std::wstring GetFeaName() override;

	virtual void GetSafData() override;
};

