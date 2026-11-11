using System;
using System.Collections.Generic;
using System.Linq;

namespace FeaExampleApi
{
	/// <summary>
	/// Reads the structure of the example FEA model. In a real link, this is the API of your application. Lengths are
	/// in metres.
	/// </summary>
	public interface IFeaGeometryApi
	{
		/// <summary>
		/// The nodes the user has selected.
		/// </summary>
		IEnumerable<FeaNode> GetSelectedNodes();

		/// <summary>
		/// The members the user has selected.
		/// </summary>
		IEnumerable<FeaMember> GetSelectedMembers();

		/// <summary>
		/// Every member in the model.
		/// </summary>
		IEnumerable<FeaMember> GetAllMembers();

		/// <summary>
		/// The node with the given id.
		/// </summary>
		FeaNode GetNode(int id);

		/// <summary>
		/// The member with the given id.
		/// </summary>
		FeaMember GetMember(int id);

		/// <summary>
		/// The distance between the two end nodes of the member.
		/// </summary>
		double GetMemberLength(int id);

		/// <summary>
		/// The member's own axes. X runs from the start of the member to its end. Y is horizontal, or global Y for a
		/// vertical member. The internal forces of the member are given in these axes.
		/// </summary>
		FeaAxes GetMemberAxes(int id);

		/// <summary>
		/// The cross-section with the given id.
		/// </summary>
		FeaCrossSection GetCrossSection(int id);

		/// <summary>
		/// The material with the given id.
		/// </summary>
		FeaMaterial GetMaterial(int id);
	}

	/// <summary>
	/// Serves a fixed model: one corner of a steel frame. A 3 m column stands on node 1 and reaches up to node 2, where
	/// a 5 m beam starts and runs to node 3. Both are IPE200 in S 355. The user has selected the frame corner: node 2
	/// and both members.
	/// </summary>
	public class FeaGeometryApi : IFeaGeometryApi
	{
		private readonly List<FeaMaterial> _materials = new List<FeaMaterial>
		{
			new FeaMaterial(1, "S 355"),
		};

		private readonly List<FeaCrossSection> _crossSections = new List<FeaCrossSection>
		{
			new FeaCrossSection(1, "IPE200", materialId: 1),
		};

		private readonly List<FeaNode> _nodes = new List<FeaNode>
		{
			new FeaNode(1, 0.0, 0.0, 0.0),
			new FeaNode(2, 0.0, 0.0, 3.0),
			new FeaNode(3, 5.0, 0.0, 3.0),
		};

		private readonly List<FeaMember> _members = new List<FeaMember>
		{
			new FeaMember(1, startNodeId: 1, endNodeId: 2, crossSectionId: 1),
			new FeaMember(2, startNodeId: 2, endNodeId: 3, crossSectionId: 1),
		};

		public IEnumerable<FeaNode> GetSelectedNodes() => new[] { GetNode(2) };

		public IEnumerable<FeaMember> GetSelectedMembers() => _members;

		public IEnumerable<FeaMember> GetAllMembers() => _members;

		public FeaNode GetNode(int id) => _nodes.Single(node => node.Id == id);

		public FeaMember GetMember(int id) => _members.Single(member => member.Id == id);

		public double GetMemberLength(int id) => GetMemberVector(id).Length;

		public FeaAxes GetMemberAxes(int id)
		{
			FeaVector3D x = GetMemberVector(id).Normalize();
			FeaVector3D globalZ = new FeaVector3D(0, 0, 1);

			bool isVertical = Math.Abs(Math.Abs(x.Z) - 1) < 1e-9;
			FeaVector3D y = isVertical
				? new FeaVector3D(0, 1, 0)
				: globalZ.Cross(x).Normalize();

			return new FeaAxes(x, y, x.Cross(y));
		}

		public FeaCrossSection GetCrossSection(int id) => _crossSections.Single(section => section.Id == id);

		public FeaMaterial GetMaterial(int id) => _materials.Single(material => material.Id == id);

		/// <summary>
		/// The vector from the start node of the member to its end node.
		/// </summary>
		private FeaVector3D GetMemberVector(int id)
		{
			FeaMember member = GetMember(id);
			return GetNode(member.StartNodeId).VectorTo(GetNode(member.EndNodeId));
		}
	}
}
