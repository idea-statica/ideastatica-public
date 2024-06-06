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

/// <summary>
/// Function to run the checkbot
/// </summary>
/// <param name="pApi">NativeFeaApi represents the model from the source FEA</param>
/// <param name="checkBotPath">The path to Checkbot.exe (IDEA StatiCa setup)</param>
/// <returns>Returns 1 if success</returns>
extern "C" __declspec(dllexport) int RunCheckbot(NativeFeaApi * pApi, std::wstring checkBotPath)
{
	try
	{
		String^ checkbotPath = gcnew System::String(checkBotPath.c_str());

		CppFeaApiWrapper::CheckbotController::Run(checkbotPath, pApi);
		return 1;
	}
	catch (System::Exception^ ex)
	{

		return 0;
	}
}

/// <summary>
/// Release Checkbot
/// </summary>
/// <returns></returns>
extern "C" __declspec(dllexport) int ReleaseCheckbot()
{
	try
	{
		CppFeaApiWrapper::CheckbotController::Release();
		return 1;
	}
	catch (System::Exception^ ex)
	{

		return 0;
	}
}

namespace CppFeaApiWrapper
{
	/// <summary>
	/// Constructor
	/// </summary>
	CheckbotController::CheckbotController()
	{
	}

	/// <summary>
	/// Destructor
	/// </summary>
	CheckbotController::~CheckbotController()
	{
	}

	/// <summary>
	/// Create an instance of CheckbotController and run the Checkbot
	/// </summary>
	/// <param name="checkbotLocation"></param>
	/// <param name="pFeaApi"></param>
	/// <returns></returns>
	CheckbotController^ CheckbotController::Run(String^ checkbotLocation, NativeFeaApi* pFeaApi)
	{
		if (_instance != nullptr)
		{
			throw gcnew System::Exception("CheckbotController is already running");
		}

		ImporterContext^ context = gcnew ImporterContext(pFeaApi);

		_instance = gcnew CheckbotController();
		_instance->RunCheckbot(checkbotLocation, context, nullptr);

		return _instance;
	}

	void CheckbotController::Release()
	{
		if (_instance != nullptr)
		{
			delete _instance;
			_instance = nullptr;
		}
	}

	void CheckbotController::RegisterImporters(IdeaStatiCa::BimApiLink::ImportersConfiguration^ config)
	{
		config->RegisterContainer(gcnew AutofacServiceProvider(container));
	}

	void CheckbotController::RunCheckbot(String^ checkbotLocation, ImporterContext^ context, IPluginLogger^ logger)
	{
		this->context = context;

		if(logger == nullptr)
		{
			logger = gcnew NullLogger();
		}

		logger->LogInformation(String::Format("Starting plugin with checkbot location {0}", checkbotLocation));

		// get project directory
		System::String^ workingDirectory = gcnew System::String(context->GetApi()->GetProjectPath().c_str());

		// create it on the disk if it does not exist
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

			// register importers to container
			container = BuildContainer(messagingService, context);

			Model::Model^ model = ResolutionExtensions::Resolve<Model::Model^>(container);

			AutofacServiceProvider^ provider = gcnew AutofacServiceProvider(container);

			Action<ImportersConfiguration^>^ registerImportersAction = nullptr;
			registerImportersAction = gcnew Action<IdeaStatiCa::BimApiLink::ImportersConfiguration^>(this, &CheckbotController::RegisterImporters);

			// Name of the FEA application which calls Checkbot
			String^ feaName = gcnew System::String(context->GetApi()->GetFeaName().c_str());

			// create BimLink
			this->bimLink = FeaBimLink::Create(feaName, workingDirectory)
				->WithIdeaStatiCa(checkbotLocation)
				->WithImporters(registerImportersAction)
				->WithLogger(logger)
				->WithBimHostingFactory(bimHostingFactory);

			// start Checkbot
			BimLinkExtension::Run(this->bimLink, model);
		}
		catch (System::Exception^ ex)
		{
			logger->LogError("BimApi failed", ex);
			throw;
		}
	}

	/// <summary>
	/// Registration of importers which are responsible for converion FEA instances to BIM API instances
	/// </summary>
	/// <param name="messagingService">Callback fonction for interaction with Checkbot</param>
	/// <param name="context">Import context - it contains the reference to NativeFeaApi</param>
	/// <returns>Autofac container</returns>
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


