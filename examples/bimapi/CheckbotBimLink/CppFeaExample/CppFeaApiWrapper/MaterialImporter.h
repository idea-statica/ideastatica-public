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
		public ref class MatrialImporter : MaterialImporterBase
		{
		private:
			ImporterContext^ context;

		public:
			MatrialImporter(ImporterContext^ context);

			virtual IIdeaMaterial^ Create(int id) override;
		};
	}
}
