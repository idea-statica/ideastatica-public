#include "pch.h"
#include "NativeFeaApi.h"

#include <iostream>
#include <fstream>


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

std::wstring NativeFeaApi::GetFeaName()
{
	return L"CppSafFeaExample";
}

void NativeFeaApi::GetSafData()
{

}