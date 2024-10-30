#include "pch.h"
#include "ResultsImporter.h"

using namespace MathNet::Numerics;


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

			List<IntIdentifier<IIdeaLoadCase^>^>^ loadCaseIdentifiers = gcnew List<IntIdentifier<IIdeaLoadCase^>^>();

			for (const auto& lcId : allLcIDs)
			{
				loadCaseIdentifiers->Add(gcnew IntIdentifier<IIdeaLoadCase^>(lcId));
			}

			InternalForcesBuilder<IIdeaMember1D^>^ builder = gcnew InternalForcesBuilder<IIdeaMember1D^>(IdeaRS::OpenModel::Result::ResultLocalSystemType::Local);
			// Iterate through all members
			for each (IIdeaMember1D ^ member in members)
			{
				IdeaMember1D^ ideaMember = safe_cast<IdeaMember1D^>(member);
				auto idObj = ideaMember->Identifier->GetId();

				int memId = safe_cast<int>(idObj);

				for each (auto lcIdentifier in loadCaseIdentifiers)
				{
					int lcId = safe_cast<int>(lcIdentifier->GetId());
					auto sections = builder->For(member, lcIdentifier);
					SetResultsForMember(ideaMember, lcIdentifier, sections);
				}
			}

			return builder;
		}

		void ResultsImporter::SetResultsForMember(IdeaMember1D^ member,
			IntIdentifier<IIdeaLoadCase^>^ loadCase,
			InternalForcesBuilder<IIdeaMember1D^>::Sections^ sections)
		{
			auto  results = pFeaResults->GetMemberInternalForces((int)member->Identifier->GetId(), loadCase->Id);

			if (results.size() < 1)
			{
				// no results
				return;
			}

			int elementCount = member->Elements->Count;

			double memberLength = GetLength(member->Elements[0]->Segment->StartNode, member->Elements[elementCount - 1]->Segment->EndNode);

			double currentSectionPos =  Math::Max(0.0, results[0]->Location / memberLength);

			for (int i = 1; i < results.size(); ++i)
			{
				auto result = results[i-1];
				double nextSectionPos = Math::Min(1.0, results[i]->Location / memberLength);

				// In source application there can be 2 results with the same location
				// (e.g. local load force in node). BimApi is not able to handle that,
				// so as workaround it is necessary to shift results a bit.
				
				if (Precision::AlmostEqual(currentSectionPos, nextSectionPos, resultSectionPositionPrecision))
				{
					if (i == 1)
					{
						// first section should be always 0, move next section only
						nextSectionPos += shift;
					}
					else if (i == results.size() - 1)
					{
						// last section should be always 1, move section before last only
						currentSectionPos -= shift;
					}
					else
					{
						currentSectionPos -= shift / 2.0;
						nextSectionPos += shift / 2.0;
					}
				}

				AddResult(sections, currentSectionPos, results[i - 1]);
				currentSectionPos = nextSectionPos;
			}

			// add the last result
			AddResult(sections, currentSectionPos, results[results.size() -1]);
		}

		double ResultsImporter::GetLength(IIdeaNode^ startNode, IIdeaNode^ endNode)
		{
			IdeaVector3D^ delta = gcnew IdeaVector3D(
				startNode->Vector->X - endNode->Vector->X,
				startNode->Vector->Y - endNode->Vector->Y,
				startNode->Vector->Z - endNode->Vector->Z);

			return Math::Sqrt((delta->X * delta->X) + (delta->Y * delta->Y) + (delta->Z * delta->Z));
		}

		void ResultsImporter::AddResult(InternalForcesBuilder<IIdeaMember1D^>::Sections^ sections, double location, NativeFeaMemberResult* pMemberResult)
		{
				// Convert source results to correct units
				// and correct coordinate system if necessary
				double N = pMemberResult->N * unitConversionFactor;
				double Vy = pMemberResult->Vy * unitConversionFactor;
				double Vz = pMemberResult->Vz * unitConversionFactor;
				double Tx = pMemberResult->Mx * unitConversionFactor;
				double My = pMemberResult->My * unitConversionFactor;
				double Mz = pMemberResult->Mz * unitConversionFactor;
				sections->Add(location, N, Vy, Vz, Tx, My, Mz);
		}
	}
}