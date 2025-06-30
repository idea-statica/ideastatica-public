namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// CrossSectionFactory
	/// </summary>
	public static partial class CrossSectionFactory
	{
		/// <summary>
		/// Rolled steel I section
		/// </summary>
		/// <param name="css">CrossSectionParameter to fill</param>
		/// <param name="b">Width</param>
		/// <param name="h">Height</param>
		/// <param name="s">Web thickness</param>
		/// <param name="t">Flange thickness</param>
		/// <param name="r2">Inside radius</param>
		/// <param name="tapperF">Flange tapper</param>
		/// <param name="r1">Flange edge rounding radius</param>
		public static void FillRolledI(CrossSectionParameter css, double b, double h, double s, double t, double r2, double tapperF, double r1)
		{
			css.CrossSectionType = CrossSectionType.RolledI;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "s", Value = s });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
			css.Parameters.Add(new ParameterDouble() { Name = "r2", Value = r2 });
			css.Parameters.Add(new ParameterDouble() { Name = "tapperF", Value = tapperF });
			css.Parameters.Add(new ParameterDouble() { Name = "r1", Value = r1 });
		}

		public static void FillRolledI(CrossSectionParameter css, string name)
		{
			css.CrossSectionType = CrossSectionType.RolledI;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
		}


		/// <summary>
		/// Rolled steel double I section
		/// </summary>
		/// <param name="css">CrossSectionParameter to fill</param>
		/// <param name="name"></param>
		/// <param name="d"></param>
		/// <param name="h">Height</param>
		/// <param name="w">Width</param>
		/// <param name="wt">Web thickness</param>
		/// <param name="ft">Flange thickness</param>
		public static void FillRolledDoubleI(CrossSectionParameter css, string name, double d, double h, double w, double wt, double ft)
		{
			css.CrossSectionType = CrossSectionType.Rolled2I;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = d });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value =  w });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = wt });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = ft });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeTapper", Value = 0 });
			css.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = 0 });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeEdgeRadius", Value = 0 });
		}

		/// <summary>
		/// Rolled steel channel
		/// </summary>
		/// <param name="css">CrossSectionParameter to fill</param>
		/// <param name="B">Width</param>
		/// <param name="D">Depth</param>
		/// <param name="tw">Web thickness</param>
		/// <param name="tf">Flange thickness</param>
		/// <param name="rw">Root radius</param>
		/// <param name="rf">Flange edge radius</param>
		/// <param name="taperF">Flange taper</param>
		/// <param name="mirrorZ">Z-axis mirror</param>
		public static void FillRolledChannel(CrossSectionParameter css, double B, double D, double tw, double tf, double rw, double rf, double taperF, bool mirrorZ = false)
		{
			css.CrossSectionType = CrossSectionType.RolledU;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = B });
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = D });
			css.Parameters.Add(new ParameterDouble() { Name = "tw", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "tf", Value = tf });
			css.Parameters.Add(new ParameterDouble() { Name = "rw", Value = rw });
			css.Parameters.Add(new ParameterDouble() { Name = "rf", Value = rf });
			css.Parameters.Add(new ParameterDouble() { Name = "tapperF", Value = taperF });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorZ", Value = mirrorZ });
		}

		public static void FillRolledChannel(CrossSectionParameter css, string name, bool mirrorZ = false)
		{
			css.CrossSectionType = CrossSectionType.RolledU;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorZ", Value = mirrorZ });
		}

		/// <summary>
		/// Rolled steel angle
		/// </summary>
		/// <param name="css">CrossSectionParameter to fill</param>
		/// <param name="B">Width</param>
		/// <param name="D">Depth</param>
		/// <param name="t">Thickness</param>
		/// <param name="rw">Root radius</param>
		/// <param name="r2">Toe radius</param>
		/// <param name="C">Centroid position</param>
		/// <param name="mirrorZ">Z-axis mirror</param>
		/// <param name="mirrorY">Y-axis mirror</param>
		public static void FillRolledAngle(CrossSectionParameter css, double B, double D, double t, double rw, double r2, double C, bool mirrorZ = false, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.RolledAngle;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = B });
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = D });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
			css.Parameters.Add(new ParameterDouble() { Name = "rw", Value = rw });
			css.Parameters.Add(new ParameterDouble() { Name = "r2", Value = r2 });
			css.Parameters.Add(new ParameterDouble() { Name = "C", Value = C });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorZ", Value = mirrorZ });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		public static void FillRolledAngle(CrossSectionParameter css, string name, bool mirrorZ = false, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.RolledAngle;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorZ", Value = mirrorZ });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		/// <summary>
		/// Steel rectangular hollow section
		/// </summary>
		/// <param name="css">CrossSectionParameter to fill</param>
		/// <param name="D">depth</param>
		/// <param name="B">width</param>
		/// <param name="t">thickness</param>
		/// <param name="r1">inner radius</param>
		/// <param name="r2">outer radius</param>
		/// <param name="d">web buckling depth</param>
		public static void FillRolledRHS(CrossSectionParameter css, double D, double B, double t, double r1, double r2, double d)
		{
			css.CrossSectionType = CrossSectionType.RolledRHS;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = B });
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = D });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
			css.Parameters.Add(new ParameterDouble() { Name = "r1", Value = r1 });
			css.Parameters.Add(new ParameterDouble() { Name = "r2", Value = r2 });
			css.Parameters.Add(new ParameterDouble() { Name = "d", Value = d });
		}

		public static void FillRolledRHS(CrossSectionParameter css, string name)
		{
			css.CrossSectionType = CrossSectionType.RolledRHS;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
		}

		/// <summary>
		/// Steel circular hollow section
		/// </summary>
		/// <param name="css">CrossSectionParameter to fill</param>
		/// <param name="r">Radius</param>
		/// <param name="t">Thickness</param>
		public static void FillRolledCHS(CrossSectionParameter css, double r, double t)
		{
			css.CrossSectionType = CrossSectionType.RolledCHS;
			css.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
		}

		public static void FillRolledCHS(CrossSectionParameter css, string name)
		{
			css.CrossSectionType = CrossSectionType.RolledCHS;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
		}

		/// <summary>
		/// Rolled steel T section
		/// </summary>
		/// <param name="css">CrossSectionParameter to fill</param>
		/// <param name="B">Width</param>
		/// <param name="H">Height</param>
		/// <param name="Tw">Web thickness</param>
		/// <param name="Tf">Flange thickness</param>
		/// <param name="R">Inside radius</param>
		/// <param name="R1">Flange edge rounding radius</param>
		/// <param name="R2">Web edge rounding radius</param>
		/// <param name="tapperF">Flange tapper</param>
		/// <param name="tapperW">Web tapper</param>
		/// <param name="mirrorY">Y-axis mirror</param>
		public static void FillRolledT(CrossSectionParameter css, double B, double H, double Tw, double Tf, double R, double R1, double R2, double tapperF, double tapperW, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.RolledT;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = B });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = H });
			css.Parameters.Add(new ParameterDouble() { Name = "Tw", Value = Tw });
			css.Parameters.Add(new ParameterDouble() { Name = "Tf", Value = Tf });
			css.Parameters.Add(new ParameterDouble() { Name = "R", Value = R });
			css.Parameters.Add(new ParameterDouble() { Name = "R1", Value = R1 });
			css.Parameters.Add(new ParameterDouble() { Name = "R2", Value = R2 });
			css.Parameters.Add(new ParameterDouble() { Name = "tapperF", Value = tapperF });
			css.Parameters.Add(new ParameterDouble() { Name = "tapperW", Value = tapperW });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		public static void FillRolledT(CrossSectionParameter css, string name, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.RolledT;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		/// <summary>
		/// Rolled steel T section (I-cut)
		/// </summary>
		/// <param name="css">CrossSectionParameter to fill</param>
		/// <param name="name">Name</param>
		/// <param name="H">Height</param>
		/// <param name="mirrorY">Y-axis mirror</param>
		public static void FillRolledTFromI(CrossSectionParameter css, string name, double H, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.RolledTI;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = H });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		/// <summary>
		/// Fill steel section of welded I
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="bu">Width of flange</param>
		/// <param name="hw">Height of web</param>
		/// <param name="tw">Web thickness</param>
		/// <param name="tf">Flange thickness</param>
		public static void FillWeldedI(CrossSectionParameter css, double bu, double hw, double tw, double tf)
		{
			css.CrossSectionType = CrossSectionType.Iw;
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeWidth", Value = bu });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = hw });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = tf });
		}


		/// <summary>
		/// Fill steel section of welded T
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">Width of flange</param>
		/// <param name="h">Height of web</param>
		/// <param name="tw">Web thickness</param>
		/// <param name="tf">Flange thickness</param>
		/// <param name="mirrorY"></param>
		public static void FillWeldedT(CrossSectionParameter css, double b, double h, double tw, double tf, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.Tw;
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeWidth", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = tf });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		/// <summary>
		/// Welded I asymetrical - steel shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="bu">Width of upper flange</param>
		/// <param name="bb">Width of bottom flange</param>
		/// <param name="hw">Height of web</param>
		/// <param name="tw">Web thickness</param>
		/// <param name="tfu">Upper flange thickness</param>
		/// <param name="tfb">Bottom flange thickness</param>
		public static void FillWeldedAsymI(CrossSectionParameter css, double bu, double bb, double hw, double tw, double tfu, double tfb)
		{
			css.CrossSectionType = CrossSectionType.Iwn;
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeWidth", Value = bu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeWidth", Value = bb });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = hw });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeThickness", Value = tfu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeThickness", Value = tfb });
		}

		/// <summary>
		/// Fills a double channel steel section. They form an open/front-to-front ][ shape.
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">The width of css</param>
		/// <param name="h">The height of css</param>
		/// <param name="tw">Web thickness</param>
		/// <param name="th">Flange thickness</param>
		/// <param name="distance">Distance between ][</param>
		public static void FillComposedDblUo(CrossSectionParameter css, double b, double h, double tw, double th, double distance)
		{
			css.CrossSectionType = CrossSectionType.RolledDoubleUo;
			css.Parameters.Add(new ParameterDouble() { Name = "Bt", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = distance });
		}

		/// <summary>
		/// Fills a double channel steel section. They form an open/front-to-front ][ shape.
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="name">Name</param>
		/// <param name="distance">Distance</param>
		public static void FillComposedDblUo(CrossSectionParameter css, string name, double distance)
		{
			css.CrossSectionType = CrossSectionType.RolledDoubleUo;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = distance });
		}

		/// <summary>
		/// Fills a double channel steel section. They form an closed/back-to-back [] shape.
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">The width of css</param>
		/// <param name="h">The height of css</param>
		/// <param name="tw">Web thickness</param>
		/// <param name="th">Flange thickness</param>
		/// <param name="distance">Distance between ][</param>
		public static void FillComposedDblUc(CrossSectionParameter css, double b, double h, double tw, double th, double distance)
		{
			css.CrossSectionType = CrossSectionType.RolledDoubleUc;
			css.Parameters.Add(new ParameterDouble() { Name = "Bt", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = distance });
		}

		/// <summary>
		/// Fills a double channel steel section. They form a closed/back-to-back [] shape.
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="name">Name</param>
		/// <param name="distance">Distance</param>
		public static void FillComposedDblUc(CrossSectionParameter css, string name, double distance)
		{
			css.CrossSectionType = CrossSectionType.RolledDoubleUc;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = distance });
		}


		/// <summary>
		/// Creates a new L shape css
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="h">The height of css.</param>
		/// <param name="b">The width of css.</param>
		/// <param name="th">Thickness</param>
		/// <param name="distance">Distance between _||_</param>
		/// <param name="shortLegUp"></param>
		/// <param name="mirrorY"></param>
		public static void FillComposedDblLt(CrossSectionParameter css, double h, double b, double th, double distance, bool shortLegUp = false, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.RolledDoubleLt;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TH", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = distance });
			css.Parameters.Add(new ParameterBool() { Name = "Mirror", Value = shortLegUp });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}


		/// <summary>
		/// Fill steel section Lt shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="name">Name</param>
		/// <param name="distance">Distance</param>
		/// <param name="shortLegUp"></param>
		/// <param name="mirrorY"></param>
		public static void FillComposedDblLt(CrossSectionParameter css, string name, double distance, bool shortLegUp = false, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.RolledDoubleLt;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = distance });
			css.Parameters.Add(new ParameterBool() { Name = "Mirror", Value = shortLegUp });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		/// <summary>
		/// Creates a new L shape css.
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="h">The height of css.</param>
		/// <param name="b">The width of css.</param>
		/// <param name="th">Thickness</param>
		/// <param name="distance">Distance between _||_</param>
		/// <param name="shortLegUp"></param>
		/// <param name="mirrorY"></param>
		public static void FillComposedDblLu(CrossSectionParameter css, double h, double b, double th, double distance, bool shortLegUp = false, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.RolledDoubleLu;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TH", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = distance });
			css.Parameters.Add(new ParameterBool() { Name = "Mirror", Value = shortLegUp });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		/// <summary>
		/// Fill steel section Lu shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="name">Name</param>
		/// <param name="distance">Distance</param>
		/// <param name="shortLegUp"></param>
		/// <param name="mirrorY"></param>
		public static void FillComposedDblLu(CrossSectionParameter css, string name, double distance, bool shortLegUp = false, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.RolledDoubleLu;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = distance });
			css.Parameters.Add(new ParameterBool() { Name = "Mirror", Value = shortLegUp });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		/// <summary>
		/// Box 2 - steel shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="bu">Width of upper flange</param>
		/// <param name="bb">Width of bottom flange</param>
		/// <param name="hw">Height of web</param>
		/// <param name="b1">Web distance</param>
		/// <param name="tw">Web thickness</param>
		/// <param name="tfu">Upper flange thickness</param>
		/// <param name="tfb">Bottom flange thickness</param>
		public static void FillWeldedBoxFlange(CrossSectionParameter css, double bu, double bb, double hw, double b1, double tw, double tfu, double tfb)
		{
			css.CrossSectionType = CrossSectionType.BoxFl;
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeWidth", Value = bu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeWidth", Value = bb });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = hw });
			css.Parameters.Add(new ParameterDouble() { Name = "WebDistance", Value = b1 });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeThickness", Value = tfu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeThickness", Value = tfb });
		}

		public static void FillWeldedBoxWeb(CrossSectionParameter css, double bu, double bb, double hw, double b1, double tw, double tfu, double tfb, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.BoxWeb;
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeWidth", Value = bu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeWidth", Value = bb });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = hw });
			css.Parameters.Add(new ParameterDouble() { Name = "WebDistance", Value = b1 });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeThickness", Value = tfu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeThickness", Value = tfb });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		public static void FillWeldedBoxDelta(CrossSectionParameter css, double bu, double bb, double hw, double b1, double h, double tw, double tfu, double tfb, double overlap, BoxDeltaAligment aligment, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.BoxDelta;
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeWidth", Value = bu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeWidth", Value = bb });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = hw });
			css.Parameters.Add(new ParameterDouble() { Name = "WebDistance", Value = b1 });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeThickness", Value = tfu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeThickness", Value = tfb });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Overlap", Value = overlap });
			css.Parameters.Add(new ParameterString() { Name = "WebAlignment", Value = aligment.ToString() });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		public static void FillWeldedTriangle(CrossSectionParameter css, double h, double w, double fTh, double webTh, double webD, bool mirrorY = false)
		{
			css.CrossSectionType = CrossSectionType.BoxFl;
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = fTh });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "WebDistance", Value = webD });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = webTh });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = w });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}
	}
}