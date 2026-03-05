using System;

namespace yjk.FeaApis
{
	public interface IFeaMember
	{
		int Id { get; }
		int BeginNodeId { get; }
		int EndNodeId { get; }
		int CrossSectionId { get; }
		FeaNode BeginNode { get; }
		FeaNode EndNode { get; }
	}

	public class FeaMember : IFeaMember
	{
		public FeaMember(int id, FeaNode beginNode, FeaNode endNode, int crossSectionId)
		{
			Id = id;
			BeginNodeId = beginNode.Id;
			EndNodeId = endNode.Id;
			CrossSectionId = crossSectionId;
			BeginNode = beginNode;
			EndNode = endNode;
		}

		public int Id { get; set; }
		public int BeginNodeId { get; set; }
		public int EndNodeId { get; set; }
		public int CrossSectionId { get; set; }
		public FeaNode BeginNode { get; set; }
		public FeaNode EndNode { get; set; }

		public double GetLength()
		{
			double length = Math.Sqrt(Math.Pow(BeginNode.X - EndNode.X, 2) + Math.Pow(BeginNode.Y - EndNode.Y, 2) + Math.Pow(BeginNode.Z - EndNode.Z, 2));
			return length;
		}
	}
}
