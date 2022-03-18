using Dlubal.RSTAB8;
using IdeaRstabPlugin.Factories;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;

namespace IdeaRstabPlugin.BimApi
{
	internal class RstabCombiInput : IIdeaCombiInput
	{
		private IModel model;

		private Dlubal.RSTAB8.ResultCombination rfCombiData;
		private List<IIdeaCombiItem> _CombiItems = new List<IIdeaCombiItem>();
		private TypeCalculationCombiEC _TypeCalculationCombi = TypeCalculationCombiEC.Envelope;
		private TypeOfCombiEC _TypeCombiEC = TypeOfCombiEC.ULS;
		private int _id;
		private string _prefix = string.Empty;
		private readonly ObjectFactory _objectFactory;

		public TypeOfCombiEC TypeCombiEC => _TypeCombiEC;

		public TypeCalculationCombiEC TypeCalculationCombi => _TypeCalculationCombi;

		public List<IIdeaCombiItem> CombiItems => _CombiItems;

		public string Id => "combiinput" + _prefix + "-" + _id.ToString();

		public string Name => "CO" + _id.ToString();

		public RstabCombiInput(IModel model, IObjectFactory objectFactory, Dlubal.RSTAB8.IResultCombination _rfCombi)
		{
			//int shiftNonlinId = 1 << 30;
			this.model = model;
			_objectFactory = objectFactory as ObjectFactory;
			rfCombiData = _rfCombi.GetData();
			_id = rfCombiData.Loading.No;
			IIdeaLoadGroup defPermdefault = objectFactory.GetLoadGroup(1);
			IIdeaLoadGroup defPermLG = objectFactory.GetLoadGroup(2);
			IIdeaLoadGroup defVarLG = objectFactory.GetLoadGroup(3);
			IIdeaLoadGroup defCyclicLG = objectFactory.GetLoadGroup(4);
			bool addNoncType = false;
			TypeOfCombiEC combiTypeCombiEC = GetCombiType(rfCombiData.DesignSituation, ref _TypeCalculationCombi, ref addNoncType);

			IDictionary<int, Tuple<bool, IList<RstabLoadCaseBase>>> groupLCs = new Dictionary<int, Tuple<bool, IList<RstabLoadCaseBase>>>();
			IList<RstabLoadCaseBase> ungroupLCs = new List<RstabLoadCaseBase>();

			CombinationLoading[] rfCombiItems = _rfCombi.GetLoadings();
			int numCombiItems = rfCombiItems.Length;
			for (int j = 0; j < numCombiItems; j++)
			{
				if (rfCombiItems[j].Loading.Type != Dlubal.RSTAB8.LoadingType.LoadCaseType && (rfCombiItems[j].Loading.Type != LoadingType.LoadCombinationType))
				{
					continue;
				}
				IdeaStatiCa.BimImporter.ImportedObjects.CombiItemBIM combiItem = new IdeaStatiCa.BimImporter.ImportedObjects.CombiItemBIM();
				int id = rfCombiItems[j].Loading.No;
				RstabLoadCaseBase loadCase = null;
				if (rfCombiItems[j].Loading.Type == LoadingType.LoadCombinationType)
				{
					//id += shiftNonlinId;
					loadCase = (RstabLoadCaseBase)_objectFactory.GetLoadCaseNonLin(id);
				}
				else
				{
					loadCase = (RstabLoadCaseBase)_objectFactory.GetLoadCase(id);
				}

				combiItem.LoadCase = loadCase;
				combiItem.Coeff = rfCombiItems[j].Factor;

				bool cyclic = loadCase.LoadGroup.GroupType == LoadGroupType.Fatigue;

				_CombiItems.Add(combiItem);

				if (rfCombiItems[j].Group > 0)
				{
					Tuple<bool, IList<RstabLoadCaseBase>> lcs = null;
					if (!groupLCs.TryGetValue(rfCombiItems[j].Group, out lcs))
					{
						lcs = new Tuple<bool, IList<RstabLoadCaseBase>>(cyclic, new List<RstabLoadCaseBase>());
						groupLCs.Add(rfCombiItems[j].Group, lcs);
					}
					lcs.Item2.Add(loadCase);
				}
				else
				{
					ungroupLCs.Add(loadCase);

					if (!_objectFactory.notExclusiveIds.Contains(loadCase))
					{
						_objectFactory.notExclusiveIds.Add(loadCase);
					}
				}

				if (rfCombiItems[j].Criterion == CombinationCriterionType.PermanentLoadingType)
				{
					if (_objectFactory.variableIds.Contains(loadCase))
					{
						//nonconf
						if (!_objectFactory.differentRelationLC.Contains(loadCase))
						{
							_objectFactory.differentRelationLC.Add(loadCase);
						}
					}
					else if (_objectFactory.cyclicIds.Contains(loadCase))
					{
						//nonconf
						if (!_objectFactory.differentRelationLC.Contains(loadCase))
						{
							_objectFactory.differentRelationLC.Add(loadCase);
						}
					}
					else if (!_objectFactory.permanentIds.Contains(loadCase))
					{
						loadCase.LoadGroup = defPermLG;
						_objectFactory.permanentIds.Add(loadCase);
					}
				}
				else if ((rfCombiItems[j].Criterion == CombinationCriterionType.VariableLoadingType) && (!cyclic))
				{
					if (_objectFactory.permanentIds.Contains(loadCase))
					{
						//nonconf
						if (!_objectFactory.differentRelationLC.Contains(loadCase))
						{
							_objectFactory.differentRelationLC.Add(loadCase);
						}
					}
					else if (_objectFactory.cyclicIds.Contains(loadCase))
					{
						//muze byt cyclic
						////nonconf
						//if (!differentRelationLC.Contains(loadCase))
						//{
						//	differentRelationLC.Add(loadCase);
						//}
					}
					else if (!_objectFactory.variableIds.Contains(loadCase))
					{
						loadCase.LoadGroup = defVarLG;
						_objectFactory.variableIds.Add(loadCase);
					}
				}
				else if ((rfCombiItems[j].Criterion == CombinationCriterionType.VariableLoadingType) && (cyclic))
				{
					if (_objectFactory.permanentIds.Contains(loadCase))
					{
						//nonconf
						if (!_objectFactory.differentRelationLC.Contains(loadCase))
						{
							_objectFactory.differentRelationLC.Add(loadCase);
						}
					}
					else if (_objectFactory.variableIds.Contains(loadCase))
					{
						//prehodime do cyclic
						_objectFactory.variableIds.Remove(loadCase);
						loadCase.LoadGroup = defCyclicLG;
						_objectFactory.cyclicIds.Add(loadCase);
						////nonconf
						//if (!differentRelationLC.Contains(loadCase))
						//{
						//	differentRelationLC.Add(loadCase);
						//}
					}
					else if (!_objectFactory.cyclicIds.Contains(loadCase))
					{
						loadCase.LoadGroup = defCyclicLG;
						_objectFactory.cyclicIds.Add(loadCase);
					}
				}
				else
				{
					if (loadCase.LoadGroup == defPermLG)
					{
						if (_objectFactory.variableIds.Contains(loadCase))
						{
							//nonconf
							if (!_objectFactory.differentRelationLC.Contains(loadCase))
							{
								_objectFactory.differentRelationLC.Add(loadCase);
							}
						}
						else if (_objectFactory.cyclicIds.Contains(loadCase))
						{
							//nonconf
							if (!_objectFactory.differentRelationLC.Contains(loadCase))
							{
								_objectFactory.differentRelationLC.Add(loadCase);
							}
						}
						else if (!_objectFactory.permanentIds.Contains(loadCase))
						{
							loadCase.LoadGroup = defPermLG;
							_objectFactory.permanentIds.Add(loadCase);
						}
					}
					else if (loadCase.LoadGroup == defVarLG)
					{
						if (_objectFactory.permanentIds.Contains(loadCase))
						{
							//nonconf
							if (!_objectFactory.differentRelationLC.Contains(loadCase))
							{
								_objectFactory.differentRelationLC.Add(loadCase);
							}
						}
						else if (_objectFactory.cyclicIds.Contains(loadCase))
						{
							loadCase.LoadGroup = defCyclicLG;
							//muze byt cyclic
							////nonconf
							//if (!differentRelationLC.Contains(loadCase))
							//{
							//	differentRelationLC.Add(loadCase);
							//}
						}
						else if (!_objectFactory.variableIds.Contains(loadCase))
						{
							_objectFactory.variableIds.Add(loadCase);
						}
					}
					else if (loadCase.LoadGroup == defCyclicLG)
					{
						if (_objectFactory.permanentIds.Contains(loadCase))
						{
							//nonconf
							if (!_objectFactory.differentRelationLC.Contains(loadCase))
							{
								_objectFactory.differentRelationLC.Add(loadCase);
							}
						}
						else if (_objectFactory.variableIds.Contains(loadCase))
						{
							//prehodime do cyclic
							_objectFactory.variableIds.Remove(loadCase);
							loadCase.LoadGroup = defCyclicLG;
							_objectFactory.cyclicIds.Add(loadCase);
							////nonconf
							//if (!differentRelationLC.Contains(loadCase))
							//{
							//	differentRelationLC.Add(loadCase);
							//}
						}
						else if (!_objectFactory.cyclicIds.Contains(loadCase))
						{
							_objectFactory.cyclicIds.Add(loadCase);
						}
					}
				}
			}

			foreach (KeyValuePair<int, Tuple<bool, IList<RstabLoadCaseBase>>> it in groupLCs)
			{
				RstabLoadGroup lg = null;
				foreach (RstabLoadCaseBase lc in it.Value.Item2)
				{
					if (!IsIdLoadGroup(lc.LoadGroup.Id)) //1 - 4 jsou default grupy
					{
						lg = lc.LoadGroup as RstabLoadGroup;
						break;
					}
				}

				if (lg == null)
				{
					lg = _objectFactory.GetLoadGroup(_objectFactory.LoadGroup.Count() + 1) as RstabLoadGroup;
					//lg.Name = "";
					lg.Relation = Relation.Exclusive;
					lg.GroupType = it.Value.Item1 ? LoadGroupType.Fatigue : LoadGroupType.Variable;
				}

				foreach (RstabLoadCaseBase lc in it.Value.Item2)
				{
					if (IsIdLoadGroup(lc.LoadGroup.Id))  //1 - 4 jsou default grupy
					{
						lc.LoadGroup = lg;
					}
					else if (lc.LoadGroup != lg)
					{
						//nonconf
						if (!_objectFactory.differentRelationLC.Contains(lc))
						{
							_objectFactory.differentRelationLC.Add(lc);
						}
					}
				}
			}
		}

		private TypeOfCombiEC GetCombiType(DesignSituationType designSituation, ref TypeCalculationCombiEC typeOfCombination, ref bool setToULS)
		{
			TypeOfCombiEC combiTypeCombiEC = TypeOfCombiEC.ULS;
			typeOfCombination = 0;
			setToULS = false;

			switch (designSituation)
			{
				case DesignSituationType.UnknownDesignSituation:
				case DesignSituationType.NoneDesignSituation:
				case DesignSituationType.OtherDesignSituation:
					combiTypeCombiEC = TypeOfCombiEC.ULS;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					setToULS = true;
					break;

				case DesignSituationType.ManualUls:
					combiTypeCombiEC = TypeOfCombiEC.ULS;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.ManualUlsAccidental:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.ManualSeismic:
					combiTypeCombiEC = TypeOfCombiEC.Seismic;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;
				//case DesignSituationType.ManualFire:
				case DesignSituationType.ManualSls:
					combiTypeCombiEC = TypeOfCombiEC.SLS_Char;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsEquPermanentTransient:
					combiTypeCombiEC = TypeOfCombiEC.ULS_SET_C;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsEquAccidental:
					combiTypeCombiEC = TypeOfCombiEC.Accidental_2;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsEquAccidentalPsi11:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsEquAccidentalPsi21:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsEquAccidentalSnowPsi11:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsEquAccidentalSnowPsi21:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsEquSeismic:
					combiTypeCombiEC = TypeOfCombiEC.Seismic;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;
				//case DesignSituationType.UlsStaticEquilibriumFundamental:
				//case DesignSituationType.UlsStaticEquilibriumAccidental:
				case DesignSituationType.UlsType1Fundamental:
					combiTypeCombiEC = TypeOfCombiEC.ULS;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsType1AccidentalPsi11:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsType1AccidentalPsi21:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsStrPermanentTransient610:
					combiTypeCombiEC = TypeOfCombiEC.ULS;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsStrPermanentTransient610AAndB:
					combiTypeCombiEC = TypeOfCombiEC.ULS;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsStrPermanentTransient610AModB:
					combiTypeCombiEC = TypeOfCombiEC.ULS;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsStrAccidental:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsStrAccidentalPsi11:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsStrAccidentalPsi21:
					combiTypeCombiEC = TypeOfCombiEC.Accidental_2;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsStrAccidentalShowPsi11:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsStrAccidentalShowPsi21:
					combiTypeCombiEC = TypeOfCombiEC.Accidental_2;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsStrSeismic:
					combiTypeCombiEC = TypeOfCombiEC.Seismic;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;
				//case DesignSituationType.UlsFailureStructureFundamental:
				//case DesignSituationType.UlsFailureStructureAccidental:
				case DesignSituationType.UlsType2Fundamental:
					combiTypeCombiEC = TypeOfCombiEC.ULS;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsType2AccidentalPsi11:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsType2AccidentalPsi21:
					combiTypeCombiEC = TypeOfCombiEC.Accidental_2;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsType3Fundamental:
					combiTypeCombiEC = TypeOfCombiEC.ULS;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsType3AccidentalPsi11:
					combiTypeCombiEC = TypeOfCombiEC.Accidental;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsType3AccidentalPsi21:
					combiTypeCombiEC = TypeOfCombiEC.Accidental_2;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.SlsCharacteristic:
					combiTypeCombiEC = TypeOfCombiEC.SLS_Char;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.SlsFrequent:
					combiTypeCombiEC = TypeOfCombiEC.SLS_Freq;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.SlsQuasiPermanent:
					combiTypeCombiEC = TypeOfCombiEC.SLS_Quasi;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				case DesignSituationType.UlsFatigue:
					combiTypeCombiEC = TypeOfCombiEC.Fatigue;
					typeOfCombination = TypeCalculationCombiEC.Envelope;
					break;

				default:
					setToULS = true;
					break;
			}

			return combiTypeCombiEC;
		}

		private bool IsIdLoadGroup(string id)
		{
			switch (id)
			{
				case "loadgroup-1":
				case "loadgroup-2":
				case "loadgroup-3":
				case "loadgroup-4":
					return true;
			}
			return false;
		}
	}
}