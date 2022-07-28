namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// CrossSectionFactory
	/// </summary>
	public static partial class CrossSectionFactory
	{
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
		public static void FillWeldedT(CrossSectionParameter css, double b, double h, double tw, double tf)
		{
			css.CrossSectionType = CrossSectionType.Tw;
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeWidth", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = tf });
		}

		/// <summary>
		/// Fill steel section of channel shape
		/// </summary>
		/// <param name="cssO">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">width</param>
		/// <param name="hw">depth</param>
		/// <param name="tw">web thickness</param>
		/// <param name="tf">flange thickness</param>
		/// <param name="rw">root radius</param>
		/// <param name="rf">flange edge rounding radius</param>
		/// <param name="taperF">Flange taper</param>
		public static void FillWeldedU(CrossSectionParameter cssO, double b, double hw, double tw, double tf, double rw, double rf, double taperF)
		{
			cssO.CrossSectionType = CrossSectionType.RolledUPar;
			cssO.Parameters.Add(new ParameterDouble() { Name = "b", Value = b });
			cssO.Parameters.Add(new ParameterDouble() { Name = "hw", Value = hw });
			cssO.Parameters.Add(new ParameterDouble() { Name = "tw", Value = tw });
			cssO.Parameters.Add(new ParameterDouble() { Name = "tf", Value = tf });
			cssO.Parameters.Add(new ParameterDouble() { Name = "rw", Value = rw });
			cssO.Parameters.Add(new ParameterDouble() { Name = "rf", Value = rf });
			cssO.Parameters.Add(new ParameterDouble() { Name = "tapperF", Value = taperF });
		}

		/// <summary>
		/// Fill steel section of angle shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="B">width</param>
		/// <param name="D">depth</param>
		/// <param name="t">thickness</param>
		/// <param name="rw">root radius</param>
		/// <param name="r2">toe radius</param>
		/// <param name="C">centroid position</param>
		/// <param name="mirrorZ">profile can be mirrored</param>
		public static void FillCssSteelAngle(CrossSectionParameter css, double B, double D, double t, double rw, double r2, double C, bool mirrorZ = false)
		{
			css.CrossSectionType = CrossSectionType.RolledAngle;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = B });
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = D });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
			css.Parameters.Add(new ParameterDouble() { Name = "rw", Value = rw });
			css.Parameters.Add(new ParameterDouble() { Name = "r2", Value = r2 });
			css.Parameters.Add(new ParameterDouble() { Name = "C", Value = C });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorZ", Value = mirrorZ });
		}

		/// <summary>
		/// Fill steel section of parametric I section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="cssGeomB">Css width</param>
		/// <param name="cssGeomH">Height of cross-section</param>
		/// <param name="cssGeomS">Web thickness</param>
		/// <param name="cssGeomT">Flange thickness</param>
		/// <param name="cssGeomR2">inside radius</param>
		/// <param name="tapperF">Flange tapper</param>
		/// <param name="r1">Flange edge rounding radius</param>
		/// <remarks>Dimension of parametric I section<br/>
		public static void FillCssIarc(CrossSectionParameter css, double cssGeomB, double cssGeomH, double cssGeomS, double cssGeomT, double cssGeomR2, double tapperF, double r1)
		{
			css.CrossSectionType = CrossSectionType.RolledI;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = cssGeomB });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = cssGeomH });
			css.Parameters.Add(new ParameterDouble() { Name = "s", Value = cssGeomS });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = cssGeomT });
			css.Parameters.Add(new ParameterDouble() { Name = "r2", Value = cssGeomR2 });
			css.Parameters.Add(new ParameterDouble() { Name = "tapperF", Value = tapperF });
			css.Parameters.Add(new ParameterDouble() { Name = "r1", Value = r1 });
		}

		/// <summary>
		/// Creates one item cross-section T - steel shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="cssGeomB">Css width</param>
		/// <param name="cssGeomH">Height of cross-section</param>
		/// <param name="cssGeomTw">Web thickness</param>
		/// <param name="cssGeomTf">Flange thickness</param>
		/// <param name="cssGeomR">Inside radius</param>
		/// <param name="cssGeomR1">Flange edge rounding radius</param>
		/// <param name="cssGeomR2">Web edge rounding radius</param>
		/// <param name="tapperF">Flange tapper</param>
		/// <param name="tapperW">Web tapper</param>
		public static void FillCssTarc(CrossSectionParameter css, double cssGeomB, double cssGeomH, double cssGeomTw, double cssGeomTf, double cssGeomR, double cssGeomR1, double cssGeomR2, double tapperF, double tapperW)
		{
			css.CrossSectionType = CrossSectionType.RolledT;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = cssGeomB });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = cssGeomH });
			css.Parameters.Add(new ParameterDouble() { Name = "Tw", Value = cssGeomTw });
			css.Parameters.Add(new ParameterDouble() { Name = "Tf", Value = cssGeomTf });
			css.Parameters.Add(new ParameterDouble() { Name = "R", Value = cssGeomR });
			css.Parameters.Add(new ParameterDouble() { Name = "R1", Value = cssGeomR1 });
			css.Parameters.Add(new ParameterDouble() { Name = "R2", Value = cssGeomR2 });
			css.Parameters.Add(new ParameterDouble() { Name = "tapperF", Value = tapperF });
			css.Parameters.Add(new ParameterDouble() { Name = "tapperW", Value = tapperW });
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
		/// Creates a ][ steel shape of css.
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="bt">The width of U top flange.</param>
		/// <param name="bb">The width of U bottom flange.</param>
		/// <param name="h">The height of U shape.</param>
		/// <param name="tb">The bottom deck thickness.</param>
		/// <param name="tl">The left deck thickness.</param>
		/// <param name="tr">The right deck thickness.</param>
		/// <param name="dis">Distance between ][</param>
		public static void FillShapeDblU(CrossSectionParameter css, double bt, double bb, double h, double tb, double tl, double tr, double dis)
		{
			css.CrossSectionType = CrossSectionType.RolledDoubleUo;
			css.Parameters.Add(new ParameterDouble() { Name = "Bt", Value = bt });
			css.Parameters.Add(new ParameterDouble() { Name = "Bb", Value = bb });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = tb });
			css.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = tl });
			css.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = tr });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = dis });
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
		public static void FillBox2(CrossSectionParameter css, double bu, double bb, double hw, double b1, double tw, double tfu, double tfb)
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

		/// <summary>
		/// Circular hollow section  - steel shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="D">Diameter</param>
		/// <param name="t">thickness</param>
		public static void FillCHSPar(CrossSectionParameter css, double D, double t)
		{
			css.CrossSectionType = CrossSectionType.CHSPar;
			css.Parameters.Add(new ParameterDouble() { Name = "R", Value = D / 2.0 });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
		}

		/// <summary>
		/// Create steel section Channel shape - steel shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="B">width</param>
		/// <param name="D">depth</param>
		/// <param name="tw">web thickness</param>
		/// <param name="tf">flange thickness</param>
		/// <param name="rw">root radius</param>
		/// <param name="rf">flange edge rounding radius</param>
		/// <param name="taperF">Flange taper</param>
		public static void FillCssSteelChannel(CrossSectionParameter css, double B, double D, double tw, double tf, double rw, double rf, double taperF)
		{
			css.CrossSectionType = CrossSectionType.RolledU;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = B });
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = D });
			css.Parameters.Add(new ParameterDouble() { Name = "tw", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "tf", Value = tf });
			css.Parameters.Add(new ParameterDouble() { Name = "rw", Value = rw });
			css.Parameters.Add(new ParameterDouble() { Name = "rf", Value = rf });
			css.Parameters.Add(new ParameterDouble() { Name = "tapperF", Value = taperF });
		}

		/// <summary>
		/// Fill cross-section of shape rectangular for steel sections
		/// </summary>
		/// <param name="cssO">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="D">depth</param>
		/// <param name="B">width</param>
		/// <param name="t">thickness</param>
		/// <param name="r1">inner radius</param>
		/// <param name="r2">outer radius</param>
		/// <param name="d">web buckling depth</param>
		public static void FillCssSteelRectangularHollow(CrossSectionParameter cssO, double D, double B, double t, double r1, double r2, double d)
		{
			cssO.CrossSectionType = CrossSectionType.RolledRHS;
			cssO.Parameters.Add(new ParameterDouble() { Name = "B", Value = B });
			cssO.Parameters.Add(new ParameterDouble() { Name = "D", Value = D });
			cssO.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
			cssO.Parameters.Add(new ParameterDouble() { Name = "r1", Value = r1 });
			cssO.Parameters.Add(new ParameterDouble() { Name = "r2", Value = r2 });
			cssO.Parameters.Add(new ParameterDouble() { Name = "d", Value = d });
		}

		/// <summary>
		/// Circular hollow section
		/// </summary>
		/// <param name="cssO"></param>
		/// <param name="r">Radius</param>
		/// <param name="t">Thickness</param>
		public static void FillCssSteelCircularHollow(CrossSectionParameter cssO, double r, double t)
		{
			cssO.CrossSectionType = CrossSectionType.RolledCHS;
			cssO.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
			cssO.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
		}

		/// <summary>
		/// Fill steel section Channel shape
		/// </summary>
		/// <param name="cssO">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="name">Name</param>
		/// <param name="Distance">Distance</param>
		public static void FillWelded2Uc(CrossSectionParameter cssO, string name, double Distance)
		{
			cssO.CrossSectionType = CrossSectionType.RolledDoubleUc;
			cssO.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = Distance });
		}

		/// <summary>
		/// Fill steel section Lt shape
		/// </summary>
		/// <param name="cssO">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="name">Name</param>
		/// <param name="Distance">Distance</param>
		public static void FillWelded2Lt(CrossSectionParameter cssO, string name, double Distance)
		{
			cssO.CrossSectionType = CrossSectionType.RolledDoubleLt;
			cssO.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = Distance });
		}

		/// <summary>
		/// Fill steel T profile made from part of I profile
		/// </summary>
		/// <param name="cssO">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="name">Name</param>
		/// <param name="H">Height</param>
		public static void FillSteelTI(CrossSectionParameter cssO, string name, double H)
		{
			cssO.CrossSectionType = CrossSectionType.RolledTI;
			cssO.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			cssO.Parameters.Add(new ParameterDouble() { Name = "H", Value = H });
		}

		/// <summary>
		/// Fill steel section Lu shape
		/// </summary>
		/// <param name="cssO">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="name">Name</param>
		/// <param name="Distance">Distance</param>
		public static void FillWelded2Lu(CrossSectionParameter cssO, string name, double Distance)
		{
			cssO.CrossSectionType = CrossSectionType.RolledDoubleLu;
			cssO.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = Distance });
		}

		/// <summary>
		/// Fill steel tube
		/// </summary>
		/// <param name="cssO">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="r">The radius of circle.</param>
		/// <param name="t">The thickness of wall.</param>
		public static void FillSteelTube(CrossSectionParameter cssO, double r, double t)
		{
			cssO.CrossSectionType = CrossSectionType.CHSPar;
			cssO.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
			cssO.Parameters.Add(new ParameterDouble() { Name = "T", Value = t });
			cssO.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
		}
	}
}