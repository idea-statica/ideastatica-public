#pragma once

#include "FeaObjects.h"

#include <vector>

namespace CppFeaExampleApi
{
	/// <summary>
	/// Reads the structure of the example FEA model. In a real link, this is the API of your application. Lengths are
	/// in metres.
	/// </summary>
	class IFeaGeometryApi
	{
	public:
		virtual ~IFeaGeometryApi() = default;

		/// <summary>
		/// The nodes the user has selected.
		/// </summary>
		virtual std::vector<FeaNode> GetSelectedNodes() const = 0;

		/// <summary>
		/// The members the user has selected.
		/// </summary>
		virtual std::vector<FeaMember> GetSelectedMembers() const = 0;

		/// <summary>
		/// Every member in the model.
		/// </summary>
		virtual std::vector<FeaMember> GetAllMembers() const = 0;

		/// <summary>
		/// The node with the given id.
		/// </summary>
		virtual FeaNode GetNode(int id) const = 0;

		/// <summary>
		/// The member with the given id.
		/// </summary>
		virtual FeaMember GetMember(int id) const = 0;

		/// <summary>
		/// The distance between the two end nodes of the member.
		/// </summary>
		virtual double GetMemberLength(int id) const = 0;

		/// <summary>
		/// The member's own axes. X runs from the start of the member to its end. Y is horizontal, or global Y for a
		/// vertical member. The internal forces of the member are given in these axes.
		/// </summary>
		virtual FeaAxes GetMemberAxes(int id) const = 0;

		/// <summary>
		/// The cross-section with the given id.
		/// </summary>
		virtual FeaCrossSection GetCrossSection(int id) const = 0;

		/// <summary>
		/// The material with the given id.
		/// </summary>
		virtual FeaMaterial GetMaterial(int id) const = 0;
	};

	/// <summary>
	/// Serves a fixed model: one corner of a steel frame. A 3 m column stands on node 1 and reaches up to node 2, where
	/// a 5 m beam starts and runs to node 3. Both are IPE200 in S 355. The user has selected the frame corner: node 2
	/// and both members.
	/// </summary>
	class FeaGeometryApi final : public IFeaGeometryApi
	{
	public:
		FeaGeometryApi();

		std::vector<FeaNode> GetSelectedNodes() const override;
		std::vector<FeaMember> GetSelectedMembers() const override;
		std::vector<FeaMember> GetAllMembers() const override;
		FeaNode GetNode(int id) const override;
		FeaMember GetMember(int id) const override;
		double GetMemberLength(int id) const override;
		FeaAxes GetMemberAxes(int id) const override;
		FeaCrossSection GetCrossSection(int id) const override;
		FeaMaterial GetMaterial(int id) const override;

	private:
		/// <summary>
		/// The vector from the start node of the member to its end node.
		/// </summary>
		FeaVector3D GetMemberVector(int id) const;

		std::vector<FeaMaterial> _materials;
		std::vector<FeaCrossSection> _crossSections;
		std::vector<FeaNode> _nodes;
		std::vector<FeaMember> _members;
	};
}
