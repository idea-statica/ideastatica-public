#include "pch.h"
#include "ImporterContext.h"

namespace CppFeaApiWrapper
{
	ImporterContext::ImporterContext(NativeFeaApi* pApi)
	{
		this->pApi = pApi;
	}
}