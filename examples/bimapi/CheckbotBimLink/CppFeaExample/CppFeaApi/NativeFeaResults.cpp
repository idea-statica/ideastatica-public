#include "pch.h"
#include "NativeFeaResults.h"

NativeFeaResults::NativeFeaResults()
{
	this->_resultsForMembers = std::map<int, NativeFeaMemberResult*>();
}

NativeFeaResults::~NativeFeaResults()
{
	for (const auto& pair : _resultsForMembers) {
		delete pair.second;
	}
}

NativeFeaMemberResult* NativeFeaResults::GetMemberInternalForces(int memberId, int loadCaseId)
{
	throw std::exception("Not implemented");
}