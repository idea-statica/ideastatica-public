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
		IContainer^ container;
		ImporterContext^ context;
		IdeaStatiCa::BimApiLink::BimLink^ bimLink;
		bool disposed; // to detect redundant calls

		void RunCheckbot(String^ checkbotLocation, ImporterContext^ context, IPluginLogger^ logger);
		void RegisterImporters(IdeaStatiCa::BimApiLink::ImportersConfiguration^ config);
		static IContainer^ BuildContainer(IProgressMessaging^ messagingService, ImporterContext^ feaApi);

	public:
		CheckbotController();
		~CheckbotController();

		static CheckbotController^ Run(String^ checkbotLocation, NativeFeaApi* pFeaApi);
		static void Release();

	};
}
