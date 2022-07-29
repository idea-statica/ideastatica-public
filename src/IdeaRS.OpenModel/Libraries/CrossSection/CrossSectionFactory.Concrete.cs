namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// CrossSectionFactory
	/// </summary>
	public static partial class CrossSectionFactory
	{
		/// <summary>
		/// I shape base - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="h">Total height</param>
		/// <param name="btf">Width of the upper part</param>
		/// <param name="bbf">Width of the bottom part</param>
		/// <param name="htf">Thickness of the upper part</param>
		/// <param name="hbf">Thickness of the bottom  part</param>
		/// <param name="tw">Thinkness of the web</param>
		/// <param name="htfh"></param>
		/// <param name="hbfh"></param>
		public static void FillShapeIBase(CrossSectionParameter css, double h, double btf, double bbf, double htf, double hbf, double tw, double htfh, double hbfh)
		{
			css.CrossSectionType = CrossSectionType.Ign;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Bh", Value = btf });
			css.Parameters.Add(new ParameterDouble() { Name = "Bs", Value = bbf });
			css.Parameters.Add(new ParameterDouble() { Name = "Ts", Value = hbf });
			css.Parameters.Add(new ParameterDouble() { Name = "Th", Value = htf });
			css.Parameters.Add(new ParameterDouble() { Name = "Tw", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "Htfh", Value = htfh });
			css.Parameters.Add(new ParameterDouble() { Name = "Hbfh", Value = hbfh });
		}

		/// <summary>
		/// I shape - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="h">Total height</param>
		/// <param name="btf">Width of the upper part</param>
		/// <param name="bbf">Width of the bottom part</param>
		/// <param name="htf">Thickness of the upper part</param>
		/// <param name="hbf">Thickness of the bottom  part</param>
		/// <param name="tw">Thinkness of the web</param>
		public static void FillShapeI(CrossSectionParameter css, double h, double btf, double bbf, double htf, double hbf, double tw)
		{
			css.CrossSectionType = CrossSectionType.Ign;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Bh", Value = btf });
			css.Parameters.Add(new ParameterDouble() { Name = "Bs", Value = bbf });
			css.Parameters.Add(new ParameterDouble() { Name = "Ts", Value = hbf });
			css.Parameters.Add(new ParameterDouble() { Name = "Th", Value = htf });
			css.Parameters.Add(new ParameterDouble() { Name = "Tw", Value = tw });
		}

		/// <summary>
		/// Creates a new L shape css - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="h">The height of css.</param>
		/// <param name="b">The width of css.</param>
		/// <param name="th">The bottom flange thisckness.</param>
		/// <param name="sh">The wall thickness.</param>
		public static void FillShapeL(CrossSectionParameter css, double h, double b, double th, double sh)
		{
			css.CrossSectionType = CrossSectionType.Lg;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TH", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "SH", Value = sh });
		}

		/// <summary>
		/// T shape  - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">Width of shape</param>
		/// <param name="h">Height of shape</param>
		/// <param name="hf">Top flange width</param>
		/// <param name="bw">Wall width</param>
		public static void FillShapeT(CrossSectionParameter css, double b, double h, double hf, double bw)
		{
			css.CrossSectionType = CrossSectionType.Tg;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth", Value = hf });
			css.Parameters.Add(new ParameterDouble() { Name = "WallWidth", Value = bw });
		}

		/// <summary>
		/// T shape  - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">Width of shape</param>
		/// <param name="h">Height of shape</param>
		/// <param name="hf">Flange thickness</param>
		/// <param name="bwT">Web thickness top</param>
		/// <param name="bwB">Web thickness bottom</param>
		public static void FillShapeTwh(CrossSectionParameter css, double b, double h, double hf, double bwT, double bwB)
		{
			css.CrossSectionType = CrossSectionType.Twh;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = hf });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThicknessTop", Value = bwT });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThicknessBottom", Value = bwB });
		}

		/// <summary>
		/// TT shape  - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">Width of shape</param>
		/// <param name="h">Height of shape</param>
		/// <param name="hf">Top flange width</param>
		/// <param name="bw">Wall width</param>
		/// <param name="s">Spacing</param>
		public static void FillShapeTT(CrossSectionParameter css, double b, double h, double hf, double bw, double s)
		{
			css.CrossSectionType = CrossSectionType.Tg;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth", Value = hf });
			css.Parameters.Add(new ParameterDouble() { Name = "WallWidth", Value = bw });
			css.Parameters.Add(new ParameterDouble() { Name = "Spacing", Value = s });
		}

		/// <summary>
		/// T turned shape - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">Width of shape</param>
		/// <param name="h">Height of shape</param>
		/// <param name="hf">Top flange width</param>
		/// <param name="bw">Wall width</param>
		public static void FillShapeTrev(CrossSectionParameter css, double b, double h, double hf, double bw)
		{
			css.CrossSectionType = CrossSectionType.Tgrev;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth", Value = hf });
			css.Parameters.Add(new ParameterDouble() { Name = "WallWidth", Value = bw });
		}

		/// <summary>
		/// T turned shape - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">Width of shape</param>
		/// <param name="h">Height of shape</param>
		/// <param name="hf">Top flange width</param>
		/// <param name="bw">Wall width</param>
		/// <param name="hfh1"></param>
		public static void FillShapeTrev1(CrossSectionParameter css, double b, double h, double hf, double bw, double hfh1)
		{
			css.CrossSectionType = CrossSectionType.Tgrev;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth", Value = hf });
			css.Parameters.Add(new ParameterDouble() { Name = "WallWidth", Value = bw });
			css.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth1", Value = hfh1 });
		}

		/// <summary>
		/// Trapezoid shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="h">Height of shape</param>
		/// <param name="bt">Top width</param>
		/// <param name="bb">Bottom width</param>
		public static void FillShapeTrapezoid1(CrossSectionParameter css, double h, double bt, double bb)
		{
			css.CrossSectionType = CrossSectionType.Trapezoid;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "TopWidth", Value = bt });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomWidth", Value = bb });
		}

		/// <summary>
		/// Creates a U shape of css.
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="bt">The width of U top flange.</param>
		/// <param name="bb">The width of U bottom flange.</param>
		/// <param name="h">The height of U shape.</param>
		/// <param name="tb">The bottom deck thickness.</param>
		/// <param name="tl">The left deck thickness.</param>
		/// <param name="tr">The right deck thickness.</param>
		public static void FillShapeU(CrossSectionParameter css, double bt, double bb, double h, double tb, double tl, double tr)
		{
			css.CrossSectionType = CrossSectionType.Ug;
			css.Parameters.Add(new ParameterDouble() { Name = "Bt", Value = bt });
			css.Parameters.Add(new ParameterDouble() { Name = "Bb", Value = bb });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = tb });
			css.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = tl });
			css.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = tr });
		}

		/// <summary>
		/// Fill cross-section of shape rectangular for concrete sections
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="width">The width of css.</param>
		/// <param name="height">The height of css.</param>
		/// <param name="thickLeft">The thickness at the left side.</param>
		/// <param name="thickRight">The thickness at the rigth side.</param>
		/// <param name="thickTop">The thickness at the top.</param>
		/// <param name="thickBottom">The thickness at the bottom.</param>
		public static void FillCssRectangleHollow(CrossSectionParameter css, double width, double height, double thickLeft, double thickRight, double thickTop, double thickBottom)
		{
			css.CrossSectionType = CrossSectionType.RHSg;
			css.Parameters.Add(new ParameterDouble() { Name = "W", Value = width });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = height });
			css.Parameters.Add(new ParameterDouble() { Name = "Tl", Value = thickLeft });
			css.Parameters.Add(new ParameterDouble() { Name = "Tr", Value = thickRight });
			css.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = thickTop });
			css.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = thickBottom });
		}

		/// <summary>
		/// Fill massive pipe shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="r">The radius of circle.</param>
		/// <param name="t">The thickness of wall.</param>
		public static void FillOHollow(CrossSectionParameter css, double r, double t)
		{
			css.CrossSectionType = CrossSectionType.CHSg;
			css.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
			css.Parameters.Add(new ParameterDouble() { Name = "T", Value = t });
		}
	}
}