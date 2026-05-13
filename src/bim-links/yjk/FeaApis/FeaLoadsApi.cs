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
			_loadGroups = new List<IFeaLoadGroup>();

			//Get load cases
			int nLDCaseNum = 0;
			int[] LDCase = new int[0];
			int[] LDCaseOld = new int[0];
			int[] LDKind = new int[0];

			_logger.LogInformation("Read load case from YJK");
			_Hi_DesignData.dsnGetLDCaseBySortNew(ref nLDCaseNum, ref LDCase, ref LDCaseOld, ref LDKind, true, true, true, true, true, true, true, true);

			//Load cases
			for (int i = 0; i < nLDCaseNum; i++)
			{
				//Get load case name
				string LDCaseName = "";
				_Hi_DesignData.dsnGetLDCaseName_EN(LDCase[i], ref LDCaseName);

				TypeOfLoadCase loadCaseType;
				switch (LDKind[i])
				{
					case 1:
						loadCaseType = TypeOfLoadCase.Dead;
						break;
					case 2:
						loadCaseType = TypeOfLoadCase.Live;
						break;
					case 3:
						loadCaseType = TypeOfLoadCase.Wind;
						break;
					case 4:
						loadCaseType = TypeOfLoadCase.HorizontalSeismic;
						break;
					case 5:
						loadCaseType = TypeOfLoadCase.VerticalSeismic;
						break;
					case 6:
						loadCaseType = TypeOfLoadCase.CivilDefence;
						break;
					case 7:
						loadCaseType = TypeOfLoadCase.Crane;
						break;
					case 8:
						loadCaseType = TypeOfLoadCase.Temperature;
						break;
					default:
						_logger.LogWarning($"Load case {LDCase[i]} ({LDCaseName}): unrecognised LDKind={LDKind[i]}, defaulting to Dead");
						loadCaseType = TypeOfLoadCase.Dead;
						break;
				}

				LoadGroupCategory groupCategory = IsVariableLoadCase(loadCaseType)
					? LoadGroupCategory.Variable
					: LoadGroupCategory.Permanent;
				int loadGroupId = EnsureLoadGroup(groupCategory);

				_logger.LogInformation($"Add load case {LDCase[i]}, {LDCaseName}, kind={LDKind[i]}, type={loadCaseType}, groupId={loadGroupId}");
				_loadCases.Add(
					new FeaLoadCase()
					{
						Id = LDCase[i],
						Name = LDCaseName,
						LoadCaseType = loadCaseType,
						LoadGroupId = loadGroupId,
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

				_logger.LogInformation($"Add load combination {loadCombId}, {"COMB" + loadCombId.ToString()}");
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

		private static bool IsVariableLoadCase(TypeOfLoadCase type)
		{
			switch (type)
			{
				case TypeOfLoadCase.Live:
				case TypeOfLoadCase.Wind:
				case TypeOfLoadCase.HorizontalSeismic:
				case TypeOfLoadCase.VerticalSeismic:
				case TypeOfLoadCase.Crane:
				case TypeOfLoadCase.Temperature:
					return true;
				default:
					return false;
			}
		}

		private int EnsureLoadGroup(LoadGroupCategory category)
		{
			foreach (IFeaLoadGroup group in _loadGroups)
			{
				if (group.LoadGroupCategory == category)
					return group.Id;
			}

			int newId = _loadGroups.Count + 1;
			string name = category == LoadGroupCategory.Permanent ? "Permanent" : "Variable";
			_logger.LogInformation($"Load group created: id={newId}, name={name}, category={category}");
			_loadGroups.Add(new FeaLoadGroup() { Id = newId, Name = name, LoadGroupCategory = category });
			return newId;
		}
	}
}
