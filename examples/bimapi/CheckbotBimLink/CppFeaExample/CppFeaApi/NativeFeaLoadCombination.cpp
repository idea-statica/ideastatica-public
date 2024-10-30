#include "pch.h"
#include "NativeFeaLoadCombination.h"

NativeFeaLoadCombination::NativeFeaLoadCombination()
{
	this->CombiFactors = new std::vector<NativeFeaCombiFactor>();
	this->Name = L"";
	this->Type = 0;
	this->Id = 0;
	this->Category = 0;
}

NativeFeaLoadCombination::NativeFeaLoadCombination(int id, std::wstring name, int category, int type, std::vector<NativeFeaCombiFactor>* combiFactors)
{
	this->Id = id;
	this->Name = name;
	this->Category = category;
	this->Type = type;
	this->CombiFactors = combiFactors;
}

NativeFeaLoadCombination::~NativeFeaLoadCombination()
{
	delete CombiFactors;
}