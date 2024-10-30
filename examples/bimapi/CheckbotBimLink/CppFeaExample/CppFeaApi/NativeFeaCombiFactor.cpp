#include "pch.h"
#include "NativeFeaCombiFactor.h"

NativeFeaCombiFactor::NativeFeaCombiFactor()
{
	this->CombiMultiplier = 1.0;
	this->LoadCaseId = 0;
}

NativeFeaCombiFactor::NativeFeaCombiFactor(int loadCaseId, double combiMultiplier)
{
	this->LoadCaseId = loadCaseId;
	this->CombiMultiplier = combiMultiplier;
}