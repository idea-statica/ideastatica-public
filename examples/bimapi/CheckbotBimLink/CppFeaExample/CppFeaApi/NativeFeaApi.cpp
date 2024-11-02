#include "pch.h"
#include "NativeFeaApi.h"


using namespace std;

NativeFeaApi::NativeFeaApi()
{
	pFeaGeometry = new NativeFeaGeometry();
	pFeaLoading = new NativeFeaLoads();
	pFeaResults = new NativeFeaResults();
}

NativeFeaApi::~NativeFeaApi()
{
	delete pFeaGeometry;
	delete pFeaLoading;
	delete pFeaResults;
}

NativeFeaGeometry* NativeFeaApi::GetGeometry()
{
	return pFeaGeometry;
}

NativeFeaLoads* NativeFeaApi::GetLoading()
{
	return pFeaLoading;
}

NativeFeaResults* NativeFeaApi::GetResults()
{
	return pFeaResults;
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