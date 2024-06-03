using IdeaRS.OpenModel.Geometry2D;
using IdeaRS.OpenModel.Parameters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Provides data of the connection
	/// </summary>
	///
	public class ConnectionData
	{
		/// <summary>
		/// Connection Point Id
		/// </summary>
		[DataMember]
		public int ConenctionPointId { get; set; }

		/// <summary>
		/// Connected beams
		/// </summary>
		public List<BeamData> Beams { get; set; }

		/// <summary>
		/// Plates of the connection
		/// </summary>
		public List<PlateData> Plates { get; set; }

		/// <summary>
		/// Folded plate of the connection
		/// </summary>
		public List<FoldedPlateData> FoldedPlates { get; set; }

		/// <summary>
		/// Bolt grids which belongs to the connection
		/// </summary>
		public List<BoltGrid> BoltGrids { get; set; }

		/// <summary>
		/// Anchor grids which belongs to the connection
		/// </summary>
		public List<AnchorGrid> AnchorGrids { get; set; }

		/// <summary>
		/// Welds of the connection
		/// </summary>
		public List<WeldData> Welds { get; set; }

		/// <summary>
		/// ConcreteBlocksof the connection
		/// </summary>
		public List<ConcreteBlockData> ConcreteBlocks { get; set; }

		/// <summary>
		/// cut beam by beams
		/// </summary>
		public List<CutBeamByBeamData> CutBeamByBeams { get; set; }
	}

	/// <summary>
	/// Provides data of the single concrete block
	/// </summary>
	public class ConcreteBlockData
	{
		/// <summary>
		/// Plate unique ID
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Name of the concrete block
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Depth of the concrete block
		/// </summary>
		public double Depth { get; set; }

		/// <summary>
		/// Name of the material
		/// </summary>
		public string Material { get; set; }

		/// <summary>
		/// Center of the concrete block LCS
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D Center { get; set; }

		/// <summary>
		/// Outline points
		/// </summary>
		public List<IdeaRS.OpenModel.Geometry2D.Point2D> OutlinePoints { get; set; }

		/// <summary>
		/// Origin of the concrete block LCS
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D Origin { get; set; }

		/// <summary>
		/// LCS - Axis X
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisX { get; set; }

		/// <summary>
		/// LCS - Axis Y
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisY { get; set; }

		/// <summary>
		/// LCS - Axis Z
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisZ { get; set; }

		/// <summary>
		/// Geometry of the concrete block in svg format
		/// </summary>
		public string Region { get; set; }

		/// <summary>
		/// Get or set the identification in the original model
		/// In the case of the imported connection from another application
		/// </summary>
		public string OriginalModelId { get; set; }
	}

	/// <summary>
	/// Data of the bolt grid
	/// </summary>
	public class BoltGrid
	{

		public string BoltAssemblyRef { get; set; }
		/// <summary>
		/// Unique Id of the bolt grid
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Is Anchor
		/// </summary>
		public bool IsAnchor { get; set; }

		/// <summary>
		/// Anchor lenght
		/// </summary>
		public double AnchorLen { get; set; }

		/// <summary>
		/// The diameter of the hole
		/// </summary>
		public double HoleDiameter { get; set; }

		/// <summary>
		/// The diameter of bolt
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// The head diameter of bolt
		/// </summary>
		public double HeadDiameter { get; set; }

		/// <summary>
		/// The Diagonal Head Diameter of bolt
		/// </summary>
		public double DiagonalHeadDiameter { get; set; }

		/// <summary>
		/// The Head Height of bolt
		/// </summary>
		public double HeadHeight { get; set; }

		/// <summary>
		/// The BoreHole of bolt
		/// </summary>
		public double BoreHole { get; set; }

		/// <summary>
		/// The Tensile Stress Area of bolt
		/// </summary>
		public double TensileStressArea { get; set; }

		/// <summary>
		/// The Nut Thickness of bolt
		/// </summary>
		public double NutThickness { get; set; }

		/// <summary>
		/// The description of the bolt assembly
		/// </summary>
		public string BoltAssemblyName { get; set; }

		/// <summary>
		/// The standard of the bolt assembly
		/// </summary>
		public string Standard { get; set; }

		/// <summary>
		/// The material of the bolt assembly
		/// </summary>
		public string Material { get; set; }

		/// <summary>
		/// Origin of the bolt grid LCS
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D Origin { get; set; }

		/// <summary>
		/// Bolt grid LCS - Axis X
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisX { get; set; }

		/// <summary>
		/// Bolt grid LCS - Axis Y
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisY { get; set; }

		/// <summary>
		/// Bolt grid LCS - Axis Z
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisZ { get; set; }

		/// <summary>
		/// Positions of holes in the local coodinate system of the bolt grid
		/// </summary>
		public List<IdeaRS.OpenModel.Geometry3D.Point3D> Positions { get; set; }

		/// <summary>
		/// Identifiers of the connected plates
		/// </summary>
		public List<int> ConnectedPlates { get; set; }

		/// <summary>
		/// Id of the weld
		/// </summary>
		public List<string> ConnectedPartIds { get; set; }

		/// <summary>
		/// Indicates, whether a shear plane is in the thread of a bolt.
		/// </summary>
		public bool ShearInThread { get; set; }

		/// <summary>
		/// Indicates type of shear transfer
		/// </summary>
		public BoltShearType BoltInteraction { get; set; }
	}

	/// <summary>
	/// Data of the anchor grid
	/// </summary>
	public class AnchorGrid : BoltGrid
	{
		/// <summary>
		/// Data of concrete block
		/// </summary>
		public ConcreteBlock ConcreteBlock { get; set; }

		/// <summary>
		/// Anchor Type - washer
		/// </summary>
		public AnchorType AnchorType { get; set; }


		/// <summary>
		/// Washer Size used if AnchorType is washer
		/// </summary>
		public double WasherSize { get; set; }

	}

	/// <summary>
	/// Data of concrete block
	/// </summary>
	public class ConcreteBlock
	{
		/// <summary>
		/// Lenght of ConcreteBlock
		/// </summary>
		public double Lenght { get; set; }

		/// <summary>
		/// Width of ConcreteBlock
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		/// Height of ConcreteBlock
		/// </summary>
		public double Height { get; set; }

		/// <summary>
		/// Material of ConcreteBlock
		/// </summary>
		public string Material { get; set; }
	}

	/// <summary>
	/// Provides data of the connected beam
	/// </summary>
	[DataContract]
	public class BeamData : OpenElementId
	{
		/// <summary>
		/// Name of the beam
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Plates of the beam
		/// </summary>
		[DataMember]
		public List<PlateData> Plates { get; set; }

		/// <summary>
		/// Type of cross section
		/// </summary>
		[DataMember]
		public string CrossSectionType { get; set; }

		/// <summary>
		/// MPRL name of beam
		/// </summary>
		[DataMember]
		public string MprlName { get; set; }

		/// <summary>
		/// Get or set the identification in the original model
		/// In the case of the imported connection from another application
		/// </summary>
		[DataMember]
		public string OriginalModelId { get; set; }

		/// <summary>
		/// Cuts on the beam
		/// </summary>
		[DataMember]
		public List<CutData> Cuts { get; set; }

		/// <summary>
		/// Is added beam
		/// </summary>
		[DataMember]
		public bool IsAdded { get; set; }

		/// <summary>
		/// Added beam lenght
		/// </summary>
		[DataMember]
		public double AddedMemberLength { get; set; }

		/// <summary>
		/// Is negative object
		/// </summary>
		[DataMember]
		public bool IsNegativeObject { get; set; }

		/// <summary>
		/// Added member
		/// </summary>
		[DataMember]
		public ReferenceElement AddedMember { get; set; }

		/// <summary>
		/// Mirror by Y
		/// </summary>
		[DataMember]
		public bool MirrorY { get; set; }

		/// <summary>
		/// The reference line of the member is in the center of gravity of the cross-section
		/// </summary>
		[DataMember]
		public bool RefLineInCenterOfGravity { get; set; }

		/// <summary>
		/// Is beam bearing member
		/// </summary>
		[DataMember]
		public bool IsBearingMember { get; set; }

		/// <summary>
		/// Automaticali add cut by workplane if it not defined
		/// </summary>
		[DataMember]
		public bool AutoAddCutByWorkplane { get; set; }
	}

	/// <summary>
	///enum of weld types
	/// </summary>
	public enum WeldType : int
	{
		/// <summary>
		/// NotSpecified
		/// </summary>
		NotSpecified = 0,

		/// <summary>
		/// Fillet
		/// </summary>
		Fillet = 1,

		/// <summary>
		/// DoubleFillet
		/// </summary>
		DoubleFillet = 2,

		/// <summary>
		/// Bevel
		/// </summary>
		Bevel = 4,

		/// <summary>
		/// Partial joint penetration butt weld
		/// </summary>
		PJP = 8,
		/// <summary>
		/// Partial joint penetration butt weld rear side
		/// </summary>
		PJPRear = 16,

		/// <summary>
		/// LengthAtHaunch
		/// </summary>
		LengthAtHaunch = 32,

		/// <summary>
		/// FilletRear
		/// </summary>
		FilletRear = 64,

		/// <summary>
		/// Contact
		/// </summary>
		Contact = 128,

		/// <summary>
		/// Intermittent
		/// </summary>
		Intermittent = 256,
	}

	/// <summary>
	/// Provides data of the single weld
	/// </summary>
	public class WeldData
	{
		/// <summary>
		/// Id of the weld
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Name of the weld
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Thickness of the weld
		/// </summary>
		public double Thickness { get; set; }

		/// <summary>
		/// Name of the material
		/// </summary>
		public string Material { get; set; }

		/// <summary>
		/// Material of the weld
		/// </summary>
		public ReferenceElement WeldMaterial { get; set; }

		/// <summary>
		/// Type of the weld
		/// </summary>
		public WeldType WeldType { get; set; }

		/// <summary>
		/// Id of the weld
		/// </summary>
		public List<string> ConnectedPartIds { get; set; }

		/// <summary>
		/// Start of the weld
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D Start { get; set; }

		/// <summary>
		/// End of the weld
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D End { get; set; }
	}

	/// <summary>
	/// Provides data of the single plate
	/// </summary>
	[DataContract]
	public class PlateData : OpenElementId
	{
		/// <summary>
		/// Name of the plate
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Thickness of the plate
		/// </summary>
		[DataMember]
		public double Thickness { get; set; }

		/// <summary>
		/// Name of the material
		/// </summary>
		[DataMember]
		public string Material { get; set; }

		/// <summary>
		/// Outline points
		/// </summary>
		[DataMember]
		public List<IdeaRS.OpenModel.Geometry2D.Point2D> OutlinePoints { get; set; }

		/// <summary>
		/// Origin of the plate LCS
		/// </summary>
		[DataMember]
		public IdeaRS.OpenModel.Geometry3D.Point3D Origin { get; set; }

		/// <summary>
		/// LCS - Axis X
		/// </summary>
		[DataMember]
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisX { get; set; }

		/// <summary>
		/// LCS - Axis Y
		/// </summary>
		[DataMember]
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisY { get; set; }

		/// <summary>
		/// LCS - Axis Z
		/// </summary>
		[DataMember]
		public IdeaRS.OpenModel.Geometry3D.Vector3D AxisZ { get; set; }

		/// <summary>
		/// Geometry of the plate in svg format. In next version will be mark as OBSOLETE! New use property Geometry
		/// </summary>
		[DataMember]
		//[ObsoleteAttribute("This property is obsolete. Use Geometry instead.", false)]
		public string Region { get; set; }

		/// <summary>
		/// Geometry of the plate described by Region2D - new prefered representation of geometry
		/// </summary>
		[DataMember]
		public Region2D Geometry { get; set; }


		/// <summary>
		/// Get or set the identification in the original model
		/// In the case of the imported connection from another application
		/// </summary>
		[DataMember]
		public string OriginalModelId { get; set; }

		/// <summary>
		/// Is negative object
		/// </summary>
		[DataMember]
		public bool IsNegativeObject { get; set; }
	}

	/// <summary>
	/// Provides data of the folded plate
	/// </summary>
	public class FoldedPlateData
	{
		/// <summary>
		/// List of plates belong to folded plate
		/// </summary>
		public List<PlateData> Plates { get; set; }

		/// <summary>
		/// List of bends connected plates of foldedplate
		/// </summary>
		public List<BendData> Bends { get; set; }
	}

	/// <summary>
	/// Provides data of bend
	/// </summary>
	public class BendData
	{
		/// <summary>
		/// First plate
		/// </summary>
		public int Plate1Id { get; set; }

		/// <summary>
		/// Second plate
		/// </summary>
		public int Plate2Id { get; set; }

		/// <summary>
		/// Radius of bend
		/// </summary>
		public double Radius { get; set; }

		/// <summary>
		/// Side boundary first plate point 1
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D Point1OfSideBoundary1 { get; set; }

		/// <summary>
		/// Side boundary first plate point 2
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D Point2OfSideBoundary1 { get; set; }

		/// <summary>
		/// End Face Normal vector
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D EndFaceNormal1 { get; set; }

		/// <summary>
		/// Side boundary second plate point 1
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D Point1OfSideBoundary2 { get; set; }

		/// <summary>
		/// Side boundary second plate point 2
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D Point2OfSideBoundary2 { get; set; }
	}

	/// <summary>
	/// Provides data of the cut beam
	/// </summary>
	public class CutData
	{
		/// <summary>
		/// 3DPlane Point
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Point3D PlanePoint { get; set; }

		/// <summary>
		/// Plane normal. Direction of normal set remove part of beam
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D NormalVector { get; set; }

		/// <summary>
		/// Direction of cut [Parallel|Perpendicular]
		/// </summary>
		public CutOrientation Direction { get; set; }
		/// <summary>
		/// Offset - shift of cut
		/// </summary>
		public double Offset { get; set; }
	}

	/// <summary>
	/// Provides data of the cut objec by object
	/// </summary>
	public class CutBeamByBeamData
	{
		/// <summary>
		/// Modified object
		/// </summary>
		public ReferenceElement ModifiedObject { get; set; }

		/// <summary>
		/// Cutting by object
		/// </summary>
		public ReferenceElement CuttingObject { get; set; }

		/// <summary>
		/// is cut welded
		/// </summary>
		public bool IsWeld { get; set; }

		/// <summary>
		/// Thickness of the weld - value 0 = recommended size
		/// </summary>
		public double WeldThickness { get; set; }

		/// <summary>
		/// Type of the weld
		/// </summary>
		public WeldType WeldType { get; set; }


		/// <summary>
		/// Offset
		/// </summary>
		public double Offset { get; set; }

		/// <summary>
		/// Cut Method
		/// For cut beam by neagtive object is reqied method Surface
		/// </summary>
		public CutMethod Method { get; set; }

		/// <summary>
		/// Cut Orientation
		/// </summary>
		public CutOrientation Orientation { get; set; }

		/// <summary>
		/// PlaneOnCuttingObject
		/// </summary>
		public DistanceComparison PlaneOnCuttingObject { get; set; }

		/// <summary>
		/// CutPart - The part of the stiffening member which is cut off
		/// </summary>
		public CutPart CutPart { get; set; }
	}

	/// <summary>
	/// Represents the type of the cut
	/// </summary>
	public enum CutOrientation
	{
		/// <summary>
		/// The cut is in default (parallel) with cutting plane
		/// </summary>
		Default = 0,

		/// <summary>
		/// The cut is in parallel with cutting plane
		/// </summary>
		Parallel = 1,

		/// <summary>
		/// The cut is perpendicular to cutting plane
		/// </summary>
		Perpendicular = 2
	}

	/// <summary>
	/// Represents the result of comparison of distances of two object from the same point
	/// </summary>
	public enum DistanceComparison
	{
		/// <summary>
		/// Closer
		/// </summary>
		Closer,
		/// <summary>
		/// Farther
		/// </summary>
		Farther,
		/// <summary>
		/// Same
		/// </summary>
		Same
	}

	/// <summary>
	/// Cut method
	/// </summary>
	public enum CutMethod
	{
		/// <summary>
		/// BoundingBox
		/// </summary>
		BoundingBox,
		/// <summary>
		/// Surface
		/// </summary>
		Surface,
		/// <summary>
		/// Mitre
		/// </summary>
		Mitre,
		/// <summary>
		/// SurfaceAll
		/// </summary>
		SurfaceAll
	}

	public enum CutPart
	{
		/// <summary>
		/// The begin is cut off
		/// </summary>
		Begin,

		/// <summary>
		/// The end is cut off
		/// </summary>
		End,
	}
}