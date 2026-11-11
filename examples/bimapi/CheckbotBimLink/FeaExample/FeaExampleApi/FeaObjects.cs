using System;
using System.Collections.Generic;

namespace FeaExampleApi
{
	/// <summary>
	/// A material of the FEA model. Cross-sections refer to it by id.
	/// </summary>
	public class FeaMaterial
	{
		public FeaMaterial(int id, string name)
		{
			Id = id;
			Name = name;
		}

		public int Id { get; }

		public string Name { get; }
	}

	/// <summary>
	/// A cross-section of the FEA model, made of one material. Members refer to it by id.
	/// </summary>
	public class FeaCrossSection
	{
		public FeaCrossSection(int id, string name, int materialId)
		{
			Id = id;
			Name = name;
			MaterialId = materialId;
		}

		public int Id { get; }

		public string Name { get; }

		public int MaterialId { get; }
	}

	/// <summary>
	/// A node of the FEA model. Coordinates are in metres.
	/// </summary>
	public class FeaNode
	{
		public FeaNode(int id, double x, double y, double z)
		{
			Id = id;
			X = x;
			Y = y;
			Z = z;
		}

		public int Id { get; }

		public double X { get; }

		public double Y { get; }

		public double Z { get; }

		/// <summary>
		/// The vector from this node to the end node.
		/// </summary>
		public FeaVector3D VectorTo(FeaNode end) => new FeaVector3D(end.X - X, end.Y - Y, end.Z - Z);
	}

	/// <summary>
	/// A straight member of the FEA model, from its start node to its end node.
	/// </summary>
	public class FeaMember
	{
		public FeaMember(int id, int startNodeId, int endNodeId, int crossSectionId)
		{
			Id = id;
			StartNodeId = startNodeId;
			EndNodeId = endNodeId;
			CrossSectionId = crossSectionId;
		}

		public int Id { get; }

		public int StartNodeId { get; }

		public int EndNodeId { get; }

		public int CrossSectionId { get; }
	}

	/// <summary>
	/// A direction or an offset in the model.
	/// </summary>
	public class FeaVector3D
	{
		public FeaVector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public double X { get; }

		public double Y { get; }

		public double Z { get; }

		/// <summary>
		/// The length of the vector.
		/// </summary>
		public double Length => Math.Sqrt((X * X) + (Y * Y) + (Z * Z));

		/// <summary>
		/// The vector of length one in the same direction.
		/// </summary>
		public FeaVector3D Normalize() => new FeaVector3D(X / Length, Y / Length, Z / Length);

		/// <summary>
		/// The cross product: a vector perpendicular to both vectors.
		/// </summary>
		public FeaVector3D Cross(FeaVector3D other) => new FeaVector3D(
			(Y * other.Z) - (Z * other.Y),
			(Z * other.X) - (X * other.Z),
			(X * other.Y) - (Y * other.X));
	}

	/// <summary>
	/// The three unit axes of a member.
	/// </summary>
	public class FeaAxes
	{
		public FeaAxes(FeaVector3D x, FeaVector3D y, FeaVector3D z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public FeaVector3D X { get; }

		public FeaVector3D Y { get; }

		public FeaVector3D Z { get; }
	}

	/// <summary>
	/// Whether the load cases of a group are always there or come and go.
	/// </summary>
	public enum FeaLoadGroupType
	{
		Permanent,
		Variable,
	}

	/// <summary>
	/// A group of load cases of one kind. Load cases refer to it by id.
	/// </summary>
	public class FeaLoadGroup
	{
		public FeaLoadGroup(int id, string name, FeaLoadGroupType type)
		{
			Id = id;
			Name = name;
			Type = type;
		}

		public int Id { get; }

		public string Name { get; }

		public FeaLoadGroupType Type { get; }
	}

	/// <summary>
	/// What causes the loads of a load case.
	/// </summary>
	public enum FeaLoadCaseType
	{
		SelfWeight,
		DeadLoad,
		Snow,
	}

	/// <summary>
	/// A load case: loads that act together. The FEA application has calculated internal forces for each one.
	/// </summary>
	public class FeaLoadCase
	{
		public FeaLoadCase(int id, string name, FeaLoadCaseType type, int loadGroupId)
		{
			Id = id;
			Name = name;
			Type = type;
			LoadGroupId = loadGroupId;
		}

		public int Id { get; }

		public string Name { get; }

		public FeaLoadCaseType Type { get; }

		public int LoadGroupId { get; }
	}

	/// <summary>
	/// The limit state a combination is checked for.
	/// </summary>
	public enum FeaLimitState
	{
		Ultimate,
		Serviceability,
	}

	/// <summary>
	/// One load case of a combination and the factor it is multiplied by.
	/// </summary>
	public class FeaCombinationFactor
	{
		public FeaCombinationFactor(int loadCaseId, double factor)
		{
			LoadCaseId = loadCaseId;
			Factor = factor;
		}

		public int LoadCaseId { get; }

		public double Factor { get; }
	}

	/// <summary>
	/// A load combination: the sum of its load cases, each multiplied by its factor.
	/// </summary>
	public class FeaCombination
	{
		public FeaCombination(int id, string name, FeaLimitState limitState, IReadOnlyList<FeaCombinationFactor> factors)
		{
			Id = id;
			Name = name;
			LimitState = limitState;
			Factors = factors;
		}

		public int Id { get; }

		public string Name { get; }

		public FeaLimitState LimitState { get; }

		public IReadOnlyList<FeaCombinationFactor> Factors { get; }
	}

	/// <summary>
	/// The internal forces at one point of a member, in the member's own axes. Forces are in kN, moments in kNm.
	/// </summary>
	public class FeaMemberResult
	{
		public FeaMemberResult(double position, double n, double vy, double vz, double mx, double my, double mz)
		{
			Position = position;
			N = n;
			Vy = vy;
			Vz = vz;
			Mx = mx;
			My = my;
			Mz = mz;
		}

		/// <summary>
		/// The distance from the start of the member, in metres.
		/// </summary>
		public double Position { get; }

		public double N { get; }

		public double Vy { get; }

		public double Vz { get; }

		public double Mx { get; }

		public double My { get; }

		public double Mz { get; }
	}
}
