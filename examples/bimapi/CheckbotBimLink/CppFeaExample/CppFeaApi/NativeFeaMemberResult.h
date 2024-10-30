#pragma once
#include "CppFeaApiDll.h"
#include <string>
#include <iostream>

class CPPFEAAPIDLL_EXPORT NativeFeaMemberResult
{
public:
	NativeFeaMemberResult();
	NativeFeaMemberResult(int memberId, std::wstring resultType, int loadCaseId, double location, int index, double n, double vy, double vz, double mx, double my, double mz);

	int MemberId;
	std::wstring ResultType;
	int LoadCaseId;
	double Location;
	int Index;
	double N;
	double Vy;
	double Vz;
	double Mx;
	double My;
	double Mz;
};

