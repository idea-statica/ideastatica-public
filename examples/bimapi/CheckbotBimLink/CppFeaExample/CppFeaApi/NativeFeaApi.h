#pragma once
#include "CppFeaApiDll.h"
#include "NativeFeaGeometry.h"


class CPPFEAAPIDLL_EXPORT NativeFeaApi
{
private:
	NativeFeaGeometry* pFeaGeometry;

public:
	NativeFeaApi();
	~NativeFeaApi();

	NativeFeaGeometry* GetGeometry();
};

