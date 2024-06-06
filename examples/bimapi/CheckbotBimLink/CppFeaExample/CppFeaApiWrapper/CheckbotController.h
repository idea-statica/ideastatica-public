#pragma once
#include "..\CppFeaApi\NativeFeaApi.h"
#include "ImporterContext.h"

using namespace System;
using namespace Autofac;
using namespace IdeaStatiCa::Plugin;


namespace CppFeaApiWrapper
{
	/// <summary>
	/// Responsible for running the Checkbot
	/// </summary>
	ref class CheckbotController 
	{
	private:
		static CheckbotController^ _instance;
		IContainer^ container;
		ImporterContext^ context;
		IdeaStatiCa::BimApiLink::BimLink^ bimLink;
		bool disposed; // to detect redundant calls

		void RunCheckbot(String^ checkbotLocation, ImporterContext^ context, IPluginLogger^ logger);

		/// <summary>
		/// Registration of importers which are responsible for converion FEA instances to BIM API instances
		/// </summary>
		/// <param name="messagingService">Callback fonction for interaction with Checkbot</param>
		/// <param name="context">Import context - it contains the reference to NativeFeaApi</param>
		/// <returns>Autofac container</returns>
		static IContainer^ BuildContainer(IProgressMessaging^ messagingService, ImporterContext^ context);

		void RegisterImporters(IdeaStatiCa::BimApiLink::ImportersConfiguration^ config);

	public:
		CheckbotController();
		~CheckbotController();

		/// <summary>
		/// Function to run the Checkbot
		/// </summary>
		/// <param name="checkbotLocation">Path to Checkbot.ex</param>
		/// <param name="pFeaApi">NativeFeaApi represents the model from the source FEA</param>
		/// <returns>The instance of CheckbotController</returns>
		static CheckbotController^ Run(String^ checkbotLocation, NativeFeaApi* pFeaApi);

		static void Release();

	};
}
