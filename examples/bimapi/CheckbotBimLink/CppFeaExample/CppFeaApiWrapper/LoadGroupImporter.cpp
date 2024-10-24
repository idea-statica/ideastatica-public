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
			auto loadGroup = this->context->GetApi()->GetLoading()->GetLoadGroup(id);

			IdeaLoadGroup^ ideaLoadGroup = gcnew IdeaLoadGroup(id);
			ideaLoadGroup->Name = gcnew String(loadGroup->Name.c_str());
			ideaLoadGroup->GroupType = GetLoadGroupType(loadGroup->LoadGroupCategory);

			return ideaLoadGroup;
		}

		IdeaRS::OpenModel::Loading::LoadGroupType LoadGroupImporter::GetLoadGroupType(int loadGroupCategory)
		{
			switch (loadGroupCategory) {
			case NativeFeaLoads::LoadGroup_Category_Permanent:
				return IdeaRS::OpenModel::Loading::LoadGroupType::Permanent;
			case NativeFeaLoads::LoadGroup_Category_Variable:
				return IdeaRS::OpenModel::Loading::LoadGroupType::Variable;
			default:
				return IdeaRS::OpenModel::Loading::LoadGroupType::Permanent;
			}
		}
	}
}
