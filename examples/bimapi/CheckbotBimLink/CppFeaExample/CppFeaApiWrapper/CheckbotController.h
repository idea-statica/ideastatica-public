#pragma once

#include "..\CppFeaApi\NativeFeaApi.h"


namespace CppFeaApiWrapper
{
	ref class CheckbotController
	{
	private:
		static CheckbotController^ _instance;
		NativeFeaApi* pApi;

	public:
		static CheckbotController^ Run(NativeFeaApi* pFeaApi);
	};
}
