#include "pch.h"
#include "NativeFeaApi.h"


using namespace std;

NativeFeaApi::NativeFeaApi()
{
	pFeaGeometry = new NativeFeaGeometry();
	pFeaLoading = new NativeFeaLoads();
}

NativeFeaApi::~NativeFeaApi()
{
	delete pFeaGeometry;
	delete pFeaLoading;
}

NativeFeaGeometry* NativeFeaApi::GetGeometry()
{
	return pFeaGeometry;
}

NativeFeaLoads* NativeFeaApi::GetLoading()
{
	return pFeaLoading;
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