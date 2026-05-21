using IdeaRS.OpenModel.Result;
using System;

namespace yjk.FeaApis
{
	public interface IFeaMember
	{
		int Id { get; }
		int BeginNodeId { get; }
		int EndNodeId { get; }
		string CrossSectionId { get; }
		FeaNode BeginNode { get; }
		FeaNode EndNode { get; }
		MemberType Type { get; }
		double RotationAngle { get; }
	}

	public enum MemberType
	{
		Column,
		Beam,
		Brace
	}

	public class FeaMember : IFeaMember
	{
		public FeaMember(int id, FeaNode beginNode, FeaNode endNode, string crossSectionId, MemberType type, double rotationAngle)
		{
			Id = id;
			BeginNodeId = beginNode.Id;
			EndNodeId = endNode.Id;
			CrossSectionId = crossSectionId;
			BeginNode = beginNode;
			EndNode = endNode;
			Type = type;
			RotationAngle = rotationAngle;
		}

		public int Id { get; set; }
		public int BeginNodeId { get; set; }
		public int EndNodeId { get; set; }
		public string CrossSectionId { get; set; }
		public FeaNode BeginNode { get; set; }
		public FeaNode EndNode { get; set; }
		public MemberType Type { get; set; }
		public double RotationAngle { get; set; }

		public double GetLength()
		{
			double length = Math.Sqrt(Math.Pow(BeginNode.X - EndNode.X, 2) + Math.Pow(BeginNode.Y - EndNode.Y, 2) + Math.Pow(BeginNode.Z - EndNode.Z, 2));
			return length;
		}
	}
}
