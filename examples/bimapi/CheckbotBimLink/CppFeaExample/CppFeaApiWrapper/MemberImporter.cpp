#include "pch.h"
#include "MemberImporter.h"
#include "Member1D.h"
#include "Segment3D.h"

using namespace MathNet::Spatial::Euclidean;
using namespace System::Collections::Generic;

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		MemberImporter::MemberImporter(ImporterContext^ context)
		{
			this->context = context;
		}

		IIdeaMember1D^ MemberImporter::Create(int id)
		{
			NativeFeaMember* feaMember = context->GetGeometry()->GetMember(id);

			IdeaElement1D^ element = gcnew IdeaElement1D(id);
			IIdeaSegment3D^ segment = CreateSegment(feaMember);
			element->Segment = segment;

			List<IIdeaElement1D^>^ elements = gcnew List<IIdeaElement1D^>();
			elements->Add(element);

			CppFeaApiWrapper::BimApi::Member1D^ member = gcnew CppFeaApiWrapper::BimApi::Member1D(id);

			member->Type = IdeaRS::OpenModel::Model::Member1DType::Beam;
			member->CrossSectionNo = feaMember->CrossSectionId;
			member->Elements = elements;

			return member;
		}

		IIdeaSegment3D^ MemberImporter::CreateSegment(NativeFeaMember* feaMember)
		{
			NativeFeaNode* pBegin = context->GetGeometry()->GetNode(feaMember->BeginNode);
			NativeFeaNode* pEnd = context->GetGeometry()->GetNode(feaMember->EndNode);

			array<UnitVector3D^>^ lcs = CalculateMemberLcs(pBegin, pEnd);

			CppFeaApiWrapper::BimApi::Segment3D^ segment = gcnew CppFeaApiWrapper::BimApi::Segment3D(feaMember->Id);
			segment->StartNodeNo = pBegin->Id;
			segment->EndNodeNo = pEnd->Id;

			IdeaRS::OpenModel::Geometry3D::CoordSystemByVector^ cs = gcnew IdeaRS::OpenModel::Geometry3D::CoordSystemByVector();
			cs->VecX = ConvertVector(lcs[0]);
			cs->VecY = ConvertVector(lcs[1]);
			cs->VecZ = ConvertVector(lcs[2]);

			segment->LocalCoordinateSystem = cs;
			return segment;
		}

		IdeaRS::OpenModel::Geometry3D::Vector3D^ MemberImporter::ConvertVector(UnitVector3D^ v)
		{
			IdeaRS::OpenModel::Geometry3D::Vector3D^ res = gcnew IdeaRS::OpenModel::Geometry3D::Vector3D();
			res->X = v->X;
			res->Y = v->Y;
			res->Z = v->Z;
			return res;
		}

		array<UnitVector3D^>^ MemberImporter::CalculateMemberLcs(NativeFeaNode* pBegin, NativeFeaNode* pEnd)
		{
			Point3D begin = Point3D(pBegin->X, pBegin->Y, pBegin->Z);
			Point3D end = Point3D(pEnd->X, pEnd->Y, pEnd->Z);
			Vector3D memberX = end - begin;

			UnitVector3D globalZ = UnitVector3D::ZAxis;

			UnitVector3D memberY;

			if (memberX.IsParallelTo(globalZ, 1E-10))
			{
				// column
				memberY = UnitVector3D::YAxis;
			}
			else
			{
				// beam
				memberY = memberX.CrossProduct(globalZ).Normalize().Negate();
			}

			array<UnitVector3D^>^ res = gcnew array<UnitVector3D^>(3);
			res[0] = memberX.Normalize();
			res[1] = memberY;
			res[2] = memberX.CrossProduct(memberY).Normalize();

			return res;
		}
	}
}