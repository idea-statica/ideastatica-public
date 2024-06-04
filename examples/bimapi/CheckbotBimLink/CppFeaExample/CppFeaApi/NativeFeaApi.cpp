#include "pch.h"
#include "NativeFeaApi.h"
#include<iostream>

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
