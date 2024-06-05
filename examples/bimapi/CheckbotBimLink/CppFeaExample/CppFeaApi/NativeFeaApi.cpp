#include "pch.h"
#include "NativeFeaApi.h"


using namespace std;

NativeFeaApi::NativeFeaApi()
{
	pFeaGeometry = new NativeFeaGeometry();
}

NativeFeaApi::~NativeFeaApi()
{
	delete pFeaGeometry;
}

NativeFeaGeometry* NativeFeaApi::GetGeometry()
{
	return pFeaGeometry;
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