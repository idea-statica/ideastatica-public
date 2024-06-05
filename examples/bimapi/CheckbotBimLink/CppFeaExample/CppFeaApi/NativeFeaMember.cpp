#include "pch.h"
#include "NativeFeaMember.h"

NativeFeaMember::NativeFeaMember()
{
	Id = 0;
	BeginNode = 0;
	EndNode = 0;
	CrossSectionId = 0;
}

NativeFeaMember::NativeFeaMember(int id, int beginNode, int endNode, int cssId)
{
	this->Id = id;
	this->BeginNode = beginNode;
	this->EndNode = endNode;
	this->CrossSectionId = cssId;
}