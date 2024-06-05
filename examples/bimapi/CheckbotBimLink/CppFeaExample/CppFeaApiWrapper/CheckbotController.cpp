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

extern "C" __declspec(dllexport) int RunCheckbot(NativeFeaApi * pApi, std::wstring checkBotPath)
{
	String^ checkbotPath = gcnew System::String(checkBotPath.c_str());

	CppFeaApiWrapper::CheckbotController::Run(checkbotPath, pApi);
  return 1;
}

namespace CppFeaApiWrapper
{
	CheckbotController^ CheckbotController::Run(String^ checkbotLocation, NativeFeaApi* pFeaApi)
	{
		if (_instance != nullptr)
		{
			throw gcnew System::Exception("CheckbotController is already running");
		}

		_instance = gcnew CheckbotController();
		_instance->pApi = pFeaApi;

		ImporterContext^ context = gcnew ImporterContext(pApi);

		CheckbotController::RunCheckbot(checkbotLocation, context, nullptr);

		return _instance;
	}

	void CheckbotController::RegisterImporters(IdeaStatiCa::BimApiLink::ImportersConfiguration^ config)
	{
		config->RegisterContainer(gcnew AutofacServiceProvider(container));
	}

	void CheckbotController::RunCheckbot(String^ checkbotLocation, ImporterContext^ context, IPluginLogger^ logger)
	{
		if(logger == nullptr)
		{
			logger = gcnew NullLogger();
		}

		logger->LogInformation(String::Format("Starting plugin with checkbot location {0}", checkbotLocation));

		System::String^ workingDirectory = "c:\\x";

		if (!System::IO::Directory::Exists(workingDirectory))
		{
			logger->LogInformation(String::Format("Creating a new project dir '{0}'", workingDirectory));
			System::IO::Directory::CreateDirectory(workingDirectory);
		}
		else
		{
			logger->LogInformation(String::Format("Using an existing project dir '{0}'", workingDirectory));
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


