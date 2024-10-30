#include "pch.h"
#include "NativeFeaLoadCase.h"

NativeFeaLoadCase::NativeFeaLoadCase()
{
	this->Id = 0;
	this->Name = L"";
	this->LoadGroupId = 0;
	this->LoadCaseType = 0;
	this->ActionType = 0;
}

NativeFeaLoadCase::NativeFeaLoadCase(int id, std::wstring name, int loadGroupId, int loadCaseType, int actionType)
{
	this->Id = id;
	this->Name = name;
	this->LoadGroupId = loadGroupId;
	this->LoadCaseType = loadCaseType;
	this->ActionType = actionType;
}