#include "pch.h"
#include "NativeFeaAppGateway.h"

namespace CheckbotClient
{
	NativeFeaAppGateway::NativeFeaAppGateway(SafProviderBase* pApi)
	{
		_pApi = pApi;
	}

	System::String^ NativeFeaAppGateway::GetModelDirectory()
	{
		throw gcnew System::NotImplementedException();
		// TODO: insert return statement here
	}

	System::String^ NativeFeaAppGateway::GetModelName()
	{
		throw gcnew System::NotImplementedException();
		// TODO: insert return statement here
	}

	System::String^ NativeFeaAppGateway::ExportSAFFileofActiveSelection(System::String^ safSavePath, System::Collections::Generic::IReadOnlyCollection<System::Guid>^% selectedElementGuids)
	{
		throw gcnew System::NotImplementedException();
		// TODO: insert return statement here
	}

	System::String^ NativeFeaAppGateway::ExportSAFFileofProvidedSelection(System::String^ safSavePath, System::Collections::Generic::IEnumerable<System::Guid>^ providedElementGuids)
	{
		throw gcnew System::NotImplementedException();
		// TODO: insert return statement here
	}
}