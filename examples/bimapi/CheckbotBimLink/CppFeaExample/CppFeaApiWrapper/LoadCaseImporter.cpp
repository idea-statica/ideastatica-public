#include "pch.h"
#include "LoadCaseImporter.h"

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
			throw gcnew System::NotImplementedException();
		}

		std::pair<IdeaRS.OpenModel.Loading.LoadCaseType, LoadCaseSubType> GetLoadType(TypeOfLoadCase typeOfLoadCase) {
			switch (typeOfLoadCase) {
			case TypeOfLoadCase::Selfweight:
				return { LoadCaseType::Permanent, LoadCaseSubType::PermanentSelfweight };
			case TypeOfLoadCase::DeadLoad:
				return { LoadCaseType::Permanent, LoadCaseSubType::PermanentStandard };
			case TypeOfLoadCase::Snow:
				return { LoadCaseType::Variable, LoadCaseSubType::VariableStatic };
			default:
				return { LoadCaseType::Permanent, LoadCaseSubType::PermanentStandard };
			}

	}
}
