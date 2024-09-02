using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Type of cross-section
	/// </summary>
	[DataContract]
	public enum CrossSectionType
	{
		/// <summary>
		/// Cross-section with one component without shape specification
		/// </summary>
		OneComponentCss = 0,

		/// <summary>
		/// Rolled I
		/// </summary>
		RolledI = 1,

		/// <summary>
		/// Rolled angle
		/// </summary>
		RolledAngle = 2,

		/// <summary>
		/// Rolled T
		/// </summary>
		RolledT = 3,

		/// <summary>
		/// Rolled Channel
		/// </summary>
		RolledU = 4,

		/// <summary>
		/// Rolled circular holow section
		/// </summary>
		RolledCHS = 5,

		/// <summary>
		/// Rolled rectangular hollow setion
		/// </summary>
		RolledRHS = 6,

		/// <summary>
		/// Composed double U rolled - ][
		/// </summary>
		RolledDoubleUo = 7,

		/// <summary>
		/// Composed double U rolled - []
		/// </summary>
		RolledDoubleUc = 8,

		///// <summary>
		///// Composed I + U
		///// </summary>
		//RolledIU = 9,

		/// <summary>
		/// Composed double L rolled - _||_
		/// </summary>
		RolledDoubleLt = 10,

		/// <summary>
		/// Pair double L  |_ _|
		/// </summary>
		RolledDoubleLu = 11,

		/// <summary>
		/// T profile made from part of I profile
		/// </summary>
		RolledTI = 12,

		/// <summary>
		/// Rolled IPar - Parametrized I section
		/// </summary>
		RolledIPar = 13,

		/// <summary>
		/// Rolled UPar - Parametrized Channel section
		/// </summary>
		RolledUPar = 14,

		/// <summary>
		/// Rolled LPar - Parametrized Angle section
		/// </summary>
		RolledLPar = 15,

		/// <summary>
		/// Welded Box - contains two dominante flanges and two webs
		/// -----
		///  | |
		/// -----
		/// </summary>
		BoxFl = 16,

		/// <summary>
		/// Welded Box - contains two dominante flanges and two webs, bottom flange is inside between webs
		/// ------
		///  |  |
		///  |--|
		/// </summary>
		BoxWeb = 17,

		/// <summary>
		/// Box from double I rolled - II
		/// </summary>
		Box2I = 18,

		/// <summary>
		/// Box from double 2U
		/// </summary>
		Box2U = 19,

		/// <summary>
		/// Box from 2U and 2 Plates
		/// </summary>
		Box2U2PI = 20,

		/// <summary>
		/// Box welded from two rolled L-profiles
		/// </summary>
		Box2L = 21,

		/// <summary>
		/// Box section welded from 4 L-profiles
		/// </summary>
		Box4L = 22,

		/// <summary>
		/// Welded I
		/// </summary>
		Iw = 23,

		/// <summary>
		/// Asymmetrical welded I
		/// </summary>
		Iwn = 24,

		/// <summary>
		/// Welded T
		/// </summary>
		Tw = 25,

		/// <summary>
		/// Circular section
		/// </summary>
		O = 26,

		/// <summary>
		/// Rectangular
		/// </summary>
		Rect = 27,

		/// <summary>
		/// Massive asymetrical I
		/// </summary>
		Ign = 28,

		/// <summary>
		/// Massive symetrical I with web root chamfering
		/// </summary>
		Igh = 29,

		/// <summary>
		/// Massive T
		/// </summary>
		Tg = 30,

		/// <summary>
		/// Massive L
		/// </summary>
		Lg = 31,

		/// <summary>
		/// Mirrored Massive L
		/// </summary>
		LgMirrored = 32,

		/// <summary>
		/// Massive U
		/// </summary>
		Ug = 33,

		/// <summary>
		/// Massive circular hollow section
		/// </summary>
		CHSg = 34,

		/// <summary>
		/// Massive Z section
		/// </summary>
		Zg = 35,

		/// <summary>
		/// Massive Rectangular hollow section
		/// </summary>
		RHSg = 36,

		/// <summary>
		/// Oval shape
		/// </summary>
		Oval = 37,

		/// <summary>
		/// General cross-section
		/// </summary>
		General = 38,

		/// <summary>
		/// Pair rolled 2I
		/// </summary>
		Rolled2I = 39,

		/// <summary>
		/// Trapezoid
		/// </summary>
		Trapezoid = 40,

		/// <summary>
		/// Massive T with top flange haunch
		/// </summary>
		Ttfh = 41,

		/// <summary>
		/// Massive T with wall haunch
		/// </summary>
		Twh = 42,

		/// <summary>
		/// Massive T reverse
		/// </summary>
		Tgrev = 43,

		/// <summary>
		/// Massive T reverse with bottom flange haunch
		/// </summary>
		Ttfhrev = 44,

		/// <summary>
		/// Massive T reverse with wall haunch
		/// </summary>
		Twhrev = 45,

		/// <summary>
		/// Massive T chamfer 1
		/// </summary>
		Tchamfer1 = 46,

		/// <summary>
		/// Massive T chamfer 2
		/// </summary>
		Tchamfer2 = 47,

		/// <summary>
		/// TT section
		/// </summary>
		TT = 48,

		/// <summary>
		/// TT chamfer section
		/// </summary>
		TT1 = 49,

		/// <summary>
		/// Massive S section
		/// </summary>
		Sg = 50,

		/// <summary>
		/// General steel cross-section
		/// </summary>
		GeneralSteel = 51,

		/// <summary>
		/// General concrete cross-section
		/// </summary>
		GeneralConcrete = 52,

		/// <summary>
		/// Hollow cross-section with composite slab
		/// </summary>
		CompositeBeamBox = 53,

		/// <summary>
		/// Hollow cross-section with composite slab
		/// </summary>
		CompositeBeamBox1 = 54,

		/// <summary>
		/// I-shaped cross-section with composite slab
		/// </summary>
		CompositeBeamIgenT = 55,

		/// <summary>
		/// Composite beam cross-section - L left
		/// </summary>
		CompositeBeamLLeft = 56,

		/// <summary>
		/// Composite slab
		/// </summary>
		CompositeBeamPlate = 57,

		/// <summary>
		/// Composite beam cross-section - R res T
		/// </summary>
		CompositeBeamRResT = 58,

		/// <summary>
		/// Composite beam cross-section - R res T - 1
		/// </summary>
		CompositeBeamRResT1 = 59,

		/// <summary>
		/// Composite beam cross-section - R T
		/// </summary>
		CompositeBeamRT = 60,

		/// <summary>
		/// I-shaped cross-section with composite slab
		/// </summary>
		CompositeBeamShapeChamf = 61,

		/// <summary>
		/// Assymetrical I-shaped cross-section with composite slab
		/// </summary>
		CompositeBeamShapeChamfAssym = 62,

		/// <summary>
		/// I-shaped cross-section with composite slab
		/// </summary>
		CompositeBeamShapeIgen = 63,

		/// <summary>
		/// I-shaped cross-section with composite slab
		/// </summary>
		CompositeBeamShapeIT = 64,

		/// <summary>
		/// Assymetrical I-shaped cross-section with composite slab
		/// </summary>
		CompositeBeamShapeITAssym = 65,

		/// <summary>
		/// Composite beam cross-section -  T left
		/// </summary>
		CompositeBeamTLeft = 66,

		/// <summary>
		/// Trapezoidal cross-section with composite slab
		/// </summary>
		CompositeBeamTrapezoid = 67,

		/// <summary>
		/// T-shaped cross-section with composite slab
		/// </summary>
		CompositeBeamTresT = 68,

		/// <summary>
		/// T-shaped cross-section with composite slab
		/// </summary>
		CompositeBeamTrev = 69,

		/// <summary>
		/// Composite beam cross-section -  T rev res I
		/// </summary>
		CompositeBeamTrevResI = 70,

		/// <summary>
		/// Composite beam cross-section -  T rev res I - 1
		/// </summary>
		CompositeBeamTrevResI1 = 71,

		/// <summary>
		/// Composite beam cross-section -  T rev res R
		/// </summary>
		CompositeBeamTrevResR = 72,

		/// <summary>
		/// Composite beam cross-section -  T rev res R - 1
		/// </summary>
		CompositeBeamTrevResR1 = 73,

		/// <summary>
		/// Composite beam cross-section -  T rev T
		/// </summary>
		CompositeBeamTrevT = 74,

		/// <summary>
		/// Composite beam cross-section -  TT
		/// </summary>
		CompositeBeamShapeTT = 75,

		/// <summary>
		/// Beam cross-section -  I haunch chamfer
		/// </summary>
		BeamShapeIHaunchChamfer = 76,

		/// <summary>
		/// Beam cross-section -  I haunch chamfer assymetric
		/// </summary>
		BeamShapeIHaunchChamferAssym = 77,

		/// <summary>
		/// Beam cross-section -  Rev U
		/// </summary>
		BeamShapeRevU = 78,

		/// <summary>
		/// Beam cross-section -  Box
		/// </summary>
		BeamShapeBox = 79,

		/// <summary>
		/// Beam cross-section -  Box - 1
		/// </summary>
		BeamShapeBox1 = 80,

		/// <summary>
		/// Beam cross-section -  T rev Chamfer Haunch S
		/// </summary>
		BeamShapeTrevChamferHaunchS = 81,

		/// <summary>
		/// Beam cross-section -  T rev Chamfer Haunch D
		/// </summary>
		BeamShapeTrevChamferHaunchD = 82,

		/// <summary>
		/// Beam cross-section -  I rev Degen
		/// </summary>
		BeamShapeIrevDegen = 83,

		/// <summary>
		/// Beam cross-section -  I rev Degen Add
		/// </summary>
		BeamShapeIrevDegenAdd = 84,

		/// <summary>
		/// Beam cross-section -  T rev Degen
		/// </summary>
		BeamShapeTrevDegen = 85,

		/// <summary>
		/// Beam cross-section -  T rev Degen Add
		/// </summary>
		BeamShapeTrevDegenAdd = 86,

		/// <summary>
		/// Beam cross-section -  Z Degen
		/// </summary>
		BeamShapeZDegen = 87,

		/// <summary>
		/// Beam cross-section -  IZ Degen
		/// </summary>
		BeamShapeIZDegen = 88,

		/// <summary>
		/// Beam cross-section -  L Degen
		/// </summary>
		BeamShapeLDegen = 89,

		/// <summary>
		/// Cold formed thin-wall Z section
		/// </summary>
		CFZ = 90,

		/// <summary>
		/// Cold formed thin-wall Z section with lips
		/// </summary>
		CFZed = 91,

		/// <summary>
		/// Cold formed thin-wall sigma section
		/// </summary>
		CFSigma = 92,

		/// <summary>
		/// Cold formed thin-wall C section
		/// </summary>
		CFC = 93,

		/// <summary>
		/// Cold formed thin-wall Channel section
		/// </summary>
		CFU = 94,

		/// <summary>
		/// Cold formed thin-wall angle section
		/// </summary>
		CFL = 95,

		/// <summary>
		/// Cold formed thin-wall regular polygon
		/// </summary>
		CFRegPolygon = 96,

		/// <summary>
		/// Cold formed thin-wall omega section
		/// </summary>
		CFOmega = 97,

		/// <summary>
		/// Welded box - delta shape
		/// </summary>
		BoxDelta = 98,

		/// <summary>
		/// Welded box - triangular shape
		/// </summary>
		BoxTriangle = 99,

		/// <summary>
		/// Cold formed - Pair 2C - opened (back to back)
		/// </summary>
		CF2Co = 100,

		/// <summary>
		/// Parametric circular hollow section
		/// </summary>
		CHSPar = 101,

		/// <summary>
		/// Cold formed - general angle section
		/// </summary>
		CFLgen = 102,

		/// <summary>
		/// Cold formed - general shape
		/// </summary>
		CFGeneral = 103, // JAR 8.4.2019 - new general shape - cause of Tekla css import

		/// <summary>
		/// Cold formed thin-wall C+ section
		/// </summary>
		CFCp = 104,

		/// <summary>
		/// Cold formed - Pair 2C+ - opened (back to back)
		/// </summary>
		CF2Cpo = 105,

		/// <summary>
		/// Cold formed -  rectangular hollow section
		/// </summary>
		CFRhs = 106,

		/// <summary>
		/// Unique Name
		/// </summary>
		UniqueName = 1001,
	}

	[DataContract]
	public enum BoxDeltaAligment
	{
		/// <summary>
		/// Center
		/// </summary>
		Center = 0,

		/// <summary>
		/// Left
		/// </summary>
		Left = 1,

		/// <summary>
		/// Right
		/// </summary>
		Right = 2
	}

	/// <summary>
	/// Cross-section defined by parameters
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.CrossSection.CrossSection,CI.CrossSection", "CI.StructModel.Libraries.CrossSection.ICrossSection,CI.BasicTypes", typeof(CrossSection))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class CrossSectionParameter : CrossSection
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CrossSectionParameter()
		{
			Parameters = new List<Parameter>();
		}

		/// <summary>
		/// Type of cross-section
		/// </summary>
		public CrossSectionType CrossSectionType { get; set; }

		/// <summary>
		/// Parameters
		/// </summary>
		public List<Parameter> Parameters { get; set; }

		/// <summary>
		/// Material
		/// </summary>
		public ReferenceElement Material { get; set; }
	}
}