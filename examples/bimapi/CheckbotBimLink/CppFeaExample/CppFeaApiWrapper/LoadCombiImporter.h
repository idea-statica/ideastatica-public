#pragma once
#include "ImporterContext.h"
#include <iostream>
#include <vector>

using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink;
using namespace IdeaStatiCa::BimApiLink::Importers;
using namespace IdeaStatiCa::BimApiLink::BimApi;
using namespace IdeaStatiCa::BimApiLink::Identifiers;
using namespace IdeaRS::OpenModel::Loading;
using namespace ImporterWrappers;
using namespace System;
using namespace System::Collections::Generic;

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		public ref class LoadCombiImporter : public LoadCombiImporterBase
		{
		private:
			ImporterContext^ context;
			List<IIdeaCombiItem^>^ GetCombiInputs(std::vector<NativeFeaCombiFactor>* loadFactors, int combiId);

		public:
			LoadCombiImporter(ImporterContext^ context);

			virtual IIdeaCombiInput^ Create(int id) override;

			static IdeaRS::OpenModel::Loading::TypeCalculationCombiEC GetCalculationType(int loadCombiType);

			static IdeaRS::OpenModel::Loading::TypeOfCombiEC GetCombiAction(int combiCategory);
		};
	}
}
