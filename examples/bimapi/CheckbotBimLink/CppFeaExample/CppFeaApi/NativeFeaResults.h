#pragma once
#include "CppFeaApiDll.h"
#include "NativeFeaMemberResult.h"
#include <iostream>
#include <vector>
#include <map>


class CPPFEAAPIDLL_EXPORT NativeFeaResults
{
private:
	std::vector<NativeFeaMemberResult*> _resultsForMembers;

public:
	NativeFeaResults();
	~NativeFeaResults();

	std::vector<NativeFeaMemberResult*> GetMemberInternalForces(int memberId, int loadCaseId);

private:
	void LoadResults();
};

