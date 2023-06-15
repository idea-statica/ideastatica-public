using MathNet.Spatial.Euclidean;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace BimApiLinkFeaExample.FeaExampleApi
{
	public interface IFeaGeometryApi
	{
		IFeaMember GetMember(int id);

		(UnitVector3D X, UnitVector3D Y, UnitVector3D Z) GetMemberLcs(int id);

		IEnumerable<int> GetMembersIdentifiers();

		IFeaNode GetNode(int id);

		IEnumerable<int> GetNodesIdentifiers();
	}

	internal class FeaGeometryApi : IFeaGeometryApi
	{
		private List<IFeaMember> _members = InitializeMembers();
		private List<IFeaNode> _nodes = InitializeNodes();

		public IFeaMember GetMember(int id) => _members.FirstOrDefault(m => m.Id == id);

		public (UnitVector3D X, UnitVector3D Y, UnitVector3D Z) GetMemberLcs(int id)
		{
			var member = GetMember(id);
			IFeaNode beg = GetNode(member.BeginNode);
			IFeaNode end = GetNode(member.EndNode);
			return CalculateMemberLcs(beg, end);
		}

		public IEnumerable<int> GetMembersIdentifiers() => _members.Select(m => m.Id);

		public IFeaNode GetNode(int id) => _nodes.FirstOrDefault(n => n.Id == id);

		public IEnumerable<int> GetNodesIdentifiers() => _nodes.Select(n => n.Id);

		private static (UnitVector3D X, UnitVector3D Y, UnitVector3D Z) CalculateMemberLcs(IFeaNode begin, IFeaNode end)
		{
			Vector3D memberX = end.Point - begin.Point;
			UnitVector3D globalZ = UnitVector3D.ZAxis;

			UnitVector3D memberY;

			if (memberX.IsParallelTo(globalZ))
			{
				// column
				memberY = UnitVector3D.YAxis;
			}
			else
			{
				// beam
				memberY = memberX.CrossProduct(globalZ).Normalize().Negate();
			}

			return (memberX.Normalize(), memberY, memberX.CrossProduct(memberY).Normalize());
		}

		private static List<IFeaMember> InitializeMembers()
		{
			return new List<IFeaMember>()
			{
				new FeaMember { Id = 1, BeginNode = 1, EndNode = 2, CrossSectionId = 1, },
				new FeaMember { Id = 2, BeginNode = 2, EndNode = 3, CrossSectionId = 1, },
			};
		}

		private static List<IFeaNode> InitializeNodes()
		{
			return new List<IFeaNode>()
			{
				new FeaNode(1, 0, 0, 0),
				new FeaNode(2, 0, 0, 3),
				new FeaNode(3, 5, 0, 3),
			};
		}
	}
}