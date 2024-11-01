#include "pch.h"
#include "NativeFeaApi.h"


using namespace std;

NativeFeaApi::NativeFeaApi()
{
}

NativeFeaApi::~NativeFeaApi()
{

}



std::wstring NativeFeaApi::GetProjectPath()
{
	return projectPath;
}

void NativeFeaApi::SetProjectPath(std::wstring path)
{
	projectPath = path;
}

std::wstring NativeFeaApi::GetFeaName()
{
	return L"NativeFeaApi";
}