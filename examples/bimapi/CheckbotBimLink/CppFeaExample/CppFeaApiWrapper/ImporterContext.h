#pragma once
#include "..\CppFeaApi\NativeFeaApi.h"

namespace CppFeaApiWrapper
{
	public ref class ImporterContext
	{
	private:
		NativeFeaApi* pApi;

	public:
		ImporterContext(NativeFeaApi* pApi);

		NativeFeaApi* GetApi();
		NativeFeaGeometry* GetGeometry();
	};
}