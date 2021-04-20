using IdeaRS.OpenModel.Geometry2D;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// CrossSectionFactory
	/// </summary>
	public partial class CrossSectionFactory
	{
		/// <summary>
		/// Fill parameters for cold formed Z section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="Width">Css width</param>
		/// <param name="Height">Height of cross-section</param>
		/// <param name="Thickness">Thickness</param>
		/// <param name="Radius">Inside radius</param>
		/// <param name="Mirror">Mirrored shape</param>
		/// <remarks>Dimension of cold formed Z section<br/>
		/// <img src="Images\CFZ.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 6;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());
		/// double Width = 0.15;
		/// double Height = 0.20;
		/// double Thickness = 0.003;
		/// double Radius = 0.005;
		/// bool Mirror = false;
		/// CrossSectionFactory.FillColdFormedZ(css, Width, Height, Thickness, Radius, Mirror);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>
		public static void FillColdFormedZ(CrossSectionParameter css, double Width, double Height, double Thickness, double Radius, bool Mirror)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.CFZ;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = Width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = Height });
			css.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = Thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = Radius });
			css.Parameters.Add(new ParameterBool() { Name = "Mirror", Value = Mirror });

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
		/// <remarks>Dimension of cold formed Z section<br/>
		/// <img src="Images\CFZ.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 6;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());
		/// double Width = 0.15;
		/// double Height = 0.20;
		/// double Thickness = 0.003;
		/// double Radius = 0.005;
		/// bool Mirror = false;
		/// CrossSectionFactory.FillColdFormedC(css, Width, Height, Thickness, Radius, Mirror);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>
		public static void FillColdFormedC(CrossSectionParameter css, double Width, double Height, double Thickness, double Radius, double lip)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.CFC;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = Width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = Height });
			css.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = Thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = Radius });
			css.Parameters.Add(new ParameterDouble() { Name = "Lip", Value = lip });
		}

		

		/// <summary>
		/// Fill parameters for cold formed Z-ed section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="Width">Css width</param>
		/// <param name="Height">Height of cross-section</param>
		/// <param name="Thickness">Thickness</param>
		/// <param name="Radius">Inside radius</param>
		/// <param name="Lip">Lip length</param>
		/// <param name="Mirror">Mirrored shape</param>
		/// <remarks>Dimension of cold formed Z-ed section<br/>
		/// <img src="Images\CFZed.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 7;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());
		/// double Width = 0.15;
		/// double Height = 0.20;
		/// double Thickness = 0.004;
		/// double Radius = 0.01;
		/// double Lip = 0.02;
		/// bool Mirror = false;
		/// CrossSectionFactory.FillColdFormedZed(css, Width, Height, Thickness, Radius, Lip, Mirror);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>
		public static void FillColdFormedZed(CrossSectionParameter css, double Width, double Height, double Thickness, double Radius, double Lip, bool Mirror)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.CFZed;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = Width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = Height });
			css.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = Thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = Radius });
			css.Parameters.Add(new ParameterDouble() { Name = "Lip", Value = Lip });
			css.Parameters.Add(new ParameterBool() { Name = "Mirror", Value = Mirror });

		}

		/// <summary>
		/// Fill parameters for cold formed Omega section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="Width">Css width</param>
		/// <param name="Height">Height of cross-section</param>
		/// <param name="FlangeWidth">Flange width</param>
		/// <param name="Thickness">Thickness</param>
		/// <param name="Radius">Inside radius</param>
		/// <remarks>Dimension of cold formed Omega section<br/>
		/// <img src="Images\CFOmega.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 8;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());
		/// double Width = 0.20;
		/// double Height = 0.10;
		/// double FlangeWidth = 0.10;
		/// double Thickness = 0.003;
		/// double Radius = 0.005;
		/// CrossSectionFactory.FillColdFormedOmega(css, Width, Height, FlangeWidth, Thickness, Radius);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>
		public static void FillColdFormedOmega(CrossSectionParameter css, double Width, double Height, double FlangeWidth, double Thickness, double Radius)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.CFOmega;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = Width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = Height });
			css.Parameters.Add(new ParameterDouble() { Name = "FlangeWidth", Value = FlangeWidth });
			css.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = Thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = Radius });
		}

		/// <summary>
		/// Fill parameters for cold formed L section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="Width">Horizontal leg length</param>
		/// <param name="Height">Vertical leg length</param>
		/// <param name="Thickness">Thickness</param>
		/// <param name="Radius">Inside radius</param>
		/// <remarks>Dimension of cold formed L section<br/>
		/// <img src="Images\CFL.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 9;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());
		/// double Width = 0.10;
		/// double Height = 0.10;
		/// double Thickness = 0.003;
		/// double Radius = 0.005;
		/// CrossSectionFactory.FillColdFormedL(css, Width, Height, Thickness, Radius);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>
		public static void FillColdFormedL(CrossSectionParameter css, double Width, double Height, double Thickness, double Radius)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.CFL;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = Width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = Height });
			css.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = Thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = Radius });
		}

		/// <summary>
		/// Fill parameters for cold formed L - gen section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="Width">Horizontal leg length</param>
		/// <param name="Height">Vertical leg length</param>
		/// <param name="Angle">Angle between legs</param>
		/// <param name="Thickness">Thickness</param>
		/// <param name="Radius">Inside radius</param>
		/// <remarks>Dimension of cold formed L - gen section<br/>
		/// <img src="Images\CFLgen.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 10;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());
		/// double Width = 0.10;
		/// double Height = 0.10;
		/// double Angle = Math.PI / 2.0;
		/// double Thickness = 0.003;
		/// double Radius = 0.005;
		/// CrossSectionFactory.FillColdFormedLgen(css, Width, Height, Angle, Thickness, Radius);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>
		public static void FillColdFormedLgen(CrossSectionParameter css, double Width, double Height, double Angle, double Thickness, double Radius)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.CFLgen;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = Width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = Height });
			css.Parameters.Add(new ParameterDouble() { Name = "Angle", Value = Angle });
			css.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = Thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = Radius });
		}

		/// <summary>
		/// Fill parameters for cold formed Regular polygon section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="Radius">Radius</param>
		/// <param name="Number">Number</param>
		/// <param name="Thickness">Thickness</param>
		/// <param name="InsideRadius">Inside radius</param>
		/// <remarks>Dimension of cold formed Regular polygon section<br/>
		/// <img src="Images\CFRegP.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 11;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());
		/// double Radius = 0.10;
		/// int Number = 8;
		/// double Thickness = 0.003;
		/// double InsideRadius = 0.005;
		/// CrossSectionFactory.FillColdFormedRegularPolygon(css, Radius, Number, Thickness, InsideRadius);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>
		public static void FillColdFormedRegularPolygon(CrossSectionParameter css, double Radius, int Number, double Thickness, double InsideRadius)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.CFRegPolygon;
			css.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = Radius });
			css.Parameters.Add(new ParameterInt() { Name = "Number", Value = Number });
			css.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = Thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "InsideRadius", Value = InsideRadius });
		}

		/// <summary>
		/// Fill parameters for cold formed sigma section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="Height">Height</param>
		/// <param name="Width">Width</param>
		/// <param name="Lip">Length of lip</param>
		/// <param name="Thickness">Thickness</param>
		/// <param name="InsideRadius">Inside radius</param>
		/// <param name="HeightMiddle">Height of middle part</param>
		/// <param name="HeightEdge">Height of edge part</param>
		/// <param name="Depth">Depth of web-fold</param>
		/// <remarks>Dimension of cold formed sigma section<br/>
		/// <img src="Images\CFSigma.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 12;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());
		/// double Height = 0.20;
		/// double Width = 0.15;
		/// double Lip = 0.02;
		/// double Thickness = 0.005;
		/// double InsideRadius = 0.010;
		/// double HeightMiddle = 0.10;
		/// double HeightEdge = 0.035;
		/// double Depth = 0.035;
		/// CrossSectionFactory.FillColdFormedSigma(css, Height, Width, Lip, Thickness, InsideRadius, HeightMiddle, HeightEdge, Depth);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>
		public static void FillColdFormedSigma(CrossSectionParameter css, double Height, double Width, double Lip, double Thickness, double InsideRadius, double HeightMiddle, double HeightEdge, double Depth)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.CFSigma;
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = Height });
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = Width });
			css.Parameters.Add(new ParameterDouble() { Name = "Lip", Value = Lip });
			css.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = Thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "InsideRadius", Value = InsideRadius });
			css.Parameters.Add(new ParameterDouble() { Name = "HeightMiddle", Value = HeightMiddle });
			css.Parameters.Add(new ParameterDouble() { Name = "HeightEdge", Value = HeightEdge });
			css.Parameters.Add(new ParameterDouble() { Name = "Depth", Value = Depth });
		}

		/// <summary>
		/// Fill parameters for cold formed RHS section
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="Height">Height</param>
		/// <param name="Width">Width</param>
		/// <param name="Thickness">Thickness</param>
		/// <param name="InsideRadius">Inside radius</param>
		/// <remarks>Dimension of cold formed RHS section<br/>
		/// <img src="Images\CFRHS.png" /> <br/>
		/// </remarks>
		/// <example> 
		/// This sample shows how to call this method./>
		/// <code lang = "C#">
		/// CrossSectionParameter css = new CrossSectionParameter();
		/// css.Id = 13;
		/// css.Material = new ReferenceElement(openStructModel.MatSteel.First());
		/// double Height = 0.20;
		/// double Width = 0.15;
		/// double Thickness = 0.005;
		/// double InsideRadius = 0.010;
		/// CrossSectionFactory.FillColdFormedRHS(css, Width, Height, Thickness, InsideRadius);
		/// openStructModel.AddObject(css);
		/// </code>
		/// </example>
		public static void FillColdFormedRHS(CrossSectionParameter css, double Height, double Width, double Thickness, double InsideRadius)
		{
			css.CrossSectionType = IdeaRS.OpenModel.CrossSection.CrossSectionType.RolledRHS;
			css.Parameters.Add(new ParameterDouble() { Name = "B", Value = Height});
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = Width });
			css.Parameters.Add(new ParameterDouble() { Name = "t", Value = Thickness });
			css.Parameters.Add(new ParameterDouble() { Name = "r1", Value = InsideRadius });
			css.Parameters.Add(new ParameterDouble() { Name = "r2", Value = InsideRadius * 2 });
			css.Parameters.Add(new ParameterDouble() { Name = "d", Value = 0.0 });
		}

		/// <summary>
		/// Fill center line for general cold formed css
		/// </summary>
		/// <param name="gcf"></param>		
		/// <param name="region2D">We need PolyLine2D from region2D</param>
		/// <param name="Thickness">Thickness</param>
		/// <param name="InsideRadius">Inside radius</param>
		public static void FillColdFormedGeneral(CrossSectionGeneralColdFormed gcf, Region2D region2D, double Thickness, double InsideRadius)
		{
			gcf.Centerline = region2D.Outline;
			gcf.Thickness = Thickness;
			gcf.Radius = InsideRadius;
		}

	}
}