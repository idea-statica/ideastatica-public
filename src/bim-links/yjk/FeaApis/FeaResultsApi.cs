using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using APIData;
using CsToYjk;
using IdeaRS.OpenModel.Loading;
using Newtonsoft.Json;

namespace yjk.FeaApis
{
	public interface IFeaResultsApi
	{
		IEnumerable<IFeaMemberResult> GetMemberInternalForces(int memberId, int loadCaseId);
		void SetResultForColumn(int iFlr, int memberDesignId, IFeaLoadsApi loads);
		void SetResultForBeam(int iFlr, int memberDesignId, IFeaLoadsApi loads);
		void SetResultForBrace(int iFlr, int memberDesignId, IFeaLoadsApi loads);
		void ClearResults();
	}

	internal class FeaResultsApi : IFeaResultsApi
	{
		private Dictionary<int, List<IFeaMemberResult>> _resultsForMembers = new Dictionary<int, List<IFeaMemberResult>>();

		public FeaResultsApi() 
		{
			//LoadResultsFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultsData.json"));
		}

		public IEnumerable<IFeaMemberResult> GetMemberInternalForces(int memberId, int loadCaseId)
		{
			return _resultsForMembers[memberId].Where(x => x.LoadCaseId == loadCaseId);			
		}

		private void LoadResultsFromFile(string filePath)
		{
			string jsonString = File.ReadAllText(filePath);
			List<FeaMemberResult> allResults = JsonConvert.DeserializeObject<List<FeaMemberResult>>(jsonString);
			_resultsForMembers = allResults.GroupBy(result => result.MemberId)
						.ToDictionary(group => group.Key, group => group.Cast<IFeaMemberResult>().ToList());
		}

		public void ClearResults() { 
			_resultsForMembers.Clear();
		}

		public void SetResultForColumn(int iFlr, int memberDesignId, IFeaLoadsApi loads)
		{
			List<IFeaMemberResult> feaMemberResults = new List<IFeaMemberResult>();

			foreach (int loadCaseId in loads.GetLoadCasesIds())
			{
				int nSect = 0;
				float[,] force = new float[2, 6];
				_Hi_DesignData.dsnGetColumnStdForce(iFlr, memberDesignId, loadCaseId, 1, ref nSect, ref force);

				FeaMemberResult feaMemberResult = new FeaMemberResult()
				{
					MemberId = memberDesignId,
					ResultType = "Load case",
					LoadCaseId = loadCaseId,
					Location = 0,
					Index = 1,
					N = -1.2994820556640625,
					Vy = 0,
					Vz = -0.22673919677734375,
					Mx = 0,
					My = 0.21835110473632813,
					Mz = 0
				};

				feaMemberResults.Add(feaMemberResult);

				FeaMemberResult feaMemberResult2 = new FeaMemberResult()
				{
					MemberId = memberDesignId,
					ResultType = "Load case",
					LoadCaseId = loadCaseId,
					Location = 1,
					Index = 2,
					N = -1.2994820556640625,
					Vy = 0,
					Vz = -0.22673919677734375,
					Mx = 0,
					My = 0.21835110473632813,
					Mz = 0
				};

				feaMemberResults.Add(feaMemberResult2);
			}

			_resultsForMembers.Add(memberDesignId, feaMemberResults);
		}

		public void SetResultForBeam(int iFlr, int memberDesignId, IFeaLoadsApi loads)
		{
			List<IFeaMemberResult> feaMemberResults = new List<IFeaMemberResult>();

			foreach (int loadCaseId in loads.GetLoadCasesIds())
			{
				int nSect = 0;
				float[,] force = new float[6, 6];
				_Hi_DesignData.dsnGetBeamStdForce(iFlr, memberDesignId, 1, 1, ref nSect, ref force);

				FeaMemberResult feaMemberResult = new FeaMemberResult()
				{
					MemberId = memberDesignId,
					ResultType = "Load case",
					LoadCaseId = loadCaseId,
					Location = 0,
					Index = 1,
					N = -1.2994820556640625,
					Vy = 0,
					Vz = -0.22673919677734375,
					Mx = 0,
					My = 0.21835110473632813,
					Mz = 0
				};

				feaMemberResults.Add(feaMemberResult);


				FeaMemberResult feaMemberResult2 = new FeaMemberResult()
				{
					MemberId = memberDesignId,
					ResultType = "Load case",
					LoadCaseId = loadCaseId,
					Location = 1,
					Index = 2,
					N = -1.2994820556640625,
					Vy = 0,
					Vz = -0.22673919677734375,
					Mx = 0,
					My = 0.21835110473632813,
					Mz = 0
				};

				feaMemberResults.Add(feaMemberResult2);

			}

			_resultsForMembers.Add(memberDesignId, feaMemberResults);
		}

		public void SetResultForBrace(int iFlr, int memberDesignId, IFeaLoadsApi loads)
		{
			List<IFeaMemberResult> feaMemberResults = new List<IFeaMemberResult>();

			foreach (int loadCaseId in loads.GetLoadCasesIds())
			{
				int nSect = 0;
				float[,] force = new float[6, 6];
				_Hi_DesignData.dsnGetBraceStdForce(iFlr, memberDesignId, 1, 1, ref nSect, ref force);

				FeaMemberResult feaMemberResult = new FeaMemberResult()
				{
					MemberId = memberDesignId,
					ResultType = "Load case",
					LoadCaseId = loadCaseId,
					Location = 0,
					Index = 1,
					N = -1.2994820556640625,
					Vy = 0,
					Vz = -0.22673919677734375,
					Mx = 0,
					My = 0.21835110473632813,
					Mz = 0
				};

				feaMemberResults.Add(feaMemberResult);

				FeaMemberResult feaMemberResult2 = new FeaMemberResult()
				{
					MemberId = memberDesignId,
					ResultType = "Load case",
					LoadCaseId = loadCaseId,
					Location = 1,
					Index = 2,
					N = -1.2994820556640625,
					Vy = 0,
					Vz = -0.22673919677734375,
					Mx = 0,
					My = 0.21835110473632813,
					Mz = 0
				};

				feaMemberResults.Add(feaMemberResult2);
			}

			_resultsForMembers.Add(memberDesignId, feaMemberResults);
		}
	}		
}
