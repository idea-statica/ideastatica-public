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

	void CheckbotController::RegisterImporters(IdeaStatiCa::BimApiLink::ImportersConfiguration^ config)
	{
		config->RegisterContainer(gcnew AutofacServiceProvider(container));
	}

	void CheckbotController::RunCheckbot(String^ checkbotLocation, ImporterContext^ context, IPluginLogger^ logger)
	{
		if (logger == nullptr)
		{
			throw gcnew System::ArgumentNullException("logger");
		}

		logger->LogInformation("Starting plugin with checkbot location " + checkbotLocation);

		System::String^ workingDirectory = "c:\\x";

		if (!System::IO::Directory::Exists(workingDirectory))
		{
			logger->LogInformation("Creating a new project dir '" + workingDirectory + "'");
			System::IO::Directory::CreateDirectory(workingDirectory);
		}
		else
		{
			logger->LogInformation("Using an existing project dir '" + workingDirectory + "'");
		}

		try
		{
			GrpcBimHostingFactory^ bimHostingFactory = gcnew GrpcBimHostingFactory();

			IProgressMessaging^ messagingService = bimHostingFactory->InitGrpcClient(logger);

			container = BuildContainer(messagingService, nullptr);

			Model::Model^ model = ResolutionExtensions::Resolve<Model::Model^>(container);

			AutofacServiceProvider^ provider = gcnew AutofacServiceProvider(container);

			Action<ImportersConfiguration^>^ registerImportersAction = nullptr;
			registerImportersAction = gcnew Action<IdeaStatiCa::BimApiLink::ImportersConfiguration^>(&CheckbotController::RegisterImporters);

			BimLink^ bimLink = FeaBimLink::Create("My application name", workingDirectory)
				->WithIdeaStatiCa(checkbotLocation)
				->WithImporters(registerImportersAction)
				->WithLogger(logger)
				->WithBimHostingFactory(bimHostingFactory);

			BimLinkExtension::Run(bimLink, model);

			//FeaBimLink::Create("My application name", workingDirectory)
			//	->WithIdeaStatiCa(checkbotLocation)
			//	->WithImporters(gcnew System::Action<Autofac::ContainerBuilder^>([&](Autofac::ContainerBuilder^ x) { x->RegisterContainer(gcnew Autofac::ServiceProvider(container)); }))
			//	->WithLogger(logger)
			//	->WithBimHostingFactory(bimHostingFactory)
			//	->Run(model);
		}
		catch (System::Exception^ ex)
		{
			logger->LogError("BimApi failed", ex);
			throw;
		}
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


