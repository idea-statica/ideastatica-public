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
			auto feaLoadCombi = this->context->GetLoading()->GetLoadCombination(id);

			auto ideaCombi = gcnew IdeaCombiInput(id);
			ideaCombi->Name = gcnew String(feaLoadCombi->Name.c_str());
			ideaCombi->TypeCombiEC = GetCombiAction(feaLoadCombi->Category);
			ideaCombi->TypeCalculationCombi = GetCalculationType(feaLoadCombi->Type);
			ideaCombi->CombiItems = GetCombiInputs(feaLoadCombi->CombiFactors, id);

			return ideaCombi;
		}

		List<IIdeaCombiItem^>^ LoadCombiImporter::GetCombiInputs(std::vector<NativeFeaCombiFactor>* loadFactors, int combiId)
		{
			List<IIdeaCombiItem^>^ combiItems = gcnew List<IIdeaCombiItem^>();

			for (auto item : *loadFactors)
			{
				String^ itemName = String::Format("Combi {0}_{1}", combiId, item.LoadCaseId);
				IdeaCombiItem^ combiItem = gcnew IdeaCombiItem(itemName);
				combiItem->Coeff = item.CombiMultiplier;
				combiItem->LoadCase = GetMaybe<IIdeaLoadCase^>(item.LoadCaseId);
				combiItems->Add(combiItem);
			}

			return combiItems;
		}

		IdeaRS::OpenModel::Loading::TypeCalculationCombiEC LoadCombiImporter::GetCalculationType(int loadCombiType)
		{
			switch (loadCombiType)
			{
				case NativeFeaLoads::Combi_Type_Linear:
					return TypeCalculationCombiEC::Linear;

				default:
					return TypeCalculationCombiEC::Linear;
			}
		}

		IdeaRS::OpenModel::Loading::TypeOfCombiEC LoadCombiImporter::GetCombiAction(int combiCategory)
		{
			switch (combiCategory)
			{
			case NativeFeaLoads::Combi_Category_ULS:
				return TypeOfCombiEC::ULS;
			case NativeFeaLoads::Combi_Category_SLS:
				return TypeOfCombiEC::SLS_Char;
			default:
				return TypeOfCombiEC::ULS;
			}
		}
	}
}
