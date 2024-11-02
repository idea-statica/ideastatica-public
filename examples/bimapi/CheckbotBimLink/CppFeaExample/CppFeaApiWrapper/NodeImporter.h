#pragma once
#include "..\CppFeaApi\NativeFeaGeometry.h"
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
		public ref class NodeImporter : public NodeImporterBase
		{
		private:
			ImporterContext^ context;
			IdeaVector3D^ GetLocation(int id);

		public:
			NodeImporter(ImporterContext^ context);

			virtual IIdeaNode^ Create(int id) override;
		};
	}
}