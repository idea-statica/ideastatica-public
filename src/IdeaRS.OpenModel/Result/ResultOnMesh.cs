using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Type of result on mesh
	/// </summary>
	public enum ResultOnMeshType
	{
		/// <summary>
		/// Strain
		/// </summary>
		Strain = 22,
		/// <summary>
		/// StrainCheck
		/// </summary>
		StrainCheck = 23,
		/// <summary>
		/// Stress
		/// </summary>
		Stress = 24,

		/// <summary>
		/// Foundation stress
		/// </summary>
		FondationStress = 26,

		/// <summary>
		/// OverallCheck
		/// </summary>
		OverallCheck = 100
	}

	/// <summary>
	/// Tyoe of structural plate
	/// </summary>
	public enum StructuralPlateType
	{
		/// <summary>
		/// Steel plate
		/// </summary>
		SteelPlate,

		/// <summary>
		/// Weld
		/// </summary>
		Weld
	}


	/// <summary>
	/// Provides information about FEM element
	/// </summary>
	[Serializable]
	[DataContract]
	public class FemElement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public FemElement()
		{
			Vertices = new List<int>(4);
		}

		/// <summary>
		/// Constructor for triangular element 2d
		/// </summary>
		/// <param name="node1"></param>
		/// <param name="node2"></param>
		/// <param name="node3"></param>
		public FemElement(int node1, int node2, int node3)
		{
			Vertices = new List<int>(3);
			Vertices.Add(node1);
			Vertices.Add(node2);
			Vertices.Add(node3);
			Type = 1;
		}

		/// <summary>
		/// Constructor for quadrilateral element 2D
		/// </summary>
		/// <param name="node1"></param>
		/// <param name="node2"></param>
		/// <param name="node3"></param>
		/// <param name="node4"></param>
		public FemElement(int node1, int node2, int node3, int node4)
		{
			Vertices = new List<int>(4);
			Vertices.Add(node1);
			Vertices.Add(node2);
			Vertices.Add(node3);
			Vertices.Add(node4);
			Type = 2;
		}

		/// <summary>
		/// Type of the finite element
		/// 1 - triangular element 2D
		/// 2 - quadrilateral element 2D
		/// </summary>
		[DataMember]
		public int Type { get; set; }

		/// <summary>
		/// Get list of vertices of the fem element
		/// </summary>
		[DataMember]
		public List<int> Vertices { get; set; }
	}

	/// <summary>
	/// Data of plate elements
	/// </summary>
	[Serializable]
	[DataContract]
	public class PlateElements
	{
		/// <summary>
		/// Type plate elements
		/// </summary>
		public StructuralPlateType PlateType { get; set;}
		/// <summary>
		/// Plate UID
		/// </summary>
		public int PlateUID { get; set; }

		/// <summary>
		/// List of fem elements for plate UID
		/// </summary>
		[DataMember]
		public List<FemElement> Elements { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public PlateElements()
		{
			Elements = new List<FemElement>();
		}

		/// <summary>
		///Constructor
		/// </summary>
		/// <param name="count">Number of plate elements</param>
		public PlateElements(int count)
		{
			Elements = new List<FemElement>(count);
		}
	}

	///<Summary>
	/// Data structure for result mesh
	///</Summary>
	[DataContract]
	[Serializable]
	public class ResultOnMesh
	{
		///<Summary>
		/// Construstor
		///</Summary>
		public ResultOnMesh()
		{
			Nodes = new List<double[]>();
			Displacement = new List<double[]>();
			Value = new List<double>();
			PlatesElement = new List<PlateElements>();
		}

		///<Summary>
		/// List of coordinates of mesh nodes.
		/// There are 3 'doubles' for each node (x, y, z)
		///</Summary>
		[DataMember]
		public List<double[]> Nodes { get; set; }

		/// <summary>
		/// List of elements for each plate UID
		/// </summary>
		[DataMember]
		public List<PlateElements> PlatesElement { get; set; }

		///<Summary>
		/// Displacements at each node of the mesh in the global x, z ,y
		///</Summary>
		[DataMember]
		public List<double[]> Displacement { get; set; }

		///<Summary>
		/// Values of required type (stress, strain etc.) in nodes
		///</Summary>
		[DataMember]
		public List<double> Value { get; set; }
	}
}
