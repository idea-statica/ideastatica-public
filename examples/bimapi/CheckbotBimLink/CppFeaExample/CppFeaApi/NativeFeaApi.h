#pragma once
#include "CppFeaApiDll.h"
#include "NativeFeaGeometry.h"
#include <string>
#include <iostream>

class CPPFEAAPIDLL_EXPORT NativeFeaApi
{
private:
	NativeFeaGeometry* pFeaGeometry;
	std::wstring projectPath;

public:
	NativeFeaApi();
	~NativeFeaApi();

	NativeFeaGeometry* GetGeometry();

	std::wstring GetProjectPath();
	void SetProjectPath(std::wstring path);
};

