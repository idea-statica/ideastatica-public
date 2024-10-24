#include "pch.h"
#include "LoadGroupImporter.h"
#include "..\CppFeaApi\NativeFeaLoads.h"

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		LoadGroupImporter::LoadGroupImporter(ImporterContext^ context)
		{
			this->context = context;
		}

		IIdeaLoadGroup^ LoadGroupImporter::Create(int id)
		{
			throw gcnew System::NotImplementedException();
		}
	}
}
