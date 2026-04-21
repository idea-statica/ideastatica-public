using APIData;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using yjk.ViewModels;

namespace yjk.FeaApis
{
	public interface IFeaLoadsApi
	{
		IFeaLoadCase GetLoadCase(int id);
		IFeaLoadGroup GetLoadGroup(int id);
		IFeaLoadCombination GetLoadCombination(int id);
		IEnumerable<int> GetLoadCasesIds();
		IEnumerable<int> GetLoadGroupsIds();
		IEnumerable<int> GetLoadCombinationsIds();

		void GetLoadCasesAndCombos();
	}

	internal class FeaLoadsApi : IFeaLoadsApi
	{
		private List<IFeaLoadCase> _loadCases;
		private List<IFeaLoadCombination> _loadCombinations;
		private List<IFeaLoadGroup> _loadGroups;

		private IPluginLogger _logger = AppLogger.Instance;

		public IFeaLoadCase GetLoadCase(int id) => _loadCases.FirstOrDefault(x => x.Id == id);

		public IEnumerable<int> GetLoadCasesIds() => _loadCases.Select(x => x.Id);

		public IFeaLoadCombination GetLoadCombination(int id) => _loadCombinations.FirstOrDefault(x => x.Id == id);		

		public IEnumerable<int> GetLoadCombinationsIds() => _loadCombinations.Select(x => x.Id);

		public IFeaLoadGroup GetLoadGroup(int id) => _loadGroups.FirstOrDefault(x => x.Id == id);		

		public IEnumerable<int> GetLoadGroupsIds() => _loadGroups.Select(x => x.Id);

		public void GetLoadCasesAndCombos()
		{
			_logger.LogInformation("FeaLoadsApi.GetLoadCasesAndCombos");

			_loadCases = new List<IFeaLoadCase>();
			_loadCombinations = new List<IFeaLoadCombination>();
			_loadGroups = InitializeLoadGroups();

			//Get load cases
			int nLDCaseNum = 0;
			int[] LDCase = new int[0];
			int[] LDCaseOld = new int[0];
			int[] LDKind = new int[0];

			_logger.LogInformation("Read load case from YJK");
			_Hi_DesignData.dsnGetLDCaseBySortNew(ref nLDCaseNum, ref LDCase, ref LDCaseOld, ref LDKind, true, true, true, true, true, true, true, true);

			//Load cases
			Dictionary<int, string> LDCaseNameList = new Dictionary<int, string>();
			for (int i = 0; i < nLDCaseNum; i++)
			{
				//Get load case name
				string LDCaseName = "";
				_Hi_DesignData.dsnGetLDCaseName_EN(LDCase[i], ref LDCaseName);

				_logger.LogInformation($"Add load case {LDCase[i]}, {LDCaseName}, {ActionType.Permanent}, {TypeOfLoadCase.Selfweight}, {1}");
				_loadCases.Add(
					new FeaLoadCase()
					{
						Id = LDCase[i],
						Name = LDCaseName,
						ActionType = ActionType.Permanent,
						LoadCaseType = TypeOfLoadCase.Selfweight,
						LoadGroupId = 1,
					}
				);


			}

			//Load combinations
			var _hi_CToSDesign = new Hi_CToSDesign();

			int nFloor = 1; //Take 1 random floor

			int numCol = _hi_CToSDesign.NColumn(nFloor);
			int nComKind = (int)PostGjKind.COM_COLUMN;
			int nTotID = 0;

			if (numCol > 0) 
			{
				nTotID = _hi_CToSDesign.FlrColumns(nFloor, numCol)[0]; //Take 1 random column
			}
			else
			{
				int numBeam = _hi_CToSDesign.NBeam(nFloor);
				nComKind = (int)PostGjKind.COM_BEAM;

				if (numBeam > 0)
				{
					nTotID = _hi_CToSDesign.FlrBeams(nFloor, numBeam)[0]; //Take 1 random beam
				}
				else
				{
					int numBrace = _hi_CToSDesign.NBrace(nFloor);
					nComKind = (int)PostGjKind.COM_BRACE;

					if (numBrace > 0)
					{
						nTotID = _hi_CToSDesign.FlrBraces(nFloor, numBrace)[0]; //Take 1 random brace
					}
				}
			}

			int nsectDSNType = (int)PostSectDsnType.SECTDSNTYPE_M; //Take 1 random load type

			List<Dictionary<int, float>> vecLDCombCoe = new List<Dictionary<int, float>>();
			List<List<int>> vecLDCombSign = new List<List<int>>();
			_Hi_DesignData.dsnGetComLDCombCoe(nFloor, nComKind, nTotID, nsectDSNType, vecLDCombCoe, vecLDCombSign);

			int loadCombId = 1;
			foreach (Dictionary<int, float> eachVecLDCombCoe in vecLDCombCoe)
			{
				List<CombiFactor> combiFactors = new List<CombiFactor>();

				foreach (KeyValuePair<int, float> kvp in eachVecLDCombCoe)
				{
					combiFactors.Add(new CombiFactor(kvp.Key, kvp.Value));

				}

				_logger.LogInformation($"Add load combination {loadCombId}, {"COMB" + loadCombId.ToString()}, {Category.ULS}, {Type.Linear}, {combiFactors}");
				_loadCombinations.Add(
					new FeaLoadCombination()
					{
						Id = loadCombId,
						Name = "COMB" + loadCombId.ToString(),
						Category = Category.ULS,
						Type = Type.Linear,
						CombiFactors = combiFactors
					}
				);

				loadCombId++;
			}
		}

		private static List<IFeaLoadCombination> InitializeLoadCombinations()
		{
			return new List<IFeaLoadCombination>
			{
				new FeaLoadCombination()
				{
					Id = 1,
					Name = "ULS-CO1",
					Category = Category.ULS,
					Type = Type.Linear,
					CombiFactors = new List<CombiFactor>()
					{
						new CombiFactor(1, 1.35),
						new CombiFactor(2, 1.35)
					}
				},
				new FeaLoadCombination()
				{
					Id = 2,
					Name = "ULS-CO2",
					Category = Category.ULS,
					Type = Type.Linear,
					CombiFactors = new List<CombiFactor>()
					{
						new CombiFactor(1, 1.35),
						new CombiFactor(2, 1.35),
						new CombiFactor(3, 1.5)
					}
				},
				new FeaLoadCombination()
				{
					Id = 3,
					Name = "SLS-CO3",
					Category = Category.SLS,
					Type = Type.Linear,
					CombiFactors = new List<CombiFactor>()
					{
						new CombiFactor(1, 1.1),
						new CombiFactor(2, 1.1)
					}
				},
				new FeaLoadCombination()
				{
					Id = 4,
					Name = "SLS-CO4",
					Category = Category.SLS,
					Type = Type.Linear,
					CombiFactors = new List<CombiFactor>()
					{
						new CombiFactor(1, 1.0),
						new CombiFactor(2, 1.0),
						new CombiFactor(3, 0.9)
					}
				},
			};
		}

		private static List<IFeaLoadCase> InitialiazeLoadCases()
		{
			return new List<IFeaLoadCase>()
			{
				new FeaLoadCase()
				{
					Id = 1,
					Name = "Selfweight",
					ActionType = ActionType.Permanent,
					LoadCaseType = TypeOfLoadCase.Selfweight,
					LoadGroupId = 1,
				},
				new FeaLoadCase()
				{
					Id = 2,
					Name = "Dead load",
					ActionType = ActionType.Permanent,
					LoadCaseType = TypeOfLoadCase.DeadLoad,
					LoadGroupId = 1,
				},
				new FeaLoadCase()
				{
					Id = 3,
					Name = "Snow",
					ActionType = ActionType.Variable,
					LoadCaseType = TypeOfLoadCase.Snow,
					LoadGroupId = 2,
				}
			};
		}

		private static List<IFeaLoadGroup> InitializeLoadGroups()
		{
			return new List<IFeaLoadGroup>()
			{
				new FeaLoadGroup()
				{
					Id = 1,
					Name = "LG1",
					LoadGroupCategory = LoadGroupCategory.Permanent
				},
/*				new FeaLoadGroup()
				{
					Id = 2,
					Name = "LG2",
					LoadGroupCategory = LoadGroupCategory.Variable
				}*/
			};
		}
	}
}
