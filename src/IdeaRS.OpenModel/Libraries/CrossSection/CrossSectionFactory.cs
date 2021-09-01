﻿namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// CrossSectionFactory
	/// </summary>
	public partial class CrossSectionFactory
	{
		/// <summary>
		/// Rectangle shape - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <returns></returns>
		public static void FillRectangle(CrossSectionParameter css, System.Double width, System.Double height)
		{
			//var css = new CrossSectionParameter();
			css.CrossSectionType = CrossSectionType.Rect;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
			//return ret;
		}

		/// <summary>
		/// Circular shape - for steel or concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="d">Diametrer of the shape</param>
		/// <returns></returns>
		public static void FillCircle(CrossSectionParameter css, System.Double d)
		{
			//var css = new CrossSectionParameter();
			css.CrossSectionType = CrossSectionType.O;
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = d });
			//return ret;
		}

		//TODO DUS Missing xml params
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
		public static void FillShapeIBase(CrossSectionParameter css, System.Double h, System.Double btf, System.Double bbf, System.Double htf, System.Double hbf, System.Double tw, System.Double htfh, System.Double hbfh)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.Ign;
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
		public static void FillShapeI(CrossSectionParameter css, System.Double h, System.Double btf, System.Double bbf, System.Double htf, System.Double hbf, System.Double tw)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.Ign;
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
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.Lg;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TH", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "SH", Value = sh });
		}

		/// <summary>
		/// Creates a new L shape css
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="h">The height of css.</param>
		/// <param name="b">The width of css.</param>
		/// <param name="th">The bottom flange thisckness.</param>
		/// <param name="sh">The wall thickness.</param>
		/// <param name="dis">Distance between _||_</param>
		public static void FillShapeDblL(CrossSectionParameter css, double h, double b, double th, double sh, double dis)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.RolledDoubleLt;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TH", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "SH", Value = sh });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = dis });
		}

		/// <summary>
		/// Creates a new L shape css.
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="h">The height of css.</param>
		/// <param name="b">The width of css.</param>
		/// <param name="th">The bottom flange thisckness.</param>
		/// <param name="sh">The wall thickness.</param>
		/// <param name="dis">Distance between _||_</param>
		public static void FillShapeDblLu(CrossSectionParameter css, double h, double b, double th, double sh, double dis)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.RolledDoubleLu;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TH", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "SH", Value = sh });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = dis });
		}


		/// <summary>
		/// T shape  - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">Width of shape</param>
		/// <param name="h">Height of shape</param>
		/// <param name="hf">Top flange width</param>
		/// <param name="bw">Wall width</param>
		public static void FillShapeT(CrossSectionParameter css, System.Double b, System.Double h, System.Double hf, System.Double bw)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.Tg;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth", Value = hf });
			css.Parameters.Add(new ParameterDouble() { Name = "WallWidth", Value = bw });
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
		public static void FillShapeTT(CrossSectionParameter css, System.Double b, System.Double h, System.Double hf, System.Double bw, System.Double s)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.Tg;
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
		public static void FillShapeTrev(CrossSectionParameter css, System.Double b, System.Double h, System.Double hf, System.Double bw)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.Tgrev;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth", Value = hf });
			css.Parameters.Add(new ParameterDouble() { Name = "WallWidth", Value = bw });
		}

		//TODO DUS Missing xml params
		/// <summary>
		/// T turned shape - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">Width of shape</param>
		/// <param name="h">Height of shape</param>
		/// <param name="hf">Top flange width</param>
		/// <param name="bw">Wall width</param>
		/// <param name="hfh1"></param>
		public static void FillShapeTrev1(CrossSectionParameter css, System.Double b, System.Double h, System.Double hf, System.Double bw, System.Double hfh1)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.Tgrev;
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
		public static void FillShapeTrapezoid1(CrossSectionParameter css, System.Double h, System.Double bt, System.Double bb)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.Trapezoid;
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
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.Ug;
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
		/// <param name="cssO">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="width">The width of css.</param>
		/// <param name="height">The height of css.</param>
		/// <param name="thickLeft">The thickness at the left side.</param>
		/// <param name="thickRight">The thickness at the rigth side.</param>
		/// <param name="thickTop">The thickness at the top.</param>
		/// <param name="thickBottom">The thickness at the bottom.</param>
		/// <returns></returns>
		public static void FillCssRectangleHollow(CrossSectionParameter cssO, double width, double height, double thickLeft, double thickRight, double thickTop, double thickBottom)
		{
			cssO.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.RHSg;
			cssO.Parameters.Add(new ParameterDouble() { Name = "W", Value = width });
			cssO.Parameters.Add(new ParameterDouble() { Name = "H", Value = height });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Tl", Value = thickLeft });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Tr", Value = thickRight });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = thickTop });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = thickBottom });
		}

		/// <summary>
		/// Fill massive pipe shape
		/// </summary>
		/// <param name="cssO">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="r">The radius of circle.</param>
		/// <param name="t">The thickness of wall.</param>
		/// <returns></returns>
		public static void FillOHollow(CrossSectionParameter cssO, double r, double t)
		{
			cssO.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.CHSg;
			cssO.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
			cssO.Parameters.Add(new ParameterDouble() { Name = "T", Value = t });
		}

		/// <summary>
		/// Fill general shape
		/// </summary>
		/// <param name="cssO">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="pt0">Cross-section point 0</param>
		/// <param name="pt1">Cross-section point 1</param>
		/// <param name="pt2">Cross-section point 2</param>
		/// <param name="pt3">Cross-section point 3</param>
		/// <param name="pt4">Cross-section point 4</param>
		/// <param name="pt5">Cross-section point 5</param>
		public static void FillGeneralShape(CrossSectionParameter cssO, double pt0, double pt1, double pt2, double pt3, double pt4, double pt5)
		{
			cssO.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.CHSg;
			cssO.Parameters.Add(new ParameterDouble() { Name = "Pt0", Value = pt0 });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Pt1", Value = pt1 });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Pt2", Value = pt2 });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Pt3", Value = pt3 });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Pt4", Value = pt4 });
			cssO.Parameters.Add(new ParameterDouble() { Name = "Pt5", Value = pt5 });
		}
	}
}