using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi.Library;
using IdeaStatiCa.TeklaStructuresPlugin.Utilities;
using System.Collections.Generic;
using Tekla.Structures.Catalogs;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class CrossSectionImporter : BaseImporter<IIdeaCrossSection>
	{
		public CrossSectionImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaCrossSection Create(string id)
		{
			PlugInLogger.LogInformation($"CrossSectionImporter create {id}");
			var cssMat = id.Split(';');
			ProfileItem profileType = Model.GetCrossSection(cssMat[0]);

			IIdeaCrossSection css = GetCrossSectionByLibraryItem(profileType, cssMat[0], cssMat[1]);

			if (css != null)
			{
				return css;
			}

			css = GetCrossSectionByParametricItem(profileType, cssMat[0], cssMat[1]);
			return css ?? GetCrossSectionByName(cssMat[0], cssMat[1]);
		}

		private static CrossSectionByName GetCrossSectionByName(string cssName, string materialNo)
		{
			return new CrossSectionByName(cssName)
			{
				MaterialNo = materialNo,
				Name = cssName,
			};
		}

		public CrossSectionByParameters GetCrossSectionByLibraryItem(ProfileItem profile, string cssId, string materialNo)
		{
			if (profile is LibraryProfileItem libraryProfileItem)
			{
				var iomCss = ProcessLibraryCss(libraryProfileItem);
				if (iomCss is IdeaRS.OpenModel.CrossSection.CrossSectionParameter cssParam)
				{
					PlugInLogger.LogDebug($"Create CrossSectionByParameters {libraryProfileItem.ProfileName}");
					return new CrossSectionByParameters(cssId)
					{
						MaterialNo = materialNo,
						Name = libraryProfileItem.ProfileName,
						Parameters = new HashSet<IdeaRS.OpenModel.CrossSection.Parameter>(cssParam.Parameters),
						Type = cssParam.CrossSectionType,
					};
				}
				else if (iomCss != null)
				{
					PlugInLogger.LogError($"Unexpected type of  CrossSection {iomCss}");
				}
			}

			return null;

		}

		private IIdeaCrossSection GetCrossSectionByParametricItem(ProfileItem profile, string cssName, string materialNo)
		{
			if (profile is ParametricProfileItem parametricProfileItem)
			{
				var iomCss = ProcessParametricCss(parametricProfileItem);
				if (iomCss is IdeaRS.OpenModel.CrossSection.CrossSectionParameter cssParam)
				{
					PlugInLogger.LogDebug($"Create CrossSectionByParameters {parametricProfileItem.ProfilePrefix}");
					return new CrossSectionByParameters(cssName)
					{
						MaterialNo = materialNo,
						Name = cssName,
						Parameters = new HashSet<IdeaRS.OpenModel.CrossSection.Parameter>(cssParam.Parameters),
						Type = cssParam.CrossSectionType
					};
				}
				else if (iomCss is IdeaRS.OpenModel.CrossSection.CrossSectionComponent cssComponent)
				{
					PlugInLogger.LogDebug($"Create CrossSectionComponent {cssName}");
					var ideaCssComponent = new CrossSectionByComoponets(cssName)
					{
						Name = cssName
					};
					cssComponent.Components.ForEach(cp =>
					{
						ideaCssComponent.Components.Add(new CrossSectionComponent()
						{
							Geometry = cp.Geometry,
							Material = Get<IIdeaMaterial>(materialNo)
						});
					});
					return ideaCssComponent;
				}
				else if (iomCss != null)
				{
					PlugInLogger.LogError($"Unexpected type of  CrossSection {iomCss.ToString()}");
				}
			}

			return null;

		}

		private IdeaRS.OpenModel.CrossSection.CrossSection ProcessLibraryCss(LibraryProfileItem profileItem)
		{
			switch (profileItem.ProfileItemType)
			{
				case ProfileItem.ProfileItemTypeEnum.PROFILE_I:
					{
						return CssFactoryHelper.ConvertIProfile(profileItem);
					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_C:
				case ProfileItem.ProfileItemTypeEnum.PROFILE_U:
					{
						return CssFactoryHelper.ConvertUProfile(profileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_P:
					{
						return CssFactoryHelper.ConvertPProfile(profileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_L:
					{
						return CssFactoryHelper.ConvertLProfile(profileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_EC:
				case ProfileItem.ProfileItemTypeEnum.PROFILE_CC: // cold formed CFC
					{
						return CssFactoryHelper.ConvertCCProfile(profileItem);
					}

				case ProfileItem.ProfileItemTypeEnum.PROFILE_PD:
					{
						return CssFactoryHelper.ConvertPDProfile(profileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_PL:
					{
						return CssFactoryHelper.ConvertPLProfile(profileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_D:
					{
						return CssFactoryHelper.ConvertOProfile(profileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_T: // cold formed T
					{
						return CssFactoryHelper.ConvertTProfile(profileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN:
					{
						//component
						return null;
					}

				case ProfileItem.ProfileItemTypeEnum.PROFILE_ZZ: // cold formed T
					{
						return CssFactoryHelper.ConvertZZProfile(profileItem);
					}

				case ProfileItem.ProfileItemTypeEnum.PROFILE_USER_PARAMETRIC: // cold formed CFZ
					{
						switch (profileItem.ProfileItemSubType.ToString())
						{
							case "999263": //C+
								{
									return CssFactoryHelper.ConvertCCPProfile(profileItem);
								}
						}
						return null;
					}

				default:
					{
						return null;
					}

			}
		}

		private static IdeaRS.OpenModel.CrossSection.CrossSection ProcessParametricCss(ParametricProfileItem paramProfileItem)
		{

			ProfileItem.ProfileItemTypeEnum paramProfItemType = paramProfileItem.ProfileItemType;
			ProfileItem.ProfileItemSubTypeEnum paramProfItemSubType = paramProfileItem.ProfileItemSubType;

			switch (paramProfItemType)
			{
				case ProfileItem.ProfileItemTypeEnum.PROFILE_PD:
					{
						return CssFactoryHelper.ConvertParamPDProfile(paramProfileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_P:
					{
						return CssFactoryHelper.ConvertParamPProfile(paramProfileItem, paramProfItemSubType);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_D:
					{
						return CssFactoryHelper.ConvertParamOProfile(paramProfileItem, paramProfItemSubType);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_CC: // cold formed CFC a take CFGeneral
					{
						return CssFactoryHelper.ConvertParamCCProfile(paramProfileItem, paramProfItemSubType);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_PL:
					{
						return CssFactoryHelper.ConvertParamPLProfile(paramProfileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_L: // clasic "L"
					{
						return CssFactoryHelper.ConvertParamLProfile(paramProfileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_I:
					{
						return CssFactoryHelper.ConvertParamIProfile(paramProfileItem, paramProfItemSubType);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_HK:
					{
						return CssFactoryHelper.ConvertParamHKProfile(paramProfileItem, paramProfItemSubType);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_ZZ: // cold formed CFZed a take CFGeneral
					{
						return CssFactoryHelper.ConvertParamZZProfile(paramProfileItem, paramProfItemSubType);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_CW: // cold formed CFGeneral
					{
						return CssFactoryHelper.ConvertParamCWProfile(paramProfileItem, paramProfItemSubType);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_CU: // cold formed CFGeneral
					{
						return CssFactoryHelper.ConvertParamCUProfile(paramProfileItem, paramProfItemSubType);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_EB: // cold formed CFGeneral
					{
						return CssFactoryHelper.ConvertParamEBProfile(paramProfileItem, paramProfItemSubType);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_EC: // cold formed CFGeneral
					{
						return CssFactoryHelper.ConvertParamECProfile(paramProfileItem, paramProfItemSubType);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_ED: // cold formed CFGeneral
					{
						return CssFactoryHelper.ConvertParamEDOrEEOrEFProfile(paramProfileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_EW: // cold formed CFGeneral
					{
						return CssFactoryHelper.ConvertParamEWProfile(paramProfileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_EE: // cold formed CFGeneral
					{
						return CssFactoryHelper.ConvertParamEDOrEEOrEFProfile(paramProfileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_EF: // cold formed CFGeneral
					{
						return CssFactoryHelper.ConvertParamEDOrEEOrEFProfile(paramProfileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_EZ: // cold formed CFGeneral
					{
						return CssFactoryHelper.ConvertParamEZProfile(paramProfileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_Z: // cold formed CFZ
					{
						return CssFactoryHelper.ConvertParamZProfile(paramProfileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_T: // cold formed T
					{
						return CssFactoryHelper.ConvertParamTarcProfile(paramProfileItem);

					}
				case ProfileItem.ProfileItemTypeEnum.PROFILE_USER_PARAMETRIC: // cold formed CFZ
					{
						if (paramProfileItem.ProfilePrefix == "B_WLD_A")
						{
							return CssFactoryHelper.ConvertParamIwnProfile(paramProfileItem);
						}
						else if (paramProfileItem.ProfilePrefix == "B_WLD_D")
						{
							return CssFactoryHelper.ConvertParamUChannelProfile(paramProfileItem);
						}
						else if (paramProfileItem.ProfilePrefix == "B_WLD_E")
						{
							return CssFactoryHelper.ConvertParamTarcProfile(paramProfileItem);
						}
						else if (paramProfileItem.ProfilePrefix == "TC")
						{
							return CssFactoryHelper.ConvertParamTCProfile(paramProfileItem);
						}
						else if (paramProfileItem.ProfilePrefix == "II")
						{
							return CssFactoryHelper.ConvertParamIIProfile(paramProfileItem);
						}
						else
						{
							return null;
						}

					}
				default:
					{
						return null;
					}
			}
		}
	}
}
