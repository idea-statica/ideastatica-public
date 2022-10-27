using Dlubal.RSTAB6;
using Dlubal.RSTAB8;
using IdeaRS.OpenModel.CrossSection;
using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Factories.RstabPluginUtils;
using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;
using System.Globalization;

using IOM = IdeaRS.OpenModel.CrossSection;

using PM = IdeaRstabPlugin.Factories.RstabParamName;

namespace IdeaRstabPlugin.Factories
{
	/// <summary>
	/// Factory class for <see cref="IIdeaCrossSection"/>.
	/// </summary>
	internal class CrossSectionFactory : IFactory<Dlubal.RSTAB8.ICrossSection, IIdeaCrossSection>
	{
		private const DB_CRSC_PROPERTY_ID CssDbPropertyType = (DB_CRSC_PROPERTY_ID)189;

		/// <summary>
		/// Creates <see cref="IIdeaCrossSection"/> instance from a given <see cref="ICrossSection"/>
		/// </summary>
		/// <param name="objectFactory">IObjectFactory instance</param>
		/// <param name="importSession"></param>
		/// <returns>IIdeaCrossSection instance</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown in case of incomplete data in RSTAB TextId property</exception>
		/// <param name="css">RSTAB cross-section</param>

		public IIdeaCrossSection Create(IObjectFactory objectFactory, IImportSession importSession, Dlubal.RSTAB8.ICrossSection css)
		{
			Dlubal.RSTAB8.CrossSection cssData = css.GetData();

			string[] cssDataTextIdSeparated = cssData.TextID.Split(' ');

			// Cross section type name
			string cssType = cssDataTextIdSeparated[0];

			// Argument out of range guard
			if (cssDataTextIdSeparated.Length == 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			IList<double> parameters = GetArrayParams(cssDataTextIdSeparated[1]);

			// Mapping RSTAB parameters
			RstabParamsMap pm = MapRstabParams(parameters);

			var crossSectionParameter = new CrossSectionParameter();

			switch (cssType)
			{
				case "Rechteck":
					IOM.CrossSectionFactory.FillRectangle(crossSectionParameter, pm.Get(PM.B), pm.Get(PM.H));
					break;

				case "Kreis":
				case "RD":
					IOM.CrossSectionFactory.FillCircle(crossSectionParameter, pm.Get(PM.D));
					break;

				case "Rundstahl":
					IOM.CrossSectionFactory.FillCircle(crossSectionParameter, pm.Get(PM.D) * 1000.0);
					break;

				case "PB":
					IOM.CrossSectionFactory.FillShapeTrev(crossSectionParameter, pm.Get(PM.B), pm.Get(PM.H), pm.Get(PM.Hf), pm.Get(PM.Bw));
					break;

				case "UZ":
					IOM.CrossSectionFactory.FillShapeT(crossSectionParameter, pm.Get(PM.B), pm.Get(PM.H), pm.Get(PM.Hf), pm.Get(PM.Bw));
					break;

				case "ITU":
					IOM.CrossSectionFactory.FillShapeI(crossSectionParameter, pm.Get(PM.H), pm.Get(PM.Btf), pm.Get(PM.Bbf), pm.Get(PM.Htf), pm.Get(PM.Hbf), pm.Get(PM.Tw));
					break;

				case "ITS":
					IOM.CrossSectionFactory.FillShapeI(crossSectionParameter, pm.Get(PM.H), pm.Get(PM.Btf), pm.Get(PM.Bbf), pm.Get(PM.Htf), pm.Get(PM.Hbf), pm.Get(PM.Tw));
					break;

				case "SBD":
					IOM.CrossSectionFactory.FillShapeIBase(crossSectionParameter, pm.Get(PM.H), pm.Get(PM.Btf), pm.Get(PM.Bbf), pm.Get(PM.Htf), pm.Get(PM.Hbf), pm.Get(PM.Tw), pm.Get(7) - pm.Get(6), pm.Get(3) - pm.Get(2));
					break;

				case "SBE":
					IOM.CrossSectionFactory.FillShapeTrev1(crossSectionParameter, pm.Get(PM.B), pm.Get(PM.H), pm.Get(PM.Hf), pm.Get(PM.Bw), pm.Get(3) - pm.Get(2));
					break;

				case "SB":
					IOM.CrossSectionFactory.FillShapeTrapezoid1(crossSectionParameter, pm.Get(PM.H), pm.Get(PM.Bt), pm.Get(PM.Bb));
					break;

				case "UM":
					IOM.CrossSectionFactory.FillShapeTT(crossSectionParameter, pm.Get(PM.B), pm.Get(PM.H), pm.Get(PM.Hf), pm.Get(PM.Bw), pm.Get(PM.B) - pm.Get(PM.Bw) - pm.Get(PM.Bw));
					break;

				case "Ring":
					IOM.CrossSectionFactory.FillOHollow(crossSectionParameter, pm.Get(PM.R) * 0.5, pm.Get(PM.T));
					break;

				case "HK":
					IOM.CrossSectionFactory.FillCssRectangleHollow(crossSectionParameter, pm.Get(PM.B), pm.Get(PM.H), pm.Get(PM.Tb), pm.Get(PM.Tt), pm.Get(PM.Tl), pm.Get(PM.Tr));
					break;

				case "FBU":
				case "PBU":
				case "UZU":
					if ((Math.Abs(pm.Get(1)) < 1e-6) || (Math.Abs(pm.Get(2)) < 1e-6))
					{
						IOM.CrossSectionFactory.FillShapeL(crossSectionParameter, pm.Get(PM.H), pm.Get(1) + pm.Get(2) + pm.Get(5), pm.Get(PM.Th), pm.Get(PM.Sh));
					}
					else
					{
						IOM.CrossSectionFactory.FillGeneralShape(crossSectionParameter, pm.Get(0), pm.Get(1), pm.Get(2), pm.Get(3), pm.Get(4), pm.Get(5));
					}
					break;

				case "IS":
					CssHelperFactory.FillCssIw(crossSectionParameter, css);
					break;

				case "IU":
					CssHelperFactory.FillCssIwn(crossSectionParameter, css);
					break;

				case "Kasten(A)":
				case "Kasten(B)":
					CssHelperFactory.FillCssBoxFl(crossSectionParameter, css);
					break;

				case "TS":
					CssHelperFactory.CreateWeldedT(crossSectionParameter, css);
					break;

				case "I":
					CssHelperFactory.FillCssI(crossSectionParameter, css);
					break;

				case "U":
					CssHelperFactory.FillCssU(crossSectionParameter, css);
					break;

				case "L":
					CssHelperFactory.FillCssL(crossSectionParameter, css);
					break;

				case "RHS":
					CssHelperFactory.FillCssRHS(crossSectionParameter, css);
					break;

				case "Rohr":
				case "CHS":
					CssHelperFactory.FillCssCHS(crossSectionParameter, css);
					break;

				case "2I":
				case "2UR":
				case "2UV":
				case "2UK":
				case "2L(A)":
				case "2LA":
				case "2LB":
				case "2L(B)":
				case "2L(BA)":
				case "2L(BB)":
				case "TO":
				case "DICKQ":
				default:
					//if (!ResolveThinwalledCssShapeV3(crossSectionParameter, css))
					if (cssType.StartsWith("2") || !TryImportCssThinWalled(crossSectionParameter, css))
					{
						var cssComponent = CssHelperFactory.CreateCrossSectionByComponents(css, objectFactory);
						if (cssComponent != null)
						{
							return cssComponent;
						}
					}
					break;
			}

			var cssParametric = new RstabCrossSectionParametric()
			{
				Id = $"css-{cssData.No}",
				Name = cssData.Description,
				Material = objectFactory.GetMaterial(cssData.MaterialNo),
				Rotation = 0,
				Type = crossSectionParameter.CrossSectionType,
				Parameters = new HashSet<Parameter>(crossSectionParameter.Parameters)
			};

			return cssParametric;
		}

		private bool TryImportCssThinWalled(CrossSectionParameter crossSectionParameter, Dlubal.RSTAB8.ICrossSection css)
		{
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();
			double type = cssDB.rsGetProperty((DB_CRSC_PROPERTY_ID)CssDbPropertyType);

			// basic cross-section type
			/*
			 *  0.0 = Unknown type
			 *  1.0 = I-Sections
			 *  2.0 = Channels
			 *  3.0 = T-Sections
			 *  4.0 = Angles
			 *  5.0 = Square and Rectangular Hollow Sections
			 *  6.0 = Pipes
			 *  7.0 = Z-Sections
			 *  8.0 = Solid Sections
			 *  9.0 = Rail Sections
			 * 10.0 = Corrugated Sheets
			 * 11.0 = Elliptical Hollow Sections
			 * 12.0 = 60 Degree Angles
			 *
			 */

			switch (type)
			{
				case 1.0:
					CssHelperFactory.FillCssI(crossSectionParameter, css);
					break;

				case 2.0:
					CssHelperFactory.FillCssU(crossSectionParameter, css);
					break;

				case 4.0:
					CssHelperFactory.FillCssL(crossSectionParameter, css);
					break;

				case 5.0:
					CssHelperFactory.FillCssRHS(crossSectionParameter, css);
					break;

				case 6.0:
					CssHelperFactory.FillCssCHS(crossSectionParameter, css);
					break;

				default:
					return false;
			}

			return true;
		}

		/// <summary>
		/// Method transpiled from: ^RSTABSupport.StructuralModelFromRstab.GetArrayParams(String ^strList, char separator)
		/// </summary>
		/// <param name="rstabParamData">Specific part of RSTAB TextID string</param>
		/// <returns>RSTAB cross-sections geom parameters</returns>
		private IList<double> GetArrayParams(string rstabParamData)
		{
			string[] dataSeparated = rstabParamData.Split('/');

			CultureInfo ci = new CultureInfo("en-US", false);
			NumberFormatInfo numberFormatInfo = ci.NumberFormat;
			numberFormatInfo.NumberDecimalSeparator = ".";
			numberFormatInfo.NumberGroupSeparator = "";

			IList<double> parameters = new List<double>();

			for (int i = 0; i < dataSeparated.Length; i++)
			{
				if (double.TryParse(dataSeparated[i], NumberStyles.Any, numberFormatInfo, out double param))
				{
					param *= 0.001;
					parameters.Add(param);
				}
			}

			return parameters;
		}

		/// <summary>
		/// Parameters mapping function
		/// </summary>
		/// <param name="parameters">List of parameters taken from RSTAB</param>
		/// <returns>RstabParamsMap instance</returns>
		private RstabParamsMap MapRstabParams(IList<double> parameters)
		{
			var rstabParamsMap = new RstabParamsMap(parameters);
			rstabParamsMap.RegisterParam(1, PM.B);
			rstabParamsMap.RegisterParam(0, PM.H);
			rstabParamsMap.RegisterParam(0, PM.D);
			rstabParamsMap.RegisterParam(2, PM.Hf);
			rstabParamsMap.RegisterParam(3, PM.Bw);
			rstabParamsMap.RegisterParam(4, PM.Btf);
			rstabParamsMap.RegisterParam(1, PM.Bbf);
			rstabParamsMap.RegisterParam(2, PM.Htf);
			rstabParamsMap.RegisterParam(5, PM.Hbf);
			rstabParamsMap.RegisterParam(3, PM.Tw);
			rstabParamsMap.RegisterParam(1, PM.Bt);
			rstabParamsMap.RegisterParam(0, PM.Bb);
			rstabParamsMap.RegisterParam(0, PM.R);
			rstabParamsMap.RegisterParam(1, PM.T);
			rstabParamsMap.RegisterParam(4, PM.Tb);
			rstabParamsMap.RegisterParam(5, PM.Tt);
			rstabParamsMap.RegisterParam(2, PM.Tl);
			rstabParamsMap.RegisterParam(3, PM.Tr);
			rstabParamsMap.RegisterParam(4, PM.Th);
			rstabParamsMap.RegisterParam(5, PM.Sh);
			rstabParamsMap.RegisterParam(6, PM.Htfh1);   //Paramter for calculating hautop = params[7] - params[6];
			rstabParamsMap.RegisterParam(7, PM.Htfh2);   //Paramter for calculating hautop = params[7] - params[6];

			return rstabParamsMap;
		}
	}
}