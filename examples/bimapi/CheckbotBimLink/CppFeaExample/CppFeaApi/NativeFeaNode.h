#pragma once
#include "CppFeaApiDll.h"


class CPPFEAAPIDLL_EXPORT NativeFeaNode
{
public:
	NativeFeaNode();
	NativeFeaNode(int id, double x, double y, double z);

	int Id;
	double X;
	double Y;
	double Z;
};
