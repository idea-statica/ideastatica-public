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
	}
}
