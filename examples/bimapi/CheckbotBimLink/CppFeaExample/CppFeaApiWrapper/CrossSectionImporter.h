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
		public ref class CrossSectionImporter : CrossSectionImporterBase
		{
		private:
			ImporterContext^ context;

		public:
			CrossSectionImporter(ImporterContext^ context);

			virtual IIdeaCrossSection^ Create(int id) override;
		};
	}
}
