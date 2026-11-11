#include "CheckbotLink.h"

#include "Importers/CombinationImporter.h"
#include "Importers/CrossSectionImporter.h"
#include "Importers/LoadCaseImporter.h"
#include "Importers/LoadGroupImporter.h"
#include "Importers/MaterialImporter.h"
#include "Importers/MemberImporter.h"
#include "Importers/NodeImporter.h"
#include "Importers/ResultsImporter.h"
#include "Model.h"

#include <msclr/marshal_cppstd.h>

using namespace BimApiLinkCppFeaExample::Importers;
using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink;
using namespace IdeaStatiCa::Plugin;
using namespace IdeaStatiCa::PluginLogger;
using namespace System;
using namespace System::IO;
using namespace System::Threading::Tasks;

namespace BimApiLinkCppFeaExample
{
	/// <summary>
	/// Starts IDEA StatiCa Checkbot linked to the example FEA application.
	/// </summary>
	ref class CheckbotLink sealed
	{
	public:
		CheckbotLink(const CppFeaExampleApi::IFeaApi& fea, IPluginLogger^ logger)
			: _fea(&fea)
			, _logger(logger)
		{
		}

		/// <summary>
		/// Opens Checkbot, which then asks the link for the model until the user closes it. The returned task ends
		/// when Checkbot is closed.
		/// </summary>
		Task^ Run(String^ checkbotPath, String^ projectPath)
		{
			Directory::CreateDirectory(projectPath);
			_logger->LogInformation(String::Format("Starting '{0}' with the project in '{1}'", checkbotPath, projectPath), nullptr);

			BimLink^ link = FeaBimLink::Create(ApplicationName, projectPath)
				->WithIdeaStatiCa(checkbotPath)
				->WithImporters(gcnew Action<ImportersConfiguration^>(this, &CheckbotLink::RegisterImporters))
				->WithResultsImporters(gcnew Action<ResultsImportersConfiguration^>(this, &CheckbotLink::RegisterResultsImporter))
				->WithLogger(_logger);

			Task^ checkbot = BimLinkExtension::Run(link, gcnew Model(_fea->Geometry(), _fea->Loads()));
			checkbot->ContinueWith(gcnew Action<Task^>(this, &CheckbotLink::LogFailure), TaskContinuationOptions::OnlyOnFaulted);
			return checkbot;
		}

	private:
		/// <summary>
		/// The name Checkbot shows for the linked application.
		/// </summary>
		literal String^ ApplicationName = "C++ FEA Example";

		/// <summary>
		/// Checkbot asks for every object by its identifier, and BimApiLink passes the request to the importer of that
		/// kind of object. One importer serves all objects of its kind.
		/// </summary>
		void RegisterImporters(ImportersConfiguration^ importers)
		{
			importers
				->RegisterImporter<IIdeaCombiInput^>(gcnew CombinationImporter(_fea->Loads()))
				->RegisterImporter<IIdeaCrossSection^>(gcnew CrossSectionImporter(_fea->Geometry()))
				->RegisterImporter<IIdeaLoadCase^>(gcnew LoadCaseImporter(_fea->Loads()))
				->RegisterImporter<IIdeaLoadGroup^>(gcnew LoadGroupImporter(_fea->Loads()))
				->RegisterImporter<IIdeaMaterial^>(gcnew MaterialImporter(_fea->Geometry()))
				->RegisterImporter<IIdeaMember1D^>(gcnew MemberImporter(_fea->Geometry()))
				->RegisterImporter<IIdeaNode^>(gcnew NodeImporter(_fea->Geometry()));
		}

		void RegisterResultsImporter(ResultsImportersConfiguration^ results)
		{
			results->RegisterImporter<IIdeaMember1D^>(gcnew ResultsImporter(*_fea));
		}

		void LogFailure(Task^ checkbot)
		{
			_logger->LogError("The C++ FEA example link failed", checkbot->Exception);
		}

		const CppFeaExampleApi::IFeaApi* _fea;
		IPluginLogger^ _logger;
	};

	/// <summary>
	/// What StartCheckbot keeps from one call to the next.
	/// </summary>
	ref class LinkState abstract sealed
	{
	public:
		static IPluginLogger^ Logger;

		/// <summary>
		/// Ends when the Checkbot started last is closed.
		/// </summary>
		static Task^ Checkbot;
	};

	std::wstring StartCheckbot(const std::wstring& checkbotPath, const std::wstring& projectPath, const CppFeaExampleApi::IFeaApi& fea)
	{
		try
		{
			if (LinkState::Checkbot != nullptr && !LinkState::Checkbot->IsCompleted)
			{
				return L"Checkbot is already running. Close it before you start it again.";
			}

			if (LinkState::Logger == nullptr)
			{
				SerilogFacade::Initialize("BimApiLinkCppFeaExample.log");
				LinkState::Logger = LoggerProvider::GetLogger("BimApiLinkCppFeaExample");
			}

			CheckbotLink^ link = gcnew CheckbotLink(fea, LinkState::Logger);
			LinkState::Checkbot = link->Run(gcnew String(checkbotPath.c_str()), gcnew String(projectPath.c_str()));
			return std::wstring();
		}
		catch (Exception^ ex)
		{
			return msclr::interop::marshal_as<std::wstring>(ex->Message);
		}
	}
}
