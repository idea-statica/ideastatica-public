#include "pch.h"
#include "ImporterContext.h"

namespace CppFeaApiWrapper
{
	ImporterContext::ImporterContext(NativeFeaApi* pApi)
	{
		this->pApi = pApi;
	}

	NativeFeaApi* ImporterContext::GetApi()
	{
		return pApi;
	}

	NativeFeaGeometry* ImporterContext::GetGeometry()
	{
		return pApi->GetGeometry();
	}
}