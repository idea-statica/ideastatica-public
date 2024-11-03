#include "pch.h"
#include "NativeFeaApi.h"

#include <iostream>
#include <fstream>
#include <filesystem> 

using namespace std;

NativeFeaApi::NativeFeaApi()
{
}

NativeFeaApi::~NativeFeaApi()
{

}

std::wstring NativeFeaApi::GetProjectName()
{
	return projectName;
}

std::wstring NativeFeaApi::GetProjectPath()
{
	return projectDir;
}

void NativeFeaApi::SetProjectPath(std::wstring feaProjDir, std::wstring projectName)
{
	this->projectDir = feaProjDir;
	this->projectName = projectName;
}

void NativeFeaApi::SetSafFilePath(std::wstring safFilePath)
{
	this->_safFilePath = safFilePath;
}

std::wstring NativeFeaApi::GetFeaName()
{
	return L"CppSafFeaExample";
}

void NativeFeaApi::ExportToSafFile(std::wstring safFilePath)
{
	// TODO : Do not copy existing SAF file but generate the real one from your FEA model
	std::filesystem::copy(_safFilePath, safFilePath, std::filesystem::copy_options::overwrite_existing);
}