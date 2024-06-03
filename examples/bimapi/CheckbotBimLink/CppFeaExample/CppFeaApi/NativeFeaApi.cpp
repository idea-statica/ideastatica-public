#include "pch.h"
#include "NativeFeaApi.h"
#include<iostream>

using namespace std;

NativeFeaApi::NativeFeaApi()
{
	pNodes = new NativeFeaNode[5];

}

NativeFeaApi::~NativeFeaApi()
{
	delete pNodes;
}

int NativeFeaApi::GetNodeCount()
{
	return 5;
}