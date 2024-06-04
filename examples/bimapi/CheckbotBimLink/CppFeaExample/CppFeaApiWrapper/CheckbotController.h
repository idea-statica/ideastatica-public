#pragma once
#include "..\CppFeaApi\NativeFeaApi.h"
#include "ImporterContext.h"

using namespace Autofac;
using namespace IdeaStatiCa::Plugin;

namespace CppFeaApiWrapper
{
	ref class CheckbotController
	{
	private:
		static CheckbotController^ _instance;
		NativeFeaApi* pApi;

	public:
		static CheckbotController^ Run(NativeFeaApi* pFeaApi);

		static IContainer^ BuildContainer(IProgressMessaging^ messagingService, ImporterContext^ feaApi);
	};
}
