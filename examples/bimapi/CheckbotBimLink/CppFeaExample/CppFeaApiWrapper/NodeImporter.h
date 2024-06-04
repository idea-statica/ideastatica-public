#pragma once
#include "..\CppFeaApi\NativeFeaGeometry.h"
#include "ImporterContext.h"

using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink;
using namespace IdeaStatiCa::BimApiLink::Importers;
using namespace IdeaStatiCa::BimApiLink::BimApi;
using namespace IdeaStatiCa::BimApiLink::Identifiers;

namespace CppFeaApiWrapper
{
  namespace Importers
  {
    //public ref class NodeImporter : public IntIdentifierImporter<IIdeaNode^>
    //{
    //public:
    //  // Inherited via IntIdentifierImporter
    //  IdeaStatiCa::BimApi::IIdeaNode^ Create(int id) override;
    //};


    public ref class NodeImporter : public IntIdentifierImporter<IIdeaNode^>
    {
    private:
      ImporterContext^ context;
      IdeaVector3D^ GetLocation(int id);

    public:
      NodeImporter(ImporterContext^ context);

      
      virtual IIdeaNode^ Create(int id) override;

      virtual IIdeaNode^ Create(IdeaStatiCa::BimApiLink::Identifiers::Identifier<IIdeaNode^>^) override;
    };
  }
}