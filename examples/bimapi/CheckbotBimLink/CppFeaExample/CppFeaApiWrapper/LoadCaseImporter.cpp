#include "pch.h"
#include "LoadCaseImporter.h"
#include "..\CppFeaApi\NativeFeaLoads.h"

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		LoadCaseImporter::LoadCaseImporter(ImporterContext^ context)
		{
			this->context = context;
		}

		IIdeaLoadCase^ LoadCaseImporter::Create(int id)
		{
			auto feaLoadCase = this->context->GetApi()->GetLoading()->GetLoadCase(id);

			auto loadType = GetLoadType(feaLoadCase->LoadCaseType);

			IdeaLoadCase^ ideaLoadCase = gcnew IdeaLoadCase(id);

			ideaLoadCase->Name = gcnew String(feaLoadCase->Name.c_str());
			ideaLoadCase->LoadGroup = Get<IIdeaLoadGroup^>(feaLoadCase->LoadGroupId);
			ideaLoadCase->LoadType = loadType->Item1;
			ideaLoadCase->Type = loadType->Item2;
			ideaLoadCase->Variable = VariableType::Standard;

			return ideaLoadCase;
		}

		Tuple<LoadCaseType, LoadCaseSubType>^ LoadCaseImporter::GetLoadType(int typeOfLoadCase)
		{
			switch (typeOfLoadCase) {
			case NativeFeaLoads::LoadCase_Type_Selfweight:
				return gcnew Tuple<LoadCaseType, LoadCaseSubType>(IdeaRS::OpenModel::Loading::LoadCaseType::Permanent, IdeaRS::OpenModel::Loading::LoadCaseSubType::PermanentSelfweight);
			case NativeFeaLoads::LoadCase_Type_Dead:
				return gcnew Tuple<LoadCaseType, LoadCaseSubType>(IdeaRS::OpenModel::Loading::LoadCaseType::Permanent, IdeaRS::OpenModel::Loading::LoadCaseSubType::PermanentStandard);
			case NativeFeaLoads::LoadCase_Type_Snow:
				return gcnew Tuple<LoadCaseType, LoadCaseSubType>(IdeaRS::OpenModel::Loading::LoadCaseType::Variable, IdeaRS::OpenModel::Loading::LoadCaseSubType::VariableStatic);
			default:
				return gcnew Tuple<LoadCaseType, LoadCaseSubType>(IdeaRS::OpenModel::Loading::LoadCaseType::Permanent, IdeaRS::OpenModel::Loading::LoadCaseSubType::PermanentStandard);
			}
		}
	}
}
