#pragma once
#include "..\SafProvider\NativeFeaApi.h"

namespace CppFeaApiWrapper
{
	public ref class ImporterContext
	{
	private:
		NativeFeaApi* pApi;

	public:
		ImporterContext(NativeFeaApi* pApi);


	};
}