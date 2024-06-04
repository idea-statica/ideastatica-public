#include "pch.h"
#include "NodeImporter.h"

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		//IdeaStatiCa::BimApi::IIdeaNode^ NodeImporter::Create(int id)
		//{
		//	base::Create(id);
		//	throw gcnew System::NotImplementedException();
		//	// TODO: insert return statement here
		//}

		NodeImporter::NodeImporter(ImporterContext^ context)
		{
			this->context = context;
		}

		IdeaVector3D^ NodeImporter::GetLocation(int id)
		{
			NativeFeaNode* feaNode = this->context->GetGeometry()->GetNode(id);
			IdeaStatiCa::BimApi::IdeaVector3D^ vect = gcnew IdeaVector3D(feaNode->X, feaNode->Y, feaNode->Z);
			return vect;
		}

		IIdeaNode^ NodeImporter::Create(int id)
		{
			IdeaVector3D^ v = GetLocation(id);
			IdeaNode^ node = gcnew IdeaNode(id);
			node->Vector = v;
			return node;
		}


		

		IIdeaNode^ Create(IdeaStatiCa::BimApiLink::Identifiers::Identifier<IIdeaNode^>^ identifier)
		{
			return nullptr;
		}
	}
}