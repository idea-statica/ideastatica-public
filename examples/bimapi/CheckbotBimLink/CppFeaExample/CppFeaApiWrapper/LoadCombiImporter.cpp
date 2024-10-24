#include "pch.h"
#include "LoadCombiImporter.h"
#include "..\CppFeaApi\NativeFeaLoads.h"

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		LoadCombiImporter::LoadCombiImporter(ImporterContext^ context)
		{
			this->context = context;
		}

		IIdeaCombiInput^ LoadCombiImporter::Create(int id)
		{
			throw gcnew System::NotImplementedException();
		}
	}
}
