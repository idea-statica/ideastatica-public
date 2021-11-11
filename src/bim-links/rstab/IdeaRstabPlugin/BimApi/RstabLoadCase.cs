using IdeaRstabPlugin.Factories;
using IdeaRstabPlugin.Providers;
using IdeaRS.OpenModel.Loading;

namespace IdeaRstabPlugin.BimApi
{
	internal class RstabLoadCase : RstabLoadCaseBase
	{
		private bool _cyclic = false;

		public RstabLoadCase(IObjectFactory objectFactory, ILoadsProvider loadsProvider, int no)
		{
			Dlubal.RSTAB8.LoadCase data = loadsProvider.GetLoadCase(no);

			SetLoadcaseType(data);
			LoadGroup = objectFactory.GetLoadGroup(GetLoadGroupInx(_cyclic));

			Id = $"loadcase-{data.Loading.No}";

			Name = $"LC {data.Loading.No} {data.Description}";
			Description = data.Description;
		}

		private void SetLoadcaseType(Dlubal.RSTAB8.LoadCase rfloadCaseData)
		{
			switch (rfloadCaseData.ActionCategory)
			{
				case Dlubal.RSTAB8.ActionCategoryType.UnknownActionCategory:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableStatic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.Permanent:
					LoadType = LoadCaseType.Permanent;
					if (rfloadCaseData.SelfWeight == true)
					{
						Type = LoadCaseSubType.PermanentSelfweight;
					}
					else
					{
						Type = LoadCaseSubType.PermanentStandard;
					}

					break;

				case Dlubal.RSTAB8.ActionCategoryType.Prestress:
					//if (!loadCase.IsPermanentPrestressing)
					{
						LoadType = LoadCaseType.Permanent;
						Type = LoadCaseSubType.PermanentPrestress;
					}

					break;

				case Dlubal.RSTAB8.ActionCategoryType.TrafficLoads:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableDynamic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.SnowIce:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableStatic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.Wind:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableDynamic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.TemperatureNonFire:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableStatic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.SettlementsOfFoundationSoil:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableStatic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.OtherCategory:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableStatic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.Accidental:
					LoadType = LoadCaseType.Accidental;
					//_Type = LoadCaseSubType.NotSet;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.Earthquake:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableDynamic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.UserDefined:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableStatic;
					break;
				//case Dlubal.RSTAB8.ActionCategoryType.ImperfectionCategory:
				case Dlubal.RSTAB8.ActionCategoryType.PermanentSmallFluctuations:
				case Dlubal.RSTAB8.ActionCategoryType.WeightOfIce:
				case Dlubal.RSTAB8.ActionCategoryType.DeadLoad:
				case Dlubal.RSTAB8.ActionCategoryType.DeadLoadDL:
				case Dlubal.RSTAB8.ActionCategoryType.DeadLoadGK:
				case Dlubal.RSTAB8.ActionCategoryType.PermanentFromCranes:
				case Dlubal.RSTAB8.ActionCategoryType.PermanentImposed:
					LoadType = LoadCaseType.Permanent;
					Type = LoadCaseSubType.PermanentStandard;
					break;
				//case Dlubal.RSTAB8.ActionCategoryType.SelfStrainingForce:
				//case Dlubal.RSTAB8.ActionCategoryType.Variable:
				//case Dlubal.RSTAB8.ActionCategoryType.Live:
				//case Dlubal.RSTAB8.ActionCategoryType.RoofLive:
				case Dlubal.RSTAB8.ActionCategoryType.Imposed:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedCategoryA:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedCategoryB:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedCategoryC:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedCategoryD:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedCategoryE:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedLive:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedLoad:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedCategoryF:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedCategoryG:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedCategoryH:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedCategoryI:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedCategoryJ:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedLoadsCategoryH:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedLoadsCategoryK:
				case Dlubal.RSTAB8.ActionCategoryType.ImposedLoadsCategoryKOther:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableStatic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.SnowIce2:
				case Dlubal.RSTAB8.ActionCategoryType.SnowFinland:
				case Dlubal.RSTAB8.ActionCategoryType.SnowHGreaterThan1000:
				case Dlubal.RSTAB8.ActionCategoryType.ShowHLowerThan1000:
				case Dlubal.RSTAB8.ActionCategoryType.SnowSKLowerThan275:
				case Dlubal.RSTAB8.ActionCategoryType.SnowSKGreaterThan275:
				case Dlubal.RSTAB8.ActionCategoryType.SnowOutdoorSKLowerThan275:
				case Dlubal.RSTAB8.ActionCategoryType.SnowOutdoorSKGreaterThan275:
				case Dlubal.RSTAB8.ActionCategoryType.Snow:
				case Dlubal.RSTAB8.ActionCategoryType.SnowSweden1:
				case Dlubal.RSTAB8.ActionCategoryType.SnowSweden2:
				case Dlubal.RSTAB8.ActionCategoryType.SnowSweden3:
				case Dlubal.RSTAB8.ActionCategoryType.SnowSaintPierreEtMiquelon:
				case Dlubal.RSTAB8.ActionCategoryType.SnowIceRain:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableStatic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.WindLoad:
				case Dlubal.RSTAB8.ActionCategoryType.WindOnIce:
				case Dlubal.RSTAB8.ActionCategoryType.WindLiveLoad:
				case Dlubal.RSTAB8.ActionCategoryType.WindWL:
				case Dlubal.RSTAB8.ActionCategoryType.WindWK:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableDynamic;
					break;
				//case Dlubal.RSTAB8.ActionCategoryType.UniformTemperatures:
				//case Dlubal.RSTAB8.ActionCategoryType.TemperatureShrinkageCreep:
				//case Dlubal.RSTAB8.ActionCategoryType.PermanentFromSoilEarthLoad:
				//case Dlubal.RSTAB8.ActionCategoryType.PermanentFromSoilEarthPressure:
				//case Dlubal.RSTAB8.ActionCategoryType.PermanentFromSoilWaterPressure:
				//case Dlubal.RSTAB8.ActionCategoryType.VariableFromSoilEarthPressure:
				//case Dlubal.RSTAB8.ActionCategoryType.VariableFromSoilWaterPressure:
				//case Dlubal.RSTAB8.ActionCategoryType.LateralEarthWaterPressure:
				//case Dlubal.RSTAB8.ActionCategoryType.LateralEarthPressure:
				//case Dlubal.RSTAB8.ActionCategoryType.Fluids:
				//case Dlubal.RSTAB8.ActionCategoryType.Flood:
				//case Dlubal.RSTAB8.ActionCategoryType.Rain:
				//case Dlubal.RSTAB8.ActionCategoryType.CraneLoad:
				//case Dlubal.RSTAB8.ActionCategoryType.ErectionLoad:
				//case Dlubal.RSTAB8.ActionCategoryType.NotionalHorizontalForces:
				//case Dlubal.RSTAB8.ActionCategoryType.ActionsDuringExecution:
				case Dlubal.RSTAB8.ActionCategoryType.AccidentalLoad:
					LoadType = LoadCaseType.Accidental;
					//_Type = LoadCaseSubType.NotSet;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.EarthquakeLoadE:
				case Dlubal.RSTAB8.ActionCategoryType.EarthquakeLiveLoad:
				case Dlubal.RSTAB8.ActionCategoryType.EarthquakeLoadEL:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableDynamic;
					break;
				//case Dlubal.RSTAB8.ActionCategoryType.GbPermanent:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings211:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings212:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings22:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings231:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings232:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings241:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings242:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings251:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings252:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings261:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings262:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings27:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings281:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings282:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings291:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings292:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings2101:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings2102:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings2111:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings2112:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings2113:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings2121:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCivilBuildings2122:
				//case Dlubal.RSTAB8.ActionCategoryType.GbIndustrialBuilding31:
				//case Dlubal.RSTAB8.ActionCategoryType.GbIndustrialBuilding321:
				//case Dlubal.RSTAB8.ActionCategoryType.GbIndustrialBuilding322:
				//case Dlubal.RSTAB8.ActionCategoryType.GbIndustrialBuilding323:
				//case Dlubal.RSTAB8.ActionCategoryType.GbIndustrialBuilding33:
				//case Dlubal.RSTAB8.ActionCategoryType.GbIndustrialBuilding34:
				//case Dlubal.RSTAB8.ActionCategoryType.GbIndustrialBuilding35:
				//case Dlubal.RSTAB8.ActionCategoryType.GbIndustrialBuilding36:
				//case Dlubal.RSTAB8.ActionCategoryType.GbLiveLoadsOnRoofs41:
				//case Dlubal.RSTAB8.ActionCategoryType.GbLiveLoadsOnRoofs42:
				//case Dlubal.RSTAB8.ActionCategoryType.GbLiveLoadsOnRoofs43:
				//case Dlubal.RSTAB8.ActionCategoryType.GbLiveLoadsOnRoofs44:
				//case Dlubal.RSTAB8.ActionCategoryType.GbAshLoadOnRoofings51:
				//case Dlubal.RSTAB8.ActionCategoryType.GbAshLoadOnRoofings52:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCraneLoads61:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCraneLoads62:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCraneLoads63:
				//case Dlubal.RSTAB8.ActionCategoryType.GbCraneLoads64:
				//case Dlubal.RSTAB8.ActionCategoryType.GbSnowLoadZone1:
				//case Dlubal.RSTAB8.ActionCategoryType.GbSnowLoadZone2:
				//case Dlubal.RSTAB8.ActionCategoryType.GbSnowLoadZone3:
				//case Dlubal.RSTAB8.ActionCategoryType.GbWindLoadSccordingTo714:
				//case Dlubal.RSTAB8.ActionCategoryType.GbAccidentalActions:
				//case Dlubal.RSTAB8.ActionCategoryType.GbSeismic:
				case Dlubal.RSTAB8.ActionCategoryType.UnevenSettlements:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableStatic;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.Gr1A:
				case Dlubal.RSTAB8.ActionCategoryType.Gr1B:
				case Dlubal.RSTAB8.ActionCategoryType.Gr2:
				case Dlubal.RSTAB8.ActionCategoryType.Gr3:
				case Dlubal.RSTAB8.ActionCategoryType.Gr4:
				case Dlubal.RSTAB8.ActionCategoryType.Gr5:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableDynamic;
					_cyclic = true;
					break;

				case Dlubal.RSTAB8.ActionCategoryType.WindLoadsFwkPersistentDesignSituations:
				case Dlubal.RSTAB8.ActionCategoryType.WindLoadsFwkExecution:
				case Dlubal.RSTAB8.ActionCategoryType.WindLoadsFw:
					LoadType = LoadCaseType.Variable;
					Type = LoadCaseSubType.VariableDynamic;
					break;
				//case Dlubal.RSTAB8.ActionCategoryType.ConstructionLoadsDueToWorkingPersonnel:
				//case Dlubal.RSTAB8.ActionCategoryType.OtherConstructionLoads:
				//case Dlubal.RSTAB8.ActionCategoryType.ImposedFromCranesCategoryA:
				//case Dlubal.RSTAB8.ActionCategoryType.ImposedFromCranesCategoryB:
				//case Dlubal.RSTAB8.ActionCategoryType.ImposedFromCranesCategoryC:
				//case Dlubal.RSTAB8.ActionCategoryType.ImposedFromCranesCategoryD:
				//case Dlubal.RSTAB8.ActionCategoryType.ImposedFromCranesGeneral:
				//case Dlubal.RSTAB8.ActionCategoryType.PipingSelfWeight:
				//case Dlubal.RSTAB8.ActionCategoryType.PipingFluid:
				//case Dlubal.RSTAB8.ActionCategoryType.PipingFluidPressureTest:
				//case Dlubal.RSTAB8.ActionCategoryType.PipingTemperature:
				//case Dlubal.RSTAB8.ActionCategoryType.PipingPressure:
				//case Dlubal.RSTAB8.ActionCategoryType.PipingTestPressure:
				//case Dlubal.RSTAB8.ActionCategoryType.PipingDisplacement:
				//case Dlubal.RSTAB8.ActionCategoryType.PipingHangerCategory:
				//case Dlubal.RSTAB8.ActionCategoryType.PipingColdSpring:
				//case Dlubal.RSTAB8.ActionCategoryType.FormFindingResults:
				//case Dlubal.RSTAB8.ActionCategoryType.Nbr1A:
				//case Dlubal.RSTAB8.ActionCategoryType.Nbr1B:
				//case Dlubal.RSTAB8.ActionCategoryType.Nbr1C:
				//case Dlubal.RSTAB8.ActionCategoryType.Nbr1D:
				//case Dlubal.RSTAB8.ActionCategoryType.Nbr1E:
				//case Dlubal.RSTAB8.ActionCategoryType.Nbr1F:
				//case Dlubal.RSTAB8.ActionCategoryType.NbrSettlements:
				//case Dlubal.RSTAB8.ActionCategoryType.NbrActionsMaxValues:
				//case Dlubal.RSTAB8.ActionCategoryType.NbrGeneralVariableActions:
				//case Dlubal.RSTAB8.ActionCategoryType.NbrExceptional:
				//case Dlubal.RSTAB8.ActionCategoryType.LoadArisingFromExtraordinaryEvent:
				default:
					//Type ^ type = UnknownLcType::typeid;
					//AddNonconformity(type->GUID, loadCase->Name);
					break;
			}
		}
	}
}