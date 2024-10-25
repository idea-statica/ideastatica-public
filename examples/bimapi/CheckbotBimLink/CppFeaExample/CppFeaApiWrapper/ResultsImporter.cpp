#include "pch.h"
#include "ResultsImporter.h"

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		ResultsImporter::ResultsImporter(ImporterContext^ context)
		{
			this->context = context;
			pFeaResults = this->context->GetResults();
			pFeaLoading = this->context->GetLoading();

		}

		System::Collections::Generic::IEnumerable<ResultsData<IIdeaMember1D^>^>^ ResultsImporter::GetResults(IReadOnlyList<IIdeaMember1D^>^ members)
		{
			auto allLcIDs = pFeaLoading->GetLoadCasesIdentifiers();

			List<Identifier<IIdeaLoadCase^>^>^ loadCaseIdentifiers = gcnew List<Identifier<IIdeaLoadCase^>^>();

			for (const auto& lcId : allLcIDs)
			{
				loadCaseIdentifiers->Add(gcnew IntIdentifier<IIdeaLoadCase^>(lcId));
			}

			InternalForcesBuilder<IIdeaMember1D^>^ builder = gcnew InternalForcesBuilder<IIdeaMember1D^>(IdeaRS::OpenModel::Result::ResultLocalSystemType::Local);

			return builder;
		}

		//IdeaVector3D^ NodeImporter::GetLocation(int id)
		//{
		//	NativeFeaNode* feaNode = this->context->GetGeometry()->GetNode(id);
		//	IdeaStatiCa::BimApi::IdeaVector3D^ vect = gcnew IdeaVector3D(feaNode->X, feaNode->Y, feaNode->Z);
		//	return vect;
		//}

		//IIdeaNode^ NodeImporter::Create(int id)
		//{
		//	IdeaVector3D^ v = GetLocation(id);
		//	IdeaNode^ node = gcnew IdeaNode(id);
		//	node->Vector = v;
		//	return node;
		//}
	}
}