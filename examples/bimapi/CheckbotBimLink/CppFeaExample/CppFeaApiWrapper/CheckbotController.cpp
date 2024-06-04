#include "pch.h"
#include "CheckbotController.h"
#include "..\CppFeaApi\NativeFeaApi.h"
#include "CheckBotControlFunctions.h"
#include "CrossSectionImporter.h"
#include "MaterialImporter.h"
#include "MemberImporter.h"
#include "NodeImporter.h"
#include "Model.h"
using namespace CppFeaApiWrapper::Importers;

using namespace Autofac;
using namespace Autofac::Extensions::DependencyInjection;

extern "C" __declspec(dllexport) int RunCheckbot(NativeFeaApi * pApi)
{
	CppFeaApiWrapper::CheckbotController::Run(pApi);
  return 1;
}

namespace CppFeaApiWrapper
{
	CheckbotController^ CheckbotController::Run(NativeFeaApi* pFeaApi)
	{
		if (_instance != nullptr)
		{
			throw gcnew System::Exception("CheckbotController is already running");
		}

		_instance = gcnew CheckbotController();
		_instance->pApi = pFeaApi;
		return _instance;
	}

	IContainer^ CheckbotController::BuildContainer(IProgressMessaging^ messagingService, ImporterContext^ context)
	{
		ContainerBuilder^ builder = gcnew ContainerBuilder();

		RegistrationExtensions::RegisterInstance<ImporterContext^>(builder, context);
		RegistrationExtensions::RegisterInstance<IProgressMessaging^>(builder, messagingService);


		// Register importers
		 RegistrationExtensions::AsImplementedInterfaces(RegistrationExtensions::RegisterType<CrossSectionImporter^>(builder))->SingleInstance();
		 RegistrationExtensions::AsImplementedInterfaces(RegistrationExtensions::RegisterType<MaterialImporter^>(builder))->SingleInstance();
		 RegistrationExtensions::AsImplementedInterfaces(RegistrationExtensions::RegisterType<MemberImporter^>(builder))->SingleInstance();
		 RegistrationExtensions::AsImplementedInterfaces(RegistrationExtensions::RegisterType<NodeImporter^>(builder))->SingleInstance();

		 RegistrationExtensions::RegisterType<Model::Model^>(builder)->SingleInstance();

		IContainer^ container = builder->Build();
		return container;
	}
}


