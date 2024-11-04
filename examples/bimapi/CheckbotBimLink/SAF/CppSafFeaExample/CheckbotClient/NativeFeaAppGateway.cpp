#include "pch.h"
#include "NativeFeaAppGateway.h"
#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>

namespace CheckbotClient
{
	NativeFeaAppGateway::NativeFeaAppGateway(SafProviderBase* pApi)
	{
		_pApi = pApi;
	}

	System::String^ NativeFeaAppGateway::GetModelDirectory()
	{
		return gcnew System::String(_pApi->GetProjectPath().c_str());
	}

	System::String^ NativeFeaAppGateway::GetModelName()
	{
		return gcnew System::String(_pApi->GetProjectName().c_str());
	}

	System::String^ NativeFeaAppGateway::ExportSAFFileofActiveSelection(System::String^ safSavePath, System::Collections::Generic::IReadOnlyCollection<System::Guid>^% selectedElementGuids)
	{
		_pApi->ExportToSafFile(msclr::interop::marshal_as<std::wstring>(safSavePath));
		return safSavePath;
	}

	System::String^ NativeFeaAppGateway::ExportSAFFileofProvidedSelection(System::String^ safSavePath, System::Collections::Generic::IEnumerable<System::Guid>^ providedElementGuids)
	{
		throw gcnew System::NotImplementedException();
		// TODO: insert return statement here
	}
}