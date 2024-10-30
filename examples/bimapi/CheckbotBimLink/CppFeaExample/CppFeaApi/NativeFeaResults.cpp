#include "pch.h"
#include "NativeFeaResults.h"
#include <windows.h>
#include <filesystem>
#include <iostream>
#include <fstream>
#include "json.hpp"

namespace fs = std::filesystem;
using json = nlohmann::json;

NativeFeaResults::NativeFeaResults()
{
	LoadResults();
}

NativeFeaResults::~NativeFeaResults()
{
	for (const auto& res : _resultsForMembers) {
		delete res;
	}
}

std::vector<NativeFeaMemberResult*> NativeFeaResults::GetMemberInternalForces(int memberId, int loadCaseId)
{
	std::vector<NativeFeaMemberResult*> matchingResults;
	std::copy_if(_resultsForMembers.begin(), _resultsForMembers.end(), std::back_inserter(matchingResults), [memberId, loadCaseId](NativeFeaMemberResult* res) {
		return res->MemberId == memberId && res->LoadCaseId == loadCaseId;
		});

	return matchingResults;
}

void NativeFeaResults::LoadResults()
{
	//auto curDir = fs::current_path();

	char path[MAX_PATH];
	GetModuleFileNameA(NULL, path, MAX_PATH);
	fs::path exePath = path;
	fs::path curDir = exePath.parent_path(); // Get the directory path


	fs::path filePath = curDir / fs::path("ResultsData.json");

	std::ifstream f(filePath);
	json data = json::parse(f);

	if (!data.is_array()) {
		std::cerr << "Expected an array in the JSON file" << std::endl;
		return;
	}

	this->_resultsForMembers = std::vector<NativeFeaMemberResult*>();

	for (const auto& item : data) {
		// Process each item in the array
		int memberId = item["MemberId"];
		std::string resultType = item["ResultType"];
		int lcId = item["LoadCaseId"];
		double location = item["Location"];
		int index = item["Index"];
		double n = item["N"];
		double vy = item["Vy"];
		double vz = item["Vz"];
		double mx = item["Mx"];
		double my = item["My"];
		double mz = item["Mz"];

		NativeFeaMemberResult* pRes = new NativeFeaMemberResult();
		pRes->MemberId = memberId;
		pRes->LoadCaseId = lcId;
		pRes->Location = location;
		std::wstring wResultType(resultType.begin(), resultType.end());
		pRes->ResultType = wResultType;
		pRes->Index = index;
		pRes->N = n;
		pRes->Vy = vy;
		pRes->Vz = vz;
		pRes->Mx = mx;
		pRes->My = my;
		pRes->Mz = mz;
		_resultsForMembers.push_back(pRes);
	}
}