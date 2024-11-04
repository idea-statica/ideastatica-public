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
	std::wstring _safFilePath;

public:
	NativeFeaApi();
	~NativeFeaApi();

	/// <summary>
	/// 
	/// </summary>
	/// <param name="feaProjDir"></param>
	/// <param name="projectName"></param>
	virtual void SetProjectPath(std::wstring feaProjDir, std::wstring projectName) override;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="safFilePath"></param>
	virtual void SetSafFilePath(std::wstring safFilePath) override;

	/// <summary>
	/// Get path to the FEA project on the disk. (Checkbot will use it to store its data)
	/// </summary>
	/// <returns></returns>
	virtual std::wstring GetProjectPath() override;

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	virtual std::wstring GetProjectName() override;

	/// <summary>
	/// Name of the FEA
	/// </summary>
	/// <returns></returns>
	virtual std::wstring GetFeaName() override;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="safFilePath"></param>
	virtual void ExportToSafFile(std::wstring safFilePath) override;
};

