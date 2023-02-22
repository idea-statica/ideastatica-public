using IdeaRS.OpenModel.CrossSection;
using IdeaStatica.BimApiLink.BimApi;

namespace IdeaStatica.BimApiLink.Utils
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
		/// <param name="C">Centroid position</param
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
	}
}
