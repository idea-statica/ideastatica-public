using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.RamToIdea.Sections;
using System;

namespace IdeaStatiCa.RamToIdea.Utilities
{
	internal static class CssFactoryHelper
	{
		internal static double RadiusDefault = 0.1e-2;
		internal static void FillCssISection(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="h">Total height</param>
			/// <param name="btf">Width of the upper part</param>
			/// <param name="bbf">Width of the bottom part</param>
			/// <param name="htf">Thickness of the upper part</param>
			/// <param name="hbf">Thickness of the bottom  part</param>
			/// <param name="tw">Thinkness of the web</param>
			double h = props.Depth.InchesToMeters();
			double btf = props.BfTop.InchesToMeters();
			double bbf = props.BFBot.InchesToMeters();
			double htf = props.TfTop.InchesToMeters();
			double hbf = props.TFBot.InchesToMeters();
			double tw = props.WebT.InchesToMeters();

			CrossSectionFactory.FillShapeI(crossSectionParameter, h, btf, bbf, htf, hbf, tw);
		}

		internal static void FillCssIRolled(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="cssGeomB">Css width</param>
			/// <param name="cssGeomH">Height of cross-section</param>
			/// <param name="cssGeomS">Web thickness</param>
			/// <param name="cssGeomT">Flange thickness</param>
			/// <param name="cssGeomR2">inside radius</param>
			/// <param name="tapperF">Flange tapper</param>
			/// <param name="r1">Flange edge rounding radius</param>

			double cssGeomBh = props.BfTop.InchesToMeters();
			double cssGeomH = props.Depth.InchesToMeters();
			double cssGeomS = props.WebT.InchesToMeters();
			double cssGeomT = props.TfTop.InchesToMeters();
			double cssGeomR2 = RadiusDefault;
			double tapperF = RadiusDefault;
			double r1 = RadiusDefault;

			CrossSectionFactory.FillRolledI(crossSectionParameter, cssGeomBh, cssGeomH, cssGeomS, cssGeomT, cssGeomR2, tapperF, r1);
		}

		internal static void FillCssPipe(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="r">The radius of circle.</param>
			/// <param name="t">The thickness of wall.</param>

			double t = props.WebT.InchesToMeters();
			var diameter = props.Depth.InchesToMeters();
			CrossSectionFactory.FillRolledCHS(crossSectionParameter, diameter/2, t);
		}


		internal static void FillCssChannel(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="B">width</param>
			/// <param name="D">depth</param>
			/// <param name="tw">web thickness</param>
			/// <param name="tf">flange thickness</param>
			/// <param name="rw">root radius</param>
			/// <param name="rf">flange edge rounding radius</param>
			/// <param name="taperF">Flange taper</param>

			double B = props.BfTop.InchesToMeters();
			double D = props.Depth.InchesToMeters();
			double tw = props.WebT.InchesToMeters();
			double tf = Math.Max(props.TFBot.InchesToMeters(), props.TfTop.InchesToMeters());
			double rw = RadiusDefault;
			double rf = RadiusDefault;
			double taperF = RadiusDefault;

			CrossSectionFactory.FillRolledChannel(crossSectionParameter, B, D, tw, tf, rw, rf, taperF);
		}


		internal static void FillCssTube(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="D">depth</param>
			/// <param name="B">width</param>
			/// <param name="t">thickness</param>
			/// <param name="r1">inner radius</param>
			/// <param name="r2">outer radius</param>
			/// <param name="d">web buckling depth</param>

			double D = props.Depth.InchesToMeters();
			double B = props.BfTop.InchesToMeters();
			double t = props.WebT.InchesToMeters();
			double r1 = RadiusDefault;
			double r2 = RadiusDefault;
			double d = RadiusDefault;

			CrossSectionFactory.FillRolledRHS(crossSectionParameter, D, B, t, r1, r2, d);
		}


		internal static void FillCssCircle(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			double d = props.Depth.InchesToMeters();

			CrossSectionFactory.FillCircle(crossSectionParameter, d);
		}


		internal static void FillCssTsection(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="cssGeomB">Css width</param>
			/// <param name="cssGeomH">Height of cross-section</param>
			/// <param name="cssGeomTw">Web thickness</param>
			/// <param name="cssGeomTf">Flange thickness</param>
			/// <param name="cssGeomR">Inside radius</param>
			/// <param name="cssGeomR1">Flange edge rounding radius</param>
			/// <param name="cssGeomR2">Web edge rounding radius</param>
			/// <param name="tapperF">Flange tapper</param>
			/// <param name="tapperW">Web tapper</param>

			double cssGeomB = props.BfTop.InchesToMeters();
			double cssGeomH = props.Depth.InchesToMeters();
			double cssGeomTw = props.WebT.InchesToMeters();
			double cssGeomTf = Math.Max(props.TFBot.InchesToMeters(), props.TfTop.InchesToMeters());
			double cssGeomR = RadiusDefault;
			double cssGeomR1 = RadiusDefault;
			double cssGeomR2 = RadiusDefault;
			double tapperF = RadiusDefault;
			double tapperW = RadiusDefault;

			CrossSectionFactory.FillRolledT(crossSectionParameter, cssGeomB, cssGeomH, cssGeomTw, cssGeomTf, cssGeomR, cssGeomR1, cssGeomR2, tapperF, tapperW);
		}

		internal static void FillCssRectangle(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="width">Width of the rectangle</param>
			/// <param name="height">Height of the rectangle</param>

			double width = props.BfTop.InchesToMeters();
			double height = props.Depth.InchesToMeters();

			CrossSectionFactory.FillRectangle(crossSectionParameter, width, height);
		}

		internal static void FillCssLSection(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="h">The height of css.</param>
			/// <param name="b">The width of css.</param>
			/// <param name="th">The bottom flange thisckness.</param>
			/// <param name="sh">The wall thickness.</param>

			double h = props.Depth.InchesToMeters();
			double b = props.BfTop.InchesToMeters();
			double th = Math.Max(props.TFBot.InchesToMeters(), props.TfTop.InchesToMeters());
			double sh = props.WebT.InchesToMeters();

			CrossSectionFactory.FillShapeL(crossSectionParameter, h, b, th, sh);
		}

		internal static void FillCssISectionWithHaunch(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="cssGeomB">Css width</param>
			/// <param name="cssGeomH">Height of cross-section</param>
			/// <param name="cssGeomS">Web thickness</param>
			/// <param name="cssGeomT">Flange thickness</param>
			/// <param name="cssGeomR2">inside radius</param>
			/// <param name="tapperF">Flange tapper</param>
			/// <param name="r1">Flange edge rounding radius</param>

			double cssGeomBh = props.BfTop.InchesToMeters();
			double cssGeomH = props.Depth.InchesToMeters();
			double cssGeomS = props.WebT.InchesToMeters();
			double cssGeomT = Math.Max(props.TFBot.InchesToMeters(), props.TfTop.InchesToMeters());
			double cssGeomR2 = RadiusDefault;
			double tapperF = RadiusDefault;
			double r1 = RadiusDefault;

			CrossSectionFactory.FillRolledI(crossSectionParameter, cssGeomBh, cssGeomH, cssGeomS, cssGeomT, cssGeomR2, tapperF, r1);
		}

		internal static void FillCssAngle(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="B">width</param>
			/// <param name="D">depth</param>
			/// <param name="t">thickness</param>
			/// <param name="rw">root radius</param>
			/// <param name="r2">toe radius</param>
			/// <param name="C">centroid position</param>

			double B = props.BfTop.InchesToMeters();
			double D = props.Depth.InchesToMeters();
			double t = Math.Max(props.TFBot.InchesToMeters(), props.TfTop.InchesToMeters());
			double rw = RadiusDefault;
			double r2 = RadiusDefault;
			double C = 0.0;

			CrossSectionFactory.FillRolledAngle(crossSectionParameter, B, D, t, rw, r2, C);
		}

		internal static void FillCssColdFormedZee(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="Width">Css width</param>
			/// <param name="Height">Height of cross-section</param>
			/// <param name="Thickness">Thickness</param>
			/// <param name="Radius">Inside radius</param>
			/// <param name="Mirror">Mirrored shape</param>

			double Width = props.BfTop.InchesToMeters();
			double Height = props.Depth.InchesToMeters();
			double Thickness = props.WebT.InchesToMeters();
			double Radius = RadiusDefault;

			CrossSectionFactory.FillColdFormedZ(crossSectionParameter, Width, Height, Thickness, Radius, false);
		}

		internal static void FillCssColdFormedChannelWithLips(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="Width">Css width</param>
			/// <param name="Height">Height of cross-section</param>
			/// <param name="Thickness">Thickness</param>
			/// <param name="Radius">Inside radius</param>
			/// <param name="lip">Lip length</param>

			double Width = props.BfTop.InchesToMeters();
			double Height = props.Depth.InchesToMeters();
			double Thickness = props.WebT.InchesToMeters();
			double Radius = RadiusDefault;
			double lip = RadiusDefault;
			CrossSectionFactory.FillColdFormedC(crossSectionParameter, Width, Height, Thickness, Radius, lip);
		}

		internal static void FillCssColdFormedChannel(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="Width">Css width</param>
			/// <param name="Height">Height of cross-section</param>
			/// <param name="Thickness">Thickness</param>
			/// <param name="Radius">Inside radius</param>
			/// <param name="lip">Lip length</param>

			double Width = props.BfTop.InchesToMeters();
			double Height = props.Depth.InchesToMeters();
			double Thickness = props.WebT.InchesToMeters();
			double Radius = RadiusDefault;
			double lip = RadiusDefault;
			CrossSectionFactory.FillColdFormedChannel(crossSectionParameter, Width, Height, Thickness, Radius);
		}

		internal static void FillShapeDblL(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="h">The height of css.</param>
			/// <param name="b">The width of css.</param>
			/// <param name="th">The bottom flange thisckness.</param>
			/// <param name="sh">The wall thickness.</param>
			/// <param name="dis">Distance between _||_</param>

			double b = props.BFBot.InchesToMeters();
			double h = props.Depth.InchesToMeters();
			double sh = props.WebT.InchesToMeters();
			double th = Math.Max(props.TFBot.InchesToMeters(), props.TfTop.InchesToMeters());
			double dis = props.BfTop.InchesToMeters() - props.BFBot.InchesToMeters() * 2;

			CrossSectionFactory.FillComposedDblLt(crossSectionParameter, h, b, th, sh, dis);
		}

		public static void FillShapeDblLu(SteelSectionProperties props, CrossSectionParameter crossSectionParameter)
		{
			/// <param name="h">The height of css.</param>
			/// <param name="b">The width of css.</param>
			/// <param name="th">The bottom flange thisckness.</param>
			/// <param name="sh">The wall thickness.</param>
			/// <param name="dis">Distance between _||_</param>

			double b = props.BFBot.InchesToMeters();
			double h = props.Depth.InchesToMeters();
			double sh = props.WebT.InchesToMeters();
			double th = Math.Max(props.TFBot.InchesToMeters(), props.TfTop.InchesToMeters());
			double dis = props.BfTop.InchesToMeters() - props.BFBot.InchesToMeters() * 2;

			CrossSectionFactory.FillComposedDblLu(crossSectionParameter, h, b, th, sh, dis);
		}
	}
}
