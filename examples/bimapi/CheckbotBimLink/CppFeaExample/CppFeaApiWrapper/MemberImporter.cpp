#include "pch.h"
#include "MemberImporter.h"

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		MemberImporter::MemberImporter(ImporterContext^ context)
		{
			this->context = context;
		}

		IIdeaMember1D^ MemberImporter::Create(int id)
		{
			NativeFeaMember* feaMember = context->GetGeometry()->GetMember(id);

			return nullptr;
		}
	}
}