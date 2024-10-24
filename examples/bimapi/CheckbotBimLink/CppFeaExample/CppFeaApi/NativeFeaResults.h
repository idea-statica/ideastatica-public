#pragma once
#include "CppFeaApiDll.h"
#include "NativeFeaMemberResult.h"
#include <iostream>
#include <vector>
#include <map>


class CPPFEAAPIDLL_EXPORT NativeFeaResults
{
private:
	std::map<int, NativeFeaMemberResult*> _resultsForMembers;

public:
	NativeFeaResults();
	~NativeFeaResults();

	NativeFeaMemberResult* GetMemberInternalForces(int memberId, int loadCaseId);
};

