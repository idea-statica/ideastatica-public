#pragma once
#include "CppFeaApiDll.h"
#include "NativeFeaGeometry.h"
#include <string>
#include <iostream>

/// <summary>
/// It represents the native FEA model
/// </summary>
class CPPFEAAPIDLL_EXPORT NativeFeaApi
{
private:
	NativeFeaGeometry* pFeaGeometry;
	std::wstring projectPath;

public:
	NativeFeaApi();
	~NativeFeaApi();

	/// <summary>
	/// Geometry of the FEA model
	/// </summary>
	/// <returns></returns>
	NativeFeaGeometry* GetGeometry();

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

