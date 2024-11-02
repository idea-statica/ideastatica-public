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

	NativeFeaLoads* ImporterContext::GetLoading()
	{
		return pApi->GetLoading();
	}

	NativeFeaResults* ImporterContext::GetResults()
	{
		return pApi->GetResults();
	}
}