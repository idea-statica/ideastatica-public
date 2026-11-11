#include "FeaGeometryApi.h"

#include "FindById.h"

#include <cmath>

namespace CppFeaExampleApi
{
	namespace
	{
		double Length(const FeaVector3D& vector)
		{
			return std::sqrt((vector.X * vector.X) + (vector.Y * vector.Y) + (vector.Z * vector.Z));
		}

		/// <summary>
		/// The vector of length one in the same direction.
		/// </summary>
		FeaVector3D Normalize(const FeaVector3D& vector)
		{
			double length = Length(vector);
			return { vector.X / length, vector.Y / length, vector.Z / length };
		}

		/// <summary>
		/// The cross product: a vector perpendicular to both vectors.
		/// </summary>
		FeaVector3D Cross(const FeaVector3D& a, const FeaVector3D& b)
		{
			return {
				(a.Y * b.Z) - (a.Z * b.Y),
				(a.Z * b.X) - (a.X * b.Z),
				(a.X * b.Y) - (a.Y * b.X),
			};
		}
	}

	FeaGeometryApi::FeaGeometryApi()
		: _materials{
			{ 1, L"S 355" },
		}
		, _crossSections{
			// Id, name, material
			{ 1, L"IPE200", 1 },
		}
		, _nodes{
			// Id, X, Y, Z
			{ 1, 0.0, 0.0, 0.0 },
			{ 2, 0.0, 0.0, 3.0 },
			{ 3, 5.0, 0.0, 3.0 },
		}
		, _members{
			// Id, start node, end node, cross-section
			{ 1, 1, 2, 1 },
			{ 2, 2, 3, 1 },
		}
	{
	}

	std::vector<FeaNode> FeaGeometryApi::GetSelectedNodes() const
	{
		return { GetNode(2) };
	}

	std::vector<FeaMember> FeaGeometryApi::GetSelectedMembers() const
	{
		return _members;
	}

	std::vector<FeaMember> FeaGeometryApi::GetAllMembers() const
	{
		return _members;
	}

	FeaNode FeaGeometryApi::GetNode(int id) const
	{
		return FindById(_nodes, id);
	}

	FeaMember FeaGeometryApi::GetMember(int id) const
	{
		return FindById(_members, id);
	}

	double FeaGeometryApi::GetMemberLength(int id) const
	{
		return Length(GetMemberVector(id));
	}

	FeaAxes FeaGeometryApi::GetMemberAxes(int id) const
	{
		FeaVector3D x = Normalize(GetMemberVector(id));
		FeaVector3D globalZ{ 0.0, 0.0, 1.0 };

		bool isVertical = std::abs(std::abs(x.Z) - 1.0) < 1e-9;
		FeaVector3D y = isVertical
			? FeaVector3D{ 0.0, 1.0, 0.0 }
			: Normalize(Cross(globalZ, x));

		return { x, y, Cross(x, y) };
	}

	FeaCrossSection FeaGeometryApi::GetCrossSection(int id) const
	{
		return FindById(_crossSections, id);
	}

	FeaMaterial FeaGeometryApi::GetMaterial(int id) const
	{
		return FindById(_materials, id);
	}

	FeaVector3D FeaGeometryApi::GetMemberVector(int id) const
	{
		FeaMember member = GetMember(id);
		FeaNode start = GetNode(member.StartNodeId);
		FeaNode end = GetNode(member.EndNodeId);

		return { end.X - start.X, end.Y - start.Y, end.Z - start.Z };
	}
}
