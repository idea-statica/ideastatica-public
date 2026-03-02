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
using Newtonsoft.Json;

namespace yjk.FeaApis
{
	public interface IFeaResultsApi
	{
		IEnumerable<IFeaMemberResult> GetMemberInternalForces(int memberId, int loadCaseId);
		void SetResultForColumn(int iFlr, int memberDesignId);
		void SetResultForBeam(int iFlr, int memberDesignId);
		void SetResultForBrace(int iFlr, int memberDesignId);
	}

	internal class FeaResultsApi : IFeaResultsApi
	{
		private Dictionary<int, List<IFeaMemberResult>> _resultsForMembers = new Dictionary<int, List<IFeaMemberResult>>();

		public FeaResultsApi() 
		{
			LoadResultsFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultsData.json"));
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

		public void SetResultForColumn(int iFlr, int memberDesignId)
		{
			int nSect = 0;
			float[,] force = new float[2,6];
			_Hi_DesignData.dsnGetColumnStdForce(iFlr, memberDesignId, 1, 1, ref nSect, ref force);

			//_resultsForMembers.Add(memberDesignId, new FeaMemberResult());
		}

		public void SetResultForBeam(int iFlr, int memberDesignId)
		{
			int nSect = 0;
			float[,] force = new float[6, 6];
			_Hi_DesignData.dsnGetBeamStdForce(iFlr, memberDesignId, 1, 1, ref nSect, ref force);

			//_resultsForMembers.Add(memberDesignId, new FeaMemberResult());
		}

		public void SetResultForBrace(int iFlr, int memberDesignId)
		{
			int nSect = 0;
			float[,] force = new float[6, 6];
			_Hi_DesignData.dsnGetBraceStdForce(iFlr, memberDesignId, 1, 1, ref nSect, ref force);

			//_resultsForMembers.Add(memberDesignId, new FeaMemberResult());
		}


	}		
}
