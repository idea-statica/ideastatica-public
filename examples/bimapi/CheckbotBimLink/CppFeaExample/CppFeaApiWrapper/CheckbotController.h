#pragma once
#include "..\CppFeaApi\NativeFeaApi.h"
#include "ImporterContext.h"

using namespace System;
using namespace Autofac;
using namespace IdeaStatiCa::Plugin;


namespace CppFeaApiWrapper
{
	ref class CheckbotController
	{
	private:
		static CheckbotController^ _instance;
		static NativeFeaApi* pApi;
		static IContainer^ container;

	public:
		static CheckbotController^ Run(String^ checkbotLocation, NativeFeaApi* pFeaApi);

		static IContainer^ BuildContainer(IProgressMessaging^ messagingService, ImporterContext^ feaApi);

		static void RunCheckbot(String^ checkbotLocation, ImporterContext^ context, IPluginLogger^ logger);

		static void RegisterImporters(IdeaStatiCa::BimApiLink::ImportersConfiguration^ config);
	};
}
