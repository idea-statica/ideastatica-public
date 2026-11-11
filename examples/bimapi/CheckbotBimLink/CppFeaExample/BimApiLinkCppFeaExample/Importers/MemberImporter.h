#pragma once

#include "BimApi/Member1D.h"
#include "BimApi/Segment3D.h"
#include "CppFeaExampleApi/FeaGeometryApi.h"

namespace BimApiLinkCppFeaExample::Importers
{
	using namespace CppFeaExampleApi;
	using namespace IdeaStatiCa::BimApi;
	using namespace IdeaStatiCa::BimApiLink::BimApi;
	using namespace System::Collections::Generic;

	using CoordSystemByVector = IdeaRS::OpenModel::Geometry3D::CoordSystemByVector;
	using Member1DType = IdeaRS::OpenModel::Model::Member1DType;
	using Vector3D = IdeaRS::OpenModel::Geometry3D::Vector3D;

	/// <summary>
	/// Imports a member with its straight axis.
	/// </summary>
	ref class MemberImporter : BimApiLinkCppFeaExampleBases::MemberImporterBase
	{
	public:
		MemberImporter(const IFeaGeometryApi& geometry)
			: _geometry(&geometry)
		{
		}

		/// <summary>
		/// Converts the member with the given FEA id. Its axis runs between its two end nodes, and its own axes are the
		/// ones its internal forces are given in.
		/// </summary>
		virtual IIdeaMember1D^ Create(int id) override
		{
			FeaMember member = _geometry->GetMember(id);
			FeaAxes axes = _geometry->GetMemberAxes(id);

			CoordSystemByVector^ memberAxes = gcnew CoordSystemByVector();
			memberAxes->VecX = ToIdea(axes.X);
			memberAxes->VecY = ToIdea(axes.Y);
			memberAxes->VecZ = ToIdea(axes.Z);

			BimApi::Segment3D^ axis = gcnew BimApi::Segment3D(id);
			axis->StartNodeId = member.StartNodeId;
			axis->EndNodeId = member.EndNodeId;
			axis->LocalCoordinateSystem = memberAxes;

			IdeaElement1D^ element = gcnew IdeaElement1D(id);
			element->Segment = axis;

			BimApi::Member1D^ ideaMember = gcnew BimApi::Member1D(id);
			ideaMember->Type = Member1DType::Beam;
			ideaMember->CrossSectionId = member.CrossSectionId;
			ideaMember->Elements = gcnew List<IIdeaElement1D^>();
			ideaMember->Elements->Add(element);
			return ideaMember;
		}

	private:
		static Vector3D^ ToIdea(const FeaVector3D& vector)
		{
			Vector3D^ ideaVector = gcnew Vector3D();
			ideaVector->X = vector.X;
			ideaVector->Y = vector.Y;
			ideaVector->Z = vector.Z;
			return ideaVector;
		}

		const IFeaGeometryApi* _geometry;
	};
}
