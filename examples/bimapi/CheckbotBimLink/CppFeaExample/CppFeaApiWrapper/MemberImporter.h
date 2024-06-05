#pragma once
#include "ImporterContext.h"

using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink;
using namespace IdeaStatiCa::BimApiLink::Importers;
using namespace IdeaStatiCa::BimApiLink::BimApi;
using namespace IdeaStatiCa::BimApiLink::Identifiers;
using namespace ImporterWrappers;
using namespace MathNet::Spatial::Euclidean;

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

		protected:
			IIdeaSegment3D^ CreateSegment(NativeFeaMember* feaMember);

			static array<UnitVector3D^>^ CalculateMemberLcs(NativeFeaNode* pBegin, NativeFeaNode* pEnd);
			static IdeaRS::OpenModel::Geometry3D::Vector3D^ ConvertVector(UnitVector3D^ v);
		};
	}
}

