using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApiLink.BimApi;
namespace IdeaStatiCa.BimApiLink.Utils
{
	/// <summary>
	/// CrossSectionFactoryHelper
	/// </summary>
	static public class CrossSectionFactoryHelper
	{
		/// <summary>
		/// Rolled steel I section
		/// </summary>
		/// <param name="css">IdeaCrossSectionByParameters to fill</param>
		/// <param name="b">Width</param>
		/// <param name="h">Height</param>
		/// <param name="s">Web thickness</param>
		/// <param name="t">Flange thickness</param>
		/// <param name="r2">Inside radius</param>
		/// <param name="tapperF">Flange tapper</param>
		/// <param name="r1">Flange edge rounding radius</param>
		public static void FillRolledI(IdeaCrossSectionByParameters css, double b, double h, double s, double t, double r2, double tapperF, double r1)
		{
			css.Type = CrossSectionType.RolledI;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "s", Value = s });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
			css.Parameters.Add(new ParameterDouble() { Name = "r2", Value = r2 });
			css.Parameters.Add(new ParameterDouble() { Name = "tapperF", Value = tapperF });
			css.Parameters.Add(new ParameterDouble() { Name = "r1", Value = r1 });
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
		public static void FillRolledT(IdeaCrossSectionByParameters css, double B, double H, double Tw, double Tf, double R, double R1, double R2, double tapperF, double tapperW, bool mirrorY = false)
		{
			css.Type = CrossSectionType.RolledT;
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

		/// <summary>
		/// Steel circular hollow section
		/// </summary>
		/// <param name="css">CrossSectionParameter to fill</param>
		/// <param name="r">Radius</param>
		/// <param name="t">Thickness</param>
		public static void FillRolledCHS(IdeaCrossSectionByParameters css, double r, double t)
		{
			css.Type = CrossSectionType.RolledCHS;
			css.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
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
		/// <param name="MirrorZ">Z-axis mirror</param>
		public static void FillRolledChannel(IdeaCrossSectionByParameters css, double B, double D, double tw, double tf, double rw, double rf, double taperF, bool mirrorZ = false)
		{
			css.Type = CrossSectionType.RolledU;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = B });
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = D });
			css.Parameters.Add(new ParameterDouble() { Name = "tw", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "tf", Value = tf });
			css.Parameters.Add(new ParameterDouble() { Name = "rw", Value = rw });
			css.Parameters.Add(new ParameterDouble() { Name = "rf", Value = rf });
			css.Parameters.Add(new ParameterDouble() { Name = "tapperF", Value = taperF });
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
		public static void FillRolledAngle(IdeaCrossSectionByParameters css, double B, double D, double t, double rw, double r2, double C, bool mirrorZ = false, bool mirrorY = false)
		{
			css.Type = CrossSectionType.RolledAngle;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = B });
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = D });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
			css.Parameters.Add(new ParameterDouble() { Name = "rw", Value = rw });
			css.Parameters.Add(new ParameterDouble() { Name = "r2", Value = r2 });
			css.Parameters.Add(new ParameterDouble() { Name = "C", Value = C });
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
		public static void FillRolledRHS(IdeaCrossSectionByParameters css, double D, double B, double t, double r1, double r2, double d)
		{
			css.Type = CrossSectionType.RolledRHS;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = B });
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = D });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = t });
			css.Parameters.Add(new ParameterDouble() { Name = "r1", Value = r1 });
			css.Parameters.Add(new ParameterDouble() { Name = "r2", Value = r2 });
			css.Parameters.Add(new ParameterDouble() { Name = "d", Value = d });
		}

		/// <summary>
		/// Circular shape - for steel or concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="d">Diametrer of the shape</param>
		public static void FillCircle(IdeaCrossSectionByParameters css, double d)
		{
			css.Type = CrossSectionType.O;
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = d });
		}

		/// <summary>
		/// Rectangle shape - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		public static void FillRectangle(IdeaCrossSectionByParameters css, double width, double height)
		{
			css.Type = CrossSectionType.Rect;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		}

		/// <summary>
		/// Fill parameters for cold formed Z section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="width">Css width</param>
		/// <param name="height">Height of cross-section</param>
		/// <param name="thickness">Thickness</param>
		/// <param name="radius">Inside radius</param>
		/// <param name="mirrorZ">Mirrored shape</param>
		public static void FillColdFormedZ(IdeaCrossSectionByParameters css, double width, double height, double thickness, double radius, bool mirrorZ)
		{
			css.Type = CrossSectionType.CFZ;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
			css.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = radius });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorZ", Value = mirrorZ });
		}

		/// <summary>
		/// Fill parameters for cold formed C section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="Width">Css width</param>
		/// <param name="Height">Height of cross-section</param>
		/// <param name="Thickness">Thickness</param>
		/// <param name="Radius">Inside radius</param>
		/// <param name="lip">Lip length</param>
		public static void FillColdFormedC(IdeaCrossSectionByParameters css, double Width, double Height, double Thickness, double Radius, double lip, bool mirrorZ = false)
		{
			css.Type = CrossSectionType.CFC;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = Width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = Height });
			css.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = Thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = Radius });
			css.Parameters.Add(new ParameterDouble() { Name = "Lip", Value = lip });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorZ", Value = mirrorZ });
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
		/// <remarks>Dimension of assymetrical I section<br/>
		/// <img src="Images\Iasym.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 4;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());
		/// double bu = 0.150;
		/// double bb = 0.180;
		/// double hw = 0.320;
		/// double tw = 0.016;
		/// double tfu = 0.014;
		/// double tfb = 0.018;
		/// CrossSectionFactory.FillWeldedAsymI(css, bu, bb, hw, tw, tfu, tfb);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>
		public static void FillWeldedAsymI(IdeaCrossSectionByParameters css, System.Double bu, System.Double bb, System.Double hw, System.Double tw, System.Double tfu, System.Double tfb)
		{
			css.Type = IdeaRS.OpenModel.CrossSection.CrossSectionType.Iwn;
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeWidth", Value = bu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeWidth", Value = bb });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = hw });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeThickness", Value = tfu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeThickness", Value = tfb });
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
		public static void FillBox2(IdeaCrossSectionByParameters css, System.Double bu, System.Double bb, System.Double hw, System.Double b1, System.Double tw, System.Double tfu, System.Double tfb)
		{
			css.Type = IdeaRS.OpenModel.CrossSection.CrossSectionType.BoxFl;
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeWidth", Value = bu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeWidth", Value = bb });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = hw });
			css.Parameters.Add(new ParameterDouble() { Name = "WebDistance", Value = b1 });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeThickness", Value = tfu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeThickness", Value = tfb });
		}

		/// <summary>
		/// Fill steel section of welded I
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="bu">Width of flange</param>
		/// <param name="hw">Height of web</param>
		/// <param name="tw">Web thickness</param>
		/// <param name="tf">Flange thickness</param>
		/// <remarks>Dimension of welded I section<br/>
		/// <img src="Images\WeldedI.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 1;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());  // I have only one material, I take the first one
		/// double bu = 0.2;
		/// double hw = 0.4;
		/// double tw = 0.025;
		/// double tf = 0.02;
		/// CrossSectionFactory.FillWeldedI(css, bu, hw, tw, tf);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>

		public static void FillWeldedI(IdeaCrossSectionByParameters css, System.Double bu, System.Double hw, System.Double tw, System.Double tf)
		{
			css.Type = IdeaRS.OpenModel.CrossSection.CrossSectionType.Iw;
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeWidth", Value = bu });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = hw });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = tf });
		}

		/// <summary>
		/// Creates a new L shape css - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="h">The height of css.</param>
		/// <param name="b">The width of css.</param>
		/// <param name="th">The bottom flange thisckness.</param>
		/// <param name="sh">The wall thickness.</param>
		public static void FillShapeL(IdeaCrossSectionByParameters css, double h, double b, double th, double sh)
		{
			css.Type = CrossSectionType.Lg;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TH", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "SH", Value = sh });
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
		public static void FillComposedDblUo(IdeaCrossSectionByParameters css, double b, double h, double tw, double th, double distance)
		{
			css.Type = CrossSectionType.RolledDoubleUo;
			css.Parameters.Add(new ParameterDouble() { Name = "Bt", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = distance });
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
		public static void FillShapeI(IdeaCrossSectionByParameters css, double h, double btf, double bbf, double htf, double hbf, double tw)
		{
			css.Type = CrossSectionType.Ign;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Bh", Value = btf });
			css.Parameters.Add(new ParameterDouble() { Name = "Bs", Value = bbf });
			css.Parameters.Add(new ParameterDouble() { Name = "Ts", Value = hbf });
			css.Parameters.Add(new ParameterDouble() { Name = "Th", Value = htf });
			css.Parameters.Add(new ParameterDouble() { Name = "Tw", Value = tw });
		}

		/// <summary>
		/// T shape  - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="b">Width of shape</param>
		/// <param name="h">Height of shape</param>
		/// <param name="hf">Top flange width</param>
		/// <param name="bw">Wall width</param>
		public static void FillShapeT(IdeaCrossSectionByParameters css, double b, double h, double hf, double bw)
		{
			css.Type = CrossSectionType.Tg;
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
		public static void IdeaCrossSectionByParameters(IdeaCrossSectionByParameters css, double b, double h, double hf, double bwT, double bwB)
		{
			css.Type = CrossSectionType.Twh;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = hf });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThicknessTop", Value = bwT });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThicknessBottom", Value = bwB });
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
		public static void FillComposedDblLt(IdeaCrossSectionByParameters css, double h, double b, double th, double distance, bool shortLegUp = false, bool mirrorY = false)
		{
			css.Type = CrossSectionType.RolledDoubleLt;
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "TH", Value = th });
			css.Parameters.Add(new ParameterDouble() { Name = "Distance", Value = distance });
			css.Parameters.Add(new ParameterBool() { Name = "Mirror", Value = shortLegUp });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
		}

		/// <summary>
		/// Fill massive pipe shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="r">The radius of circle.</param>
		/// <param name="t">The thickness of wall.</param>
		public static void FillOHollow(IdeaCrossSectionByParameters css, double r, double t)
		{
			css.Type = CrossSectionType.CHSg;
			css.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
			css.Parameters.Add(new ParameterDouble() { Name = "T", Value = t });
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
		public static void FillCssRectangleHollow(IdeaCrossSectionByParameters css, double width, double height, double thickLeft, double thickRight, double thickTop, double thickBottom)
		{
			css.Type = CrossSectionType.RHSg;
			css.Parameters.Add(new ParameterDouble() { Name = "W", Value = width });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = height });
			css.Parameters.Add(new ParameterDouble() { Name = "Tl", Value = thickLeft });
			css.Parameters.Add(new ParameterDouble() { Name = "Tr", Value = thickRight });
			css.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = thickTop });
			css.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = thickBottom });
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
		public static void FillWeldedBoxFlange(IdeaCrossSectionByParameters css, double bu, double bb, double hw, double b1, double tw, double tfu, double tfb)
		{
			css.Type = CrossSectionType.BoxFl;
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeWidth", Value = bu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeWidth", Value = bb });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = hw });
			css.Parameters.Add(new ParameterDouble() { Name = "WebDistance", Value = b1 });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "UpperFlangeThickness", Value = tfu });
			css.Parameters.Add(new ParameterDouble() { Name = "BottomFlangeThickness", Value = tfb });
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
		public static void FillShapeU(IdeaCrossSectionByParameters css, double bt, double bb, double h, double tb, double tl, double tr)
		{
			css.Type = CrossSectionType.Ug;
			css.Parameters.Add(new ParameterDouble() { Name = "Bt", Value = bt });
			css.Parameters.Add(new ParameterDouble() { Name = "Bb", Value = bb });
			css.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = tb });
			css.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = tl });
			css.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = tr });
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
		public static void FillWeldedT(IdeaCrossSectionByParameters css, double b, double h, double tw, double tf, bool mirrorY = false)
		{
			css.Type = CrossSectionType.Tw;
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeWidth", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "WebHeight", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = tw });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = tf });
			css.Parameters.Add(new ParameterBool() { Name = "MirrorY", Value = mirrorY });
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
		public static void FillShapeTwh(IdeaCrossSectionByParameters css, double b, double h, double hf, double bwT, double bwB)
		{
			css.Type = CrossSectionType.Twh;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = h });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = b });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = hf });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThicknessTop", Value = bwT });
			css.Parameters.Add(new ParameterDouble() { Name = "WebThicknessBottom", Value = bwB });
		}
	}
}
