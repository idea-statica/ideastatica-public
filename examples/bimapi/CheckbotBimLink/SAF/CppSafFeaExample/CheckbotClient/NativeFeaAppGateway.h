#pragma once
#include "..\SafProvider\SafProviderBase.h"

using namespace SafFeaBimLink;

namespace CheckbotClient
{
	public ref class  NativeFeaAppGateway : ISafDataSource
	{
	private:
		SafProviderBase* _pApi;

	public:
		NativeFeaAppGateway(SafProviderBase* pApi);


		// Inherited via ISafDataSource
		virtual System::String^ GetModelDirectory();
		virtual System::String^ GetModelName();
		virtual System::String^ ExportSAFFileofActiveSelection(System::String^ safSavePath, System::Collections::Generic::IReadOnlyCollection<System::Guid>^% selectedElementGuids);
		virtual System::String^ ExportSAFFileofProvidedSelection(System::String^ safSavePath, System::Collections::Generic::IEnumerable<System::Guid>^ providedElementGuids);
	};
}