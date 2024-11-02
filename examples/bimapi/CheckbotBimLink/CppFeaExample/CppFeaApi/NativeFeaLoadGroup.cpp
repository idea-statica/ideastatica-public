#include "pch.h"
#include "NativeFeaLoadGroup.h"

NativeFeaLoadGroup::NativeFeaLoadGroup()
{
	this->Id = 0;
	this->Name = L"";
	this->LoadGroupCategory = 0;
}

NativeFeaLoadGroup::NativeFeaLoadGroup(int id, std::wstring name, int loadGroupCategory)
{
	this->Id = id;
	this->Name = name;
	this->LoadGroupCategory = loadGroupCategory;
}