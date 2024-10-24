#pragma once
#include "ImporterContext.h"

using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink;
using namespace IdeaStatiCa::BimApiLink::Importers;
using namespace IdeaStatiCa::BimApiLink::BimApi;
using namespace IdeaStatiCa::BimApiLink::Identifiers;
using namespace IdeaRS::OpenModel::Loading;
using namespace ImporterWrappers;
using namespace System;

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		public ref class LoadCombiImporter : public LoadCombiImporterBase
		{
		private:
			ImporterContext^ context;


		public:
			LoadCombiImporter(ImporterContext^ context);

			virtual IIdeaCombiInput^ Create(int id) override;
		};
	}
}
