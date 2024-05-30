#include "CppFeaApiDll.h"

#pragma once
class DLLEXPORT NativeFeaNode
{
public:
	NativeFeaNode();
	NativeFeaNode(int id, double x, double y, double z);

	int Id;
	double X;
	double Y;
	double Z;
};

