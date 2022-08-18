using IdeaRS.OpenModel.Parameters;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaBoltGrid : IIdeaPersistentObject
	{
		/// <summary>
		/// Origin node of the bolt grid.
		/// </summary>
		IIdeaNode Origin { get; }

		/// <summary>
		/// Local Coordinate System (LCS) of the bolt grid. Only vector definition of the LCS is supported, so the instance of <see cref="IdeaRS.OpenModel.Geometry3D.CoordSystemByVector"/> must be returned.
		/// LCS only effects rotation of the segment, it does not modify on nodes' position.
		/// </summary>
		IdeaRS.OpenModel.Geometry3D.CoordSystem LocalCoordinateSystem { get; }

		/// <summary>
		/// Collection of nodes specific position of the bolt
		/// </summary>
		IEnumerable<IIdeaNode> Positions { get; }

		/// <summary>
		/// Collection of parf witch shoud be bolted
		/// </summary>
		IEnumerable<IIdeaObjectConnectable> ConnectedParts { get; }

		/// <summary>
		/// Shear in thread
		/// </summary>
		bool ShearInThread { get; }

		/// <summary>
		/// Defines a transfer of shear force in bolts.
		/// </summary>
		BoltShearType BoltShearType { get; }

		/// <summary>
		/// Bolt Assembly
		/// </summary>
		IIdeaBoltAssembly BoltAssembly { get; }
	}

	public interface IIdeaBoltAssembly : IIdeaObject
	{
		/// <summary>
		/// Hole Diameter
		/// </summary>
		double HoleDiameter { get; }

		/// <summary>
		/// Diameter
		/// </summary>
		double Diameter { get; }

		/// <summary>
		/// Head Diamete
		/// </summary>
		double HeadDiameter { get; }

		/// <summary>
		/// Lenght of bolt
		/// </summary>
		double Lenght { get; }

		/// <summary>
		/// Diagonal head diametere
		/// </summary>
		double DiagonalHeadDiameter { get; }

		/// <summary>
		/// Head height
		/// </summary>
		double HeadHeight { get; }

		/// <summary>
		/// Bore hole
		/// </summary>
		double BoreHole { get; }

		/// <summary>
		/// Tensile Stress Area
		/// </summary>
		double TensileStressArea { get; }

		/// <summary>
		/// Nut thickness
		/// </summary>
		double NutThickness { get; }

		/// <summary>
		/// Standard
		/// </summary>
		string Standard { get; }

		/// <summary>
		/// Material of the plate.
		/// </summary>
		IIdeaMaterial Material { get; }
	}



}