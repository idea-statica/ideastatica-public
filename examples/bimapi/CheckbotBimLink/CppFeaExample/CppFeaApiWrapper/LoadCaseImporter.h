#pragma once
#include "ImporterContext.h"

using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink;
using namespace IdeaStatiCa::BimApiLink::Importers;
using namespace IdeaStatiCa::BimApiLink::BimApi;
using namespace IdeaStatiCa::BimApiLink::Identifiers;
using namespace ImporterWrappers;

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		public ref class LoadCaseImporter : public LoadCaseImporterBase
		{
		private:
			ImporterContext^ context;

		public:
			LoadCaseImporter(ImporterContext^ context);

			virtual IIdeaLoadCase^ Create(int id) override;
		};
	}
}

