using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry2D;
using IdeaStatiCa.BimApiLink.Utils;
using System;
using System.Collections;
using Tekla.Structures.Catalogs;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utilities
{
	internal static class CssFactoryHelper
	{
		public static readonly string WidthKey = "WIDTH";
		public static readonly string HeightKey = "HEIGHT";
		public static readonly string TubeDiameterKey = "DIAMETER";
		private static readonly string FlangeThicknessKey = "FLANGE_THICKNESS";
		private static readonly string FlangeThicknessKey1 = "FLANGE_THICKNESS_1";
		private static readonly string FlangeWidthKey1 = "FLANGE_WIDTH_1";
		private static readonly string FlangeWidthKey2 = "FLANGE_WIDTH_2";
		private static readonly string WebThicknessKey = "WEB_THICKNESS";
		private static readonly string WebRadiusKey = "ROUNDING_RADIUS_1";
		private static readonly string FlangeRadiusKey = "ROUNDING_RADIUS_2";
		private static readonly string PltThicknessKey = "PLATE_THICKNESS";

		private static readonly string PlateThicknessKey = "PLATE_THICKNESS";
		private static readonly string RoundingRadiusKey = "ROUNDING_RADIUS";
		private static readonly string EdgeFoldKey = "EDGE_FOLD";

		private static readonly string EdgeFold1Key = "EDGE_FOLD_1";
		private static readonly string EdgeFold2Key = "EDGE_FOLD_2";

		public const double MinCssDimension = 5e-3;
		public static Hashtable GetCssProperties(LibraryProfileItem profileItem)
		{
			Hashtable cssProperties = new Hashtable();
			foreach (ProfileItemParameter profParam in profileItem.aProfileItemParameters)
			{
				if (cssProperties.ContainsKey(profParam.Property))
				{
					cssProperties[profParam.Property] = profParam.Value;
				}
				else
				{
					cssProperties.Add(profParam.Property, profParam.Value);
				}
			}
			return cssProperties;
		}

		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamCProfile(ParametricProfileItem paramProfileItem,
			ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_C_COLD_ROLLED)
			{
				CrossSectionParameter cssParameter = new CrossSectionParameter
				{
					Name = paramProfileItem.ProfilePrefix,
				};

				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par3Item.Value.MilimetersToMeters();
				double radius = par4Item.Value.MilimetersToMeters();
				var dWidth = par2Item.Value.MilimetersToMeters();

				CrossSectionFactory.FillColdFormedC(cssParameter, dWidth, dHeight, pltThickness, radius, 0.0, true);
				return cssParameter;
			}
			else
			{
				return null;
			}
		}

		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamCCProfile(ParametricProfileItem paramProfileItem,
			ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_CC_SYMMETRICAL)
			{
				CrossSectionParameter cssParameter = new CrossSectionParameter
				{
					Name = paramProfileItem.ProfilePrefix,
				};

				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double lip = par3Item.Value.MilimetersToMeters();
				var dWidth = par4Item.Value.MilimetersToMeters();

				CrossSectionFactory.FillColdFormedC(cssParameter, dWidth, dHeight, pltThickness, 0.1e-2, lip, true);
				return cssParameter;
			}
			else
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par5Item = paramProfileItem.aProfileItemParameters[4] as ProfileItemParameter;
				var par6Item = paramProfileItem.aProfileItemParameters[5] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double lipT = par3Item.Value.MilimetersToMeters();
				double widthT = par4Item.Value.MilimetersToMeters();
				double lipB = par5Item.Value.MilimetersToMeters();
				double widthB = par6Item.Value.MilimetersToMeters();

				Region2D region2D = CreateCenterline_CC(dHeight, widthT, widthB, pltThickness, lipT, lipB);
				CrossSectionGeneralColdFormed crossSectionGeneralColdFormed = new CrossSectionGeneralColdFormed()
				{
					Name = paramProfileItem.ProfilePrefix,
					CrossSectionType = CrossSectionType.CFGeneral,
				};
				CrossSectionFactory.FillColdFormedGeneral(crossSectionGeneralColdFormed, region2D, pltThickness, 0.001); // zde zkousim naplnit CF general
				return crossSectionGeneralColdFormed;
			}
		}

		// PROFILE_I
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertIProfile(LibraryProfileItem profileItem)
		{
			Hashtable cssProperties = GetCssProperties(profileItem);
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};

			var dWidth = ((double)cssProperties[WidthKey]).MilimetersToMeters();
			var dHeight = ((double)cssProperties[HeightKey]).MilimetersToMeters();
			double dFlange = ((double)cssProperties[FlangeThicknessKey]).MilimetersToMeters();
			double dWeb = ((double)cssProperties[WebThicknessKey]).MilimetersToMeters();

			if (profileItem.ProfileItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_I_WELDED_SYMMETRICAL)
			{
				dHeight -= (dFlange * 2);
				CrossSectionFactory.FillWeldedI(cssParameter, dWidth, dHeight, dWeb, dFlange);
			}
			else
			{
				double dRadiusWeb = ((double)cssProperties[WebRadiusKey]).MilimetersToMeters();
				double dRadiusFlange = ((double)cssProperties[FlangeRadiusKey]).MilimetersToMeters();
				CrossSectionFactory.FillRolledI(cssParameter, dWidth, dHeight, dWeb, dFlange, dRadiusWeb, 0.1e-2, dRadiusFlange);
			}
			return cssParameter;
		}

		// PROFILE_U:
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertUProfile(LibraryProfileItem profileItem)
		{
			Hashtable cssProperties = GetCssProperties(profileItem);
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};

			var dWidth = ((double)cssProperties[WidthKey]).MilimetersToMeters();
			var dHeight = ((double)cssProperties[HeightKey]).MilimetersToMeters();
			double dFlange = ((double)cssProperties[FlangeThicknessKey]).MilimetersToMeters();
			double dWeb = ((double)cssProperties[WebThicknessKey]).MilimetersToMeters();
			double dRadiusWeb = ((double)cssProperties[WebRadiusKey]).MilimetersToMeters();
			double dRadiusFlange = ((double)cssProperties[FlangeRadiusKey]).MilimetersToMeters();

			CrossSectionFactory.FillRolledChannel(cssParameter, dWidth, dHeight, dWeb, dFlange, dRadiusWeb, 0.1e-2, dRadiusFlange, true);
			return cssParameter;
		}

		// PROFILE_P:
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertPProfile(LibraryProfileItem profileItem)
		{
			Hashtable cssProperties = GetCssProperties(profileItem);
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};

			var dHeight = ((double)cssProperties[HeightKey]).MilimetersToMeters();
			var dWidth = dHeight;
			if (profileItem.ProfileItemSubType != ProfileItem.ProfileItemSubTypeEnum.PROFILE_P_SQUARE)
			{
				dWidth = ((double)cssProperties[WidthKey]).MilimetersToMeters();
			}
			double dFlangeThickness = ((double)cssProperties[PlateThicknessKey]).MilimetersToMeters();
			double dRoundingRadius = ((double)cssProperties[RoundingRadiusKey]).MilimetersToMeters();
			double r1 = (dRoundingRadius - dFlangeThickness > 0.001 ? dRoundingRadius - dFlangeThickness : 0.001);
			double r2 = (dRoundingRadius > 0.001 ? dRoundingRadius : 0.001);

			CrossSectionFactory.FillRolledRHS(cssParameter, dHeight, dWidth, dFlangeThickness, r1, r2, 0.001);
			return cssParameter;
		}

		// PROFILE_L
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertLProfile(LibraryProfileItem profileItem)
		{
			Hashtable cssProperties = GetCssProperties(profileItem);
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};

			var dWidth = ((double)cssProperties[WidthKey]).MilimetersToMeters();        //Section width
			var dHeight = ((double)cssProperties[HeightKey]).MilimetersToMeters();      //Section height
			double dFlangeThickness1 = ((double)cssProperties[FlangeThicknessKey1]).MilimetersToMeters();       //Flange thickness
			double dWebRadius = ((double)cssProperties[WebRadiusKey]).MilimetersToMeters();     //Flange thickness
			double dFlangeRadius = ((double)cssProperties[FlangeRadiusKey]).MilimetersToMeters();       //Flange thickness

			CrossSectionFactory.FillRolledAngle(cssParameter, dWidth, dHeight, dFlangeThickness1, dWebRadius, dFlangeRadius, 0.1e-2, false);
			return cssParameter;
		}

		// PROFILE_PD
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertPDProfile(LibraryProfileItem profileItem)
		{
			Hashtable cssProperties = GetCssProperties(profileItem);
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};

			var dWidth = ((double)cssProperties[TubeDiameterKey]).MilimetersToMeters();
			double thick = ((double)cssProperties[PltThicknessKey]).MilimetersToMeters();

			CrossSectionFactory.FillRolledCHS(cssParameter, 0.5 * dWidth, thick);
			return cssParameter;
		}

		//PROFILE_PL
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertPLProfile(LibraryProfileItem profileItem)
		{
			Hashtable cssProperties = GetCssProperties(profileItem);
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};

			//for DE env it shoud be like this. If next bugfix will need to change it. new solution how to read sizes need to be found
			var dWidth = ((double)cssProperties[WidthKey]).MilimetersToMeters();
			var dHeight = ((double)cssProperties[HeightKey]).MilimetersToMeters();

			CrossSectionFactory.FillRectangle(cssParameter, dWidth, dHeight);

			cssParameter.CrossSectionRotation = Math.PI / 2;
			return cssParameter;
		}



		//PROFILE_D
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertOProfile(LibraryProfileItem profileItem)
		{
			Hashtable cssProperties = GetCssProperties(profileItem);
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};

			var dWidth = ((double)cssProperties[TubeDiameterKey]).MilimetersToMeters();

			//test min width 
			dWidth = (dWidth >= MinCssDimension ? dWidth : MinCssDimension + 0.0001);

			CrossSectionFactory.FillCircle(cssParameter, dWidth);
			return cssParameter;
		}

		//PROFILE_T
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertTProfile(LibraryProfileItem profileItem)
		{
			Hashtable cssProperties = GetCssProperties(profileItem);
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};

			var dWidth = ((double)cssProperties[WidthKey]).MilimetersToMeters();
			var dHeight = ((double)cssProperties[HeightKey]).MilimetersToMeters();
			double webThickness = ((double)cssProperties[WebThicknessKey]).MilimetersToMeters();
			double flangethickness = ((double)cssProperties[FlangeThicknessKey]).MilimetersToMeters();
			double insideradius = ((double)cssProperties[WebRadiusKey]).MilimetersToMeters();
			double flangeEdgeRoundingRadius = ((double)cssProperties[FlangeRadiusKey]).MilimetersToMeters();
			double webEdgeRounding = 0.00001;

			insideradius = insideradius > 0.001 ? insideradius : 0.001;
			flangeEdgeRoundingRadius = (flangeEdgeRoundingRadius > 0.001 ? flangeEdgeRoundingRadius : 0.001);

			CrossSectionFactory.FillRolledT(cssParameter, dHeight, dWidth, webThickness, flangethickness, insideradius, flangeEdgeRoundingRadius, webEdgeRounding, 0.00001, 0.00001);
			return cssParameter;
		}

		//subtype PROFILE_PD
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamPDProfile(ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
			double diam = par1Item.Value.MilimetersToMeters();
			double thick = par2Item.Value.MilimetersToMeters();

			CrossSectionFactory.FillRolledCHS(cssParameter, 0.5 * diam, thick);
			return cssParameter;
		}

		// subtype PROFILE_P
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamPProfile(
			ParametricProfileItem paramProfileItem, ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			double thick = 0;
			double radius = 0;
			var dWidth = 0.0;
			var dHeight = 0.0;
			if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_P_RECTANGULAR)
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				dHeight = par1Item.Value.MilimetersToMeters();
				dWidth = par2Item.Value.MilimetersToMeters();
				thick = par3Item.Value.MilimetersToMeters();
				radius = par4Item.Value.MilimetersToMeters();
			}
			else
			{
				double diam = 0;
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				diam = par1Item.Value.MilimetersToMeters();
				thick = par2Item.Value.MilimetersToMeters();
				radius = 2 * thick;

				dWidth = diam;
				dHeight = diam;
			}

			CrossSectionFactory.FillRolledRHS(cssParameter, dHeight, dWidth, thick, radius, radius, 0);
			return cssParameter;
		}

		// subprofile PROFILE_D
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamOProfile(
			ParametricProfileItem paramProfileItem, ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var dWidth = par1Item.Value.MilimetersToMeters();
			//test min width 
			dWidth = (dWidth > MinCssDimension ? dWidth : MinCssDimension + 0.0001);
			CrossSectionFactory.FillCircle(cssParameter, dWidth);
			return cssParameter;
		}

		// subprofile  PROFILE_PL
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamPLProfile(
			ParametricProfileItem paramProfileItem)
		{
			//Tekla has different convention of plate 
			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
			var dHeight = 0.0;
			var dWidth = 0.0;

			//due too tekla different convention swap width and Height + add rotation
			if (par1Item.Property == HeightKey)
			{
				dHeight = par1Item.Value.MilimetersToMeters();
			}
			else
			{
				dWidth = par1Item.Value.MilimetersToMeters();
			}

			if (par2Item.Property == WidthKey)
			{
				dWidth = par2Item.Value.MilimetersToMeters();
			}
			else
			{
				dHeight = par2Item.Value.MilimetersToMeters();
			}

			IdeaRS.OpenModel.Geometry2D.Region2D compReg = new IdeaRS.OpenModel.Geometry2D.Region2D();

			IdeaRS.OpenModel.Geometry2D.PolyLine2D compOutline = new IdeaRS.OpenModel.Geometry2D.PolyLine2D();
			compReg.Outline = compOutline;

			var dWidth_2 = dWidth * 0.5;
			var dHeight_2 = dHeight * 0.5;


			compOutline.StartPoint = new Point2D() { X = -dWidth_2, Y = -dHeight_2 };

			compOutline.Segments.Add(new IdeaRS.OpenModel.Geometry2D.LineSegment2D() { EndPoint = new Point2D() { X = -dWidth_2, Y = dHeight_2 } });
			compOutline.Segments.Add(new IdeaRS.OpenModel.Geometry2D.LineSegment2D() { EndPoint = new Point2D() { X = dWidth_2, Y = dHeight_2 } });
			compOutline.Segments.Add(new IdeaRS.OpenModel.Geometry2D.LineSegment2D() { EndPoint = new Point2D() { X = dWidth_2, Y = -dHeight_2 } });

			//close segment
			if (compOutline.Segments.Count > 0 && compOutline.StartPoint != compOutline.Segments[compOutline.Segments.Count - 1].EndPoint)
			{
				compOutline.Segments.Add(new IdeaRS.OpenModel.Geometry2D.LineSegment2D() { EndPoint = compOutline.StartPoint });
			}
			CssComponent comp = new CssComponent
			{

				Phase = 0,
				Geometry = compReg,
				Id = 1,

			};
			IdeaRS.OpenModel.CrossSection.CrossSectionComponent css = new IdeaRS.OpenModel.CrossSection.CrossSectionComponent();
			css.Components.Add(comp);
			css.Name = paramProfileItem.ProfilePrefix;
			return css;
		}

		/// <summary>
		/// Convert PROFILE_UNKNOWN subtype TC - concrete T
		/// </summary>
		/// <param name="profileItem">LibraryProfileItem</param>
		/// <param name="cssProperties">unused</param>
		/// <param name="mat">Material</param>
		/// <param name="paramProfileItem">Profile params</param>
		/// <param name="paramProfItemSubType">unused</param>
		/// <param name="dWidth">Boundary width</param>
		/// <param name="dHeight">Boundary height</param>
		/// <returns>Create cross-section</returns>
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamTCProfile(ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			double dbw = 0.0;
			double dhf = 0.0;
			var dHeight = 0.0;
			var dWidth = 0.0;
			foreach (ProfileItemParameter parItem in paramProfileItem.aProfileItemParameters)
			{
				if (parItem.Property == "albl_Height")
				{
					dHeight = parItem.Value.MilimetersToMeters();
				}
				else if (parItem.Property == "albl_Width")
				{
					dWidth = parItem.Value.MilimetersToMeters();
				}
				else if (parItem.Property == "albl_Flange")
				{
					dhf = parItem.Value.MilimetersToMeters();
				}
				else if (parItem.Property == "albl_Web")
				{
					dbw = parItem.Value.MilimetersToMeters();
				}
			}

			cssParameter.CrossSectionRotation = Math.PI;

			CrossSectionFactory.FillShapeT(cssParameter, dWidth, dHeight, dhf, dbw);
			return cssParameter;
		}

		/// <summary>
		/// Convert PROFILE_UNKNOWN subtype II - concrete I
		/// </summary>
		/// <param name="profileItem">LibraryProfileItem</param>
		/// <param name="cssProperties">unused</param>
		/// <param name="mat">Material</param>
		/// <param name="paramProfileItem">Profile params</param>
		/// <param name="paramProfItemSubType">unused</param>
		/// <param name="dWidth">Boundary width</param>
		/// <param name="dHeight">Boundary height</param>
		/// <returns>Create cross-section</returns>
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamIIProfile(
			ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{


				Name = paramProfileItem.ProfilePrefix,
			};

			double dtf = 0.0;
			double dbf = 0.0;
			double htf = 0.0;
			double hbf = 0.0;
			double tw = 0.0;
			var dHeight = 0.0;
			foreach (ProfileItemParameter parItem in paramProfileItem.aProfileItemParameters)
			{
				if (parItem.Property == "albl_height_total")
				{
					dHeight = parItem.Value.MilimetersToMeters();
				}
				else if (parItem.Property == "albl_Web_thickness")
				{
					tw = parItem.Value.MilimetersToMeters();
				}
				else if (parItem.Property == "albl_Width_of_the_top_flange")
				{
					dtf = parItem.Value.MilimetersToMeters();
				}
				else if (parItem.Property == "albl_Width_of_the_bottom_flange")
				{
					dbf = parItem.Value.MilimetersToMeters();
				}
				else if (parItem.Property == "albl_Depth_of_the_top_flange")
				{
					htf = parItem.Value.MilimetersToMeters();
				}
				else if (parItem.Property == "albl_Depth_of_the_bottom_flange")
				{
					hbf = parItem.Value.MilimetersToMeters();
				}
			}

			cssParameter.CrossSectionRotation = 0.0;
			CrossSectionFactory.FillShapeI(cssParameter, dHeight, dtf, dbf, htf, hbf, tw);
			return cssParameter;
		}

		// subprofile PROFILE_L
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamLProfile(
			ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
			var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
			var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;

			var dHeight = par1Item.Value.MilimetersToMeters();
			var dWidth = par2Item.Value.MilimetersToMeters();
			double thickness = par3Item.Value.MilimetersToMeters();
			double rounding = par4Item.Value.MilimetersToMeters();

			CrossSectionFactory.FillRolledAngle(cssParameter, dWidth, dHeight, thickness, rounding, 0.5 * thickness, 0.1e-2, false);
			return cssParameter;
		}

		// subprofile PROFILE_I
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamIProfile(
			ParametricProfileItem paramProfileItem, ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_I_WELDED_UNSYMMETRICAL
														|| paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_I_WELDED_UNSYMMETRICAL2)
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par5Item = paramProfileItem.aProfileItemParameters[4] as ProfileItemParameter;
				var par6Item = paramProfileItem.aProfileItemParameters[5] as ProfileItemParameter;

				var dHeight = par1Item.Value.MilimetersToMeters();
				double fl1t = par3Item.Value.MilimetersToMeters();
				double fl1w = par4Item.Value.MilimetersToMeters();
				double fl2t = par5Item.Value.MilimetersToMeters();
				double fl2w = par6Item.Value.MilimetersToMeters();

				CrossSectionFactory.FillWeldedAsymI(cssParameter, fl1w, fl2w, dHeight - fl1t - fl2t, 0.014, 0.014, 0.014);
			}
			else
			{
				// parametric I profile
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par5Item = paramProfileItem.aProfileItemParameters[4] as ProfileItemParameter;

				var dHeight = par1Item.Value.MilimetersToMeters();
				var dWidth = par2Item.Value.MilimetersToMeters();
				double dWeb = par3Item.Value.MilimetersToMeters();
				double dFlange = par4Item.Value.MilimetersToMeters();
				double dRounding = par5Item.Value.MilimetersToMeters();

				if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_I_WELDED_SYMMETRICAL
					|| paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_I_WELDED_SYMMETRICAL2)
				{
					CrossSectionFactory.FillWeldedI(cssParameter, dWidth, dHeight - 2 * dFlange, dWeb, dFlange);
				}
				else
				{
					CrossSectionFactory.FillRolledI(cssParameter, dHeight, dWeb, dFlange, dRounding, 0.1e-2, dRounding, 0);
				}
			}

			return cssParameter;
		}

		// subprofile PROFILE_HK
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamHKProfile(
			ParametricProfileItem paramProfileItem, ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_HK_UNSYMMETRICAL)
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par5Item = paramProfileItem.aProfileItemParameters[4] as ProfileItemParameter;
				var par6Item = paramProfileItem.aProfileItemParameters[5] as ProfileItemParameter;
				var par7Item = paramProfileItem.aProfileItemParameters[6] as ProfileItemParameter;

				var dHeight = par1Item.Value.MilimetersToMeters();
				double wt = par2Item.Value.MilimetersToMeters();

				double fl1t = par3Item.Value.MilimetersToMeters();
				double fl1w = par4Item.Value.MilimetersToMeters();
				double fl2t = par5Item.Value.MilimetersToMeters();
				double fl2w = par6Item.Value.MilimetersToMeters();
				double cant = par7Item.Value.MilimetersToMeters();

				CrossSectionFactory.FillWeldedBoxFlange(cssParameter, fl1w, fl2w, dHeight - (fl1t + fl2t), fl2w - (2 * (cant + wt)), wt, fl1t, fl2t);
			}
			else if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_HK_SYMMETRICAL)
			{
				// symetrical
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par5Item = paramProfileItem.aProfileItemParameters[4] as ProfileItemParameter;

				var dHeight = par1Item.Value.MilimetersToMeters();
				var dWidth = par4Item.Value.MilimetersToMeters();

				double wt = par2Item.Value.MilimetersToMeters();
				double flt = par3Item.Value.MilimetersToMeters();
				double cant = par5Item.Value.MilimetersToMeters();
				CrossSectionFactory.FillWeldedBoxFlange(cssParameter, dWidth, dWidth, dHeight - (flt + flt), dWidth - (2 * (cant + wt)), wt, flt, flt);
			}
			else
			{
				string errMsg = string.Format("Not supported parametric profile '{0}' '{1}'", paramProfileItem.ToString(), paramProfItemSubType.ToString());
				throw new NotImplementedException(errMsg);
			}

			return cssParameter;
		}

		// subprofile PROFILE_ZZ
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamZZProfile(
			ParametricProfileItem paramProfileItem, ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_ZZ_SYMMETRICAL)
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double lip = par3Item.Value.MilimetersToMeters();
				var dWidth = par4Item.Value.MilimetersToMeters();
				const bool mirror = false;

				CrossSectionFactory.FillColdFormedZed(cssParameter, dWidth, dHeight, pltThickness, 0.001, lip, mirror);
			}
			else
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par6Item = paramProfileItem.aProfileItemParameters[5] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double widthT = par4Item.Value.MilimetersToMeters();
				double widthB = par6Item.Value.MilimetersToMeters();
				var dWidth = Math.Max(widthB, widthT);
				const bool mirror = false;

				CrossSectionFactory.FillColdFormedZ(cssParameter, dWidth, dHeight, pltThickness, 0.001, mirror);
			}

			return cssParameter;
		}

		// subprofile PROFILE_CW
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamCWProfile(
			ParametricProfileItem paramProfileItem, ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_CW_SYMMETRICAL)
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double lip = par3Item.Value.MilimetersToMeters();
				var dWidth = par4Item.Value.MilimetersToMeters();

				CrossSectionFactory.FillColdFormedRHS(cssParameter, dHeight, dWidth, pltThickness, lip);
			}
			else
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par8Item = paramProfileItem.aProfileItemParameters[7] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double lipT = par3Item.Value.MilimetersToMeters();
				double widthT = par4Item.Value.MilimetersToMeters();
				double widthB = par8Item.Value.MilimetersToMeters();
				var dWidth = Math.Max(widthB, widthT);

				CrossSectionFactory.FillColdFormedRHS(cssParameter, dHeight, dWidth, pltThickness, lipT);
			}

			return cssParameter;
		}

		// subprofile PROFILE_CU
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamCUProfile(
			ParametricProfileItem paramProfileItem, ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_CU_SYMMETRICAL)
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par5Item = paramProfileItem.aProfileItemParameters[4] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				var dWidth = par4Item.Value.MilimetersToMeters();
				double lip = par5Item.Value.MilimetersToMeters();

				CrossSectionFactory.FillColdFormedRHS(cssParameter, dHeight, dWidth, pltThickness, lip);
			}
			else
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par6Item = paramProfileItem.aProfileItemParameters[5] as ProfileItemParameter;
				var par7Item = paramProfileItem.aProfileItemParameters[6] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double widthT = par4Item.Value.MilimetersToMeters();
				double widthB = par6Item.Value.MilimetersToMeters();
				double lip = par7Item.Value.MilimetersToMeters();
				var dWidth = Math.Max(widthB, widthT);

				CrossSectionFactory.FillColdFormedRHS(cssParameter, dHeight, dWidth, pltThickness, lip);
			}

			return cssParameter;
		}

		// subprofile PROFILE_EB
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamEBProfile(
			ParametricProfileItem paramProfileItem, ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_EB_SYMMETRICAL)
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double lip = par3Item.Value.MilimetersToMeters();
				var dWidth = par4Item.Value.MilimetersToMeters();

				CrossSectionFactory.FillColdFormedRHS(cssParameter, dHeight, dWidth, pltThickness, lip);
			}
			else
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par6Item = paramProfileItem.aProfileItemParameters[5] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double lipT = par3Item.Value.MilimetersToMeters();
				double widthT = par4Item.Value.MilimetersToMeters();
				double widthB = par6Item.Value.MilimetersToMeters();
				var dWidth = Math.Max(widthB, widthT);

				CrossSectionFactory.FillColdFormedRHS(cssParameter, dHeight, dWidth, pltThickness, lipT);
			}

			return cssParameter;
		}

		// profile PROFILE_EC
		// profile PROFILE_CC
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertCCProfile(LibraryProfileItem profileItem)
		{

			Hashtable cssProperties = GetCssProperties(profileItem);
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};

			double dWidth = 0.0;
			if (cssProperties.ContainsKey(WidthKey))
			{
				dWidth = ((double)cssProperties[WidthKey]).MilimetersToMeters();
			}
			else
			{
				dWidth = Math.Min(((double)cssProperties[FlangeWidthKey1]).MilimetersToMeters(), ((double)cssProperties[FlangeWidthKey2]).MilimetersToMeters());
			}

			var dHeight = ((double)cssProperties[HeightKey]).MilimetersToMeters();
			double plateThickness = ((double)cssProperties[PlateThicknessKey]).MilimetersToMeters();

			double lip = 0.0;
			if (cssProperties.ContainsKey(EdgeFoldKey))
			{
				lip = ((double)cssProperties[EdgeFoldKey]).MilimetersToMeters();
			}
			else
			{
				lip = Math.Min(((double)cssProperties[EdgeFold1Key]).MilimetersToMeters(), ((double)cssProperties[EdgeFold2Key]).MilimetersToMeters());
			}

			CrossSectionFactory.FillColdFormedC(cssParameter, dWidth, dHeight, plateThickness, 0.1e-2, lip);

			return cssParameter;
		}

		// profile PROFILE_C+
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertCCPProfile(LibraryProfileItem profileItem)
		{

			Hashtable cssProperties = GetCssProperties(profileItem);
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};

			var dWidth = ((double)cssProperties["albl_Flange"]).MilimetersToMeters();
			var dHeight = ((double)cssProperties["albl_Height"]).MilimetersToMeters();
			double plateThickness = ((double)cssProperties["albl_Thickness"]).MilimetersToMeters();
			double lip1 = ((double)cssProperties["albl_Lip1"]).MilimetersToMeters();
			double lip2 = ((double)cssProperties["albl_Lip2"]).MilimetersToMeters();

			CrossSectionFactory.FillColdFormedCp(cssParameter, dWidth, dHeight, plateThickness, 0.1e-2, lip1, lip2, 0.1e-2);

			return cssParameter;
		}

		// subprofile PROFILE_ZZ
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertZZProfile(LibraryProfileItem profileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = profileItem.ProfileName,
			};
			Hashtable cssProperties = GetCssProperties(profileItem);

			var dWidth = ((double)cssProperties[FlangeWidthKey1]).MilimetersToMeters() + ((double)cssProperties[FlangeWidthKey2]).MilimetersToMeters();
			var dHeight = ((double)cssProperties[HeightKey]).MilimetersToMeters();
			double plateThickness = ((double)cssProperties[PlateThicknessKey]).MilimetersToMeters();

			double lip = Math.Min(((double)cssProperties[EdgeFold1Key]).MilimetersToMeters(), ((double)cssProperties[EdgeFold2Key]).MilimetersToMeters());

			CrossSectionFactory.FillColdFormedZed(cssParameter, dWidth, dHeight, plateThickness, 0.001, lip);
			return cssParameter;
		}


		// subprofile PROFILE_EC
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamECProfile(
			ParametricProfileItem paramProfileItem, ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			if (paramProfItemSubType == ProfileItem.ProfileItemSubTypeEnum.PROFILE_EC_SYMMETRICAL)
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double lip = par3Item.Value.MilimetersToMeters();
				var dWidth = par4Item.Value.MilimetersToMeters();

				CrossSectionFactory.FillColdFormedRHS(cssParameter, dHeight, dWidth, pltThickness, lip);
			}
			else
			{
				var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
				var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
				var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
				var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
				var par6Item = paramProfileItem.aProfileItemParameters[5] as ProfileItemParameter;
				var dHeight = par1Item.Value.MilimetersToMeters();
				double pltThickness = par2Item.Value.MilimetersToMeters();
				double lipT = par3Item.Value.MilimetersToMeters();
				double widthT = par4Item.Value.MilimetersToMeters();
				double widthB = par6Item.Value.MilimetersToMeters();
				var dWidth = Math.Max(widthB, widthT);

				CrossSectionFactory.FillColdFormedRHS(cssParameter, dHeight, dWidth, pltThickness, lipT);
			}

			return cssParameter;
		}

		// subprofile PROFILE_EE
		// subprofile PROFILE_EF
		// subprofile PROFILE_ED
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamEDOrEEOrEFProfile(
			ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
			var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
			var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
			var dHeight = par1Item.Value.MilimetersToMeters();
			double pltThickness = par2Item.Value.MilimetersToMeters();
			double lip = par3Item.Value.MilimetersToMeters();
			var dWidth = par4Item.Value.MilimetersToMeters();

			CrossSectionFactory.FillColdFormedRHS(cssParameter, dHeight, dWidth, pltThickness, lip);
			return cssParameter;
		}

		// subprofile PROFILE_EW
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamEWProfile(
			ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};
			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
			var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
			var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
			var par5Item = paramProfileItem.aProfileItemParameters[4] as ProfileItemParameter;
			var dHeight = par1Item.Value.MilimetersToMeters();
			double pltThickness = par2Item.Value.MilimetersToMeters();
			double lip = par3Item.Value.MilimetersToMeters();
			double widthT = par4Item.Value.MilimetersToMeters();
			double widthB = par5Item.Value.MilimetersToMeters();
			var dWidth = Math.Max(widthB, widthT);

			CrossSectionFactory.FillColdFormedRHS(cssParameter, dHeight, dWidth, pltThickness, lip);
			return cssParameter;
		}



		// subprofile PROFILE_EZ
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamEZProfile(
			ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
			var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
			var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
			var dHeight = par1Item.Value.MilimetersToMeters();
			double pltThickness = par2Item.Value.MilimetersToMeters();
			double lip = par3Item.Value.MilimetersToMeters();
			var dWidth = par4Item.Value.MilimetersToMeters();

			CrossSectionFactory.FillColdFormedZed(cssParameter, dWidth, dHeight, pltThickness, 0.1e-2, lip, false);
			return cssParameter;
		}

		//subprofile PROFILE_Z
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamZProfile(
			ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
			var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
			var dHeight = par1Item.Value.MilimetersToMeters();
			double pltThickness = par2Item.Value.MilimetersToMeters();
			var dWidth = par3Item.Value.MilimetersToMeters();
			const bool mirror = false;

			CrossSectionFactory.FillColdFormedZ(cssParameter, dWidth, dHeight, pltThickness, 0.1e-2, mirror);
			return cssParameter;
		}

		//subprofile PROFILE_Iwn
		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamIwnProfile(
			ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
			var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
			var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
			var dWidth = par1Item.Value.MilimetersToMeters();
			double webThickness = par3Item.Value.MilimetersToMeters();
			var dHeight = par2Item.Value.MilimetersToMeters();
			double flangeThickness = par4Item.Value.MilimetersToMeters();
			dHeight -= (flangeThickness * 2);
			CrossSectionFactory.FillWeldedAsymI(cssParameter, dWidth, dWidth, dHeight, webThickness, flangeThickness, flangeThickness);
			return cssParameter;
		}

		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamUChannelProfile(
			ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
			var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
			var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;
			var dHeight = par2Item.Value.MilimetersToMeters();
			double webThickness = par3Item.Value.MilimetersToMeters();
			var dWidth = par1Item.Value.MilimetersToMeters();
			double flangeThickness = par4Item.Value.MilimetersToMeters();
			CrossSectionFactory.FillRolledChannel(cssParameter, dWidth, dHeight, webThickness, flangeThickness, 0.00001, 0.1e-2, 0);
			return cssParameter;
		}

		internal static IdeaRS.OpenModel.CrossSection.CrossSection ConvertParamTarcProfile(
			ParametricProfileItem paramProfileItem)
		{
			CrossSectionParameter cssParameter = new CrossSectionParameter
			{
				Name = paramProfileItem.ProfilePrefix,
			};

			var par1Item = paramProfileItem.aProfileItemParameters[0] as ProfileItemParameter;
			var par2Item = paramProfileItem.aProfileItemParameters[1] as ProfileItemParameter;
			var par3Item = paramProfileItem.aProfileItemParameters[2] as ProfileItemParameter;
			var par4Item = paramProfileItem.aProfileItemParameters[3] as ProfileItemParameter;

			double webThickness = par3Item.Value.MilimetersToMeters();
			double flangeThickness = par4Item.Value.MilimetersToMeters();
			var dHeight = 0.0;
			var dWidth = 0.0;
			if (par1Item.Property == HeightKey)
			{
				dHeight = par1Item.Value.MilimetersToMeters();
			}
			else
			{
				dWidth = par1Item.Value.MilimetersToMeters();
			}

			if (par2Item.Property == WidthKey)
			{
				dWidth = par2Item.Value.MilimetersToMeters();
			}
			else
			{
				dHeight = par2Item.Value.MilimetersToMeters();
				dHeight -= (flangeThickness * 2);
			}

			CrossSectionFactory.FillRolledT(cssParameter, dWidth, dHeight, webThickness, flangeThickness, 0.00001, 0.00001, 0.00001, 0.00001, 0.00001);
			return cssParameter;
		}

		/// <summary>
		/// Create the centerline of CC shape. 
		/// </summary>
		/// <param name="height">Height</param>
		/// <param name="widthT">Top width</param>
		/// <param name="widthB">Bottom width</param>
		/// <param name="thickness">Thickness</param>
		/// <param name="lipT">Top lip</param>
		/// <param name="lipB">Bottom lip</param>
		/// <returns></returns>
		public static Region2D CreateCenterline_CC(double height, double widthT, double widthB, double thickness, double lipT, double lipB)
		{
			PolyLine2D centerline = new PolyLine2D
			{
				StartPoint = new Point2D() { X = widthT - (thickness * 0.5), Y = height - lipT }
			};

			LineSegment2D seg1 = new LineSegment2D
			{
				EndPoint = new Point2D() { X = widthT - (thickness * 0.5), Y = height - (thickness * 0.5) }
			};
			centerline.Segments.Add(seg1);

			LineSegment2D seg2 = new LineSegment2D
			{
				EndPoint = new Point2D() { X = thickness * 0.5, Y = height - (thickness * 0.5) }
			};
			centerline.Segments.Add(seg2);

			LineSegment2D seg3 = new LineSegment2D
			{
				EndPoint = new Point2D() { X = thickness * 0.5, Y = thickness * 0.5 }
			};
			centerline.Segments.Add(seg3);

			LineSegment2D seg4 = new LineSegment2D
			{
				EndPoint = new Point2D() { X = widthB - (thickness * 0.5), Y = thickness * 0.5 }
			};
			centerline.Segments.Add(seg4);

			LineSegment2D seg5 = new LineSegment2D
			{
				EndPoint = new Point2D() { X = widthB - (thickness * 0.5), Y = lipB }
			};
			centerline.Segments.Add(seg5);

			return new Region2D()
			{
				Outline = centerline,
			};
		}
	}
}
