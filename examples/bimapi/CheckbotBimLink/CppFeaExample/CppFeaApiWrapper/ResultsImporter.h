#pragma once
#include "ImporterContext.h"

using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink;
using namespace IdeaStatiCa::BimApiLink::Importers;
using namespace IdeaStatiCa::BimApiLink::BimApi;
using namespace IdeaStatiCa::BimApiLink::Identifiers;
using namespace IdeaStatiCa::BimApiLink::Results;
using namespace IdeaRS::OpenModel::Loading;
using namespace ImporterWrappers;
using namespace System;
using namespace System::Collections::Generic;

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		public ref class ResultsImporter : public ResultsImporterBase
		{
		private:
			ImporterContext^ context;
			NativeFeaResults* pFeaResults;
			NativeFeaLoads* pFeaLoading;


			// BimApi uses basic SI units
			const static int unitConversionFactor = 1000;

			// Precision for check, if 2 results are in the same location
			const static double resultSectionPositionPrecision = 1e-6;

			// BimApi can not handle more results in the same location,
			// so in those cases it is necessary shift the location
			const static double shift = 5e-6;

			void AddResult(InternalForcesBuilder<IIdeaMember1D^>::Sections^ sections, double location, NativeFeaMemberResult* pMemberResult);
			double GetLength(IIdeaNode^ startNode, IIdeaNode^ endNode);

			void SetResultsForMember(IdeaMember1D^ member,
				IntIdentifier<IIdeaLoadCase^>^ loadCase,
				InternalForcesBuilder<IIdeaMember1D^>::Sections^ sections);

		public:
			ResultsImporter(ImporterContext^ context);

			virtual System::Collections::Generic::IEnumerable<ResultsData<IIdeaMember1D^>^>^ GetResults(IReadOnlyList<IIdeaMember1D^>^ members) override;
		};
	}
}
