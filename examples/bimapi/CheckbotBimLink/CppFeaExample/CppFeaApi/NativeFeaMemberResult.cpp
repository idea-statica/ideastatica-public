#include "pch.h"
#include "NativeFeaMemberResult.h"

NativeFeaMemberResult::NativeFeaMemberResult()
{
	MemberId = 0;
	std::wstring ResultType = L"";
	LoadCaseId = 0;
	Location = 0.0;
	Index = 0;
	N = 0.0;
	Vy = 0.0;
	Vz = 0.0;
	Mx = 0.0;
	My = 0.0;
	Mz = 0.0;
}

NativeFeaMemberResult::NativeFeaMemberResult(int memberId, std::wstring resultType, int loadCaseId, double location, int index, double n, double vy, double vz, double mx, double my, double mz)
{
	MemberId = memberId;
	ResultType = resultType;
	LoadCaseId = loadCaseId;
	Location = location;
	Index = index;
	N = n;
	Vy = vy;
	Vz = vz;
	Mx = mx;
	My = my;
	Mz = mz;
}