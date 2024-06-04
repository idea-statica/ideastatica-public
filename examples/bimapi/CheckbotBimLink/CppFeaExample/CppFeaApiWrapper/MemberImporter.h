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
    public ref class MemberImporter : MemberImporterBase
    {
    private:
      ImporterContext^ context;

    public:
      MemberImporter(ImporterContext^ context);

      virtual IIdeaMember1D^ Create(int id) override;
    };
  }
}

