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
			// Iterate through all members
			for each (IIdeaMember1D ^ member in members)
			{
				for each (auto lcId in loadCaseIdentifiers)
				{
					auto sections = builder->For(member, lcId);

				}
			}

			return builder;
		}


		//private static void GetResultsForMember(InternalForcesBuilder<IIdeaMember1D>.Sections sections,
		//	IdeaMember1D member, IntIdentifier<IIdeaLoadCase> loadCase,
		//	IFeaResultsApi resultsApi)
		//{
		//	var results = resultsApi.GetMemberInternalForces((int)member.Identifier.GetId(), loadCase.Id);
		//	var memberLength = GetLength(member.Elements.First().Segment.StartNode, member.Elements.Last().Segment.EndNode);

		//	double currentSection = Math.Max(0, results.ElementAt(0).Location / memberLength);

		//	for (var i = 1; i < results.Count(); ++i)
		//	{
		//		IFeaMemberResult result = results.ElementAt(i - 1);
		//		double nextSection = Math.Min(1, results.ElementAt(i).Location / memberLength);

		//		// In source application there can be 2 results with the same location
		//		// (e.g. local load force in node). BimApi is not able to handle that,
		//		// so as workaround it is necessary to shift results a bit.
		//		if (currentSection.AlmostEqual(nextSection, resultSectionPositionPrecision))
		//		{

		//			if (i == 1)
		//			{
		//				// first section should be always 0, move next section only
		//				nextSection += shift;
		//			}
		//			else if (i == results.Count() - 1)
		//			{
		//				// last section should be always 1, move section before last only
		//				currentSection -= shift;
		//			}
		//			else
		//			{
		//				currentSection -= shift / 2.0;
		//				nextSection += shift / 2.0;
		//			}
		//		}

		//		AddResult(sections, currentSection, result);
		//		currentSection = nextSection;
		//	}

		//	// add the very last result
		//	AddResult(sections, currentSection, results.Last());
		//}

		double ResultsImporter::GetLength(IIdeaNode^ startNode, IIdeaNode^ endNode)
		{
			IdeaVector3D^ delta = gcnew IdeaVector3D(
				startNode->Vector->X - endNode->Vector->X,
				startNode->Vector->Y - endNode->Vector->Y,
				startNode->Vector->Z - endNode->Vector->Z);

			return Math::Sqrt((delta->X * delta->X) + (delta->Y * delta->Y) + (delta->Z * delta->Z));
		}

		void ResultsImporter::AddResult(InternalForcesBuilder<IIdeaMember1D^>::Sections sections, double location, NativeFeaMemberResult* pMemberResult)
		{
				// Convert source results to correct units
				// and correct coordinate system if necessary
				double N = pMemberResult->N * unitConversionFactor;
				double Vy = pMemberResult->Vy * unitConversionFactor;
				double Vz = pMemberResult->Vz * unitConversionFactor;
				double Tx = pMemberResult->Mx * unitConversionFactor;
				double My = pMemberResult->My * unitConversionFactor;
				double Mz = pMemberResult->Mz * unitConversionFactor;
				sections.Add(location, N, Vy, Vz, Tx, My, Mz);
		}
	}
}