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
		void SetResultForColumn(int iFlr, FeaMember member, IFeaLoadsApi loads);
		void SetResultForBeam(int iFlr, FeaMember member, IFeaLoadsApi loads);
		void SetResultForBrace(int iFlr, FeaMember member, IFeaLoadsApi loads);

		void SetResult(int iFlr, FeaMember member, IFeaLoadsApi loads, MemberType memberType);
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

		public void SetResultForColumn(int iFlr, FeaMember member, IFeaLoadsApi loads)
		{
			List<IFeaMemberResult> feaMemberResults = new List<IFeaMemberResult>();

			foreach (int loadCaseId in loads.GetLoadCasesIds())
			{
				int nSect = 0;
				float[,] force = new float[0, 0];
				_Hi_DesignData.dsnGetColumnStdForce(iFlr, member.Id, loadCaseId, 1, ref nSect, ref force);
				feaMemberResults.AddRange(GetFeaMemberResult(member, nSect, force, loadCaseId));
			}

			_resultsForMembers.Add(member.Id, feaMemberResults);
		}

		public void SetResultForBeam(int iFlr, FeaMember member, IFeaLoadsApi loads)
		{
			List<IFeaMemberResult> feaMemberResults = new List<IFeaMemberResult>();

			foreach (int loadCaseId in loads.GetLoadCasesIds())
			{
				int nSect = 0;
				float[,] force = new float[0, 0];
				_Hi_DesignData.dsnGetBeamStdForce(iFlr, member.Id, loadCaseId, 1, ref nSect, ref force);
				feaMemberResults.AddRange(GetFeaMemberResult(member, nSect, force, loadCaseId));
			}

			_resultsForMembers.Add(member.Id, feaMemberResults);
		}

		public void SetResultForBrace(int iFlr, FeaMember member, IFeaLoadsApi loads)
		{
			List<IFeaMemberResult> feaMemberResults = new List<IFeaMemberResult>();

			foreach (int loadCaseId in loads.GetLoadCasesIds())
			{
				int nSect = 0;
				float[,] force = new float[0, 0];
				_Hi_DesignData.dsnGetBraceStdForce(iFlr, member.Id, loadCaseId, 1, ref nSect, ref force);
				feaMemberResults.AddRange(GetFeaMemberResult(member, nSect, force, loadCaseId));
			}

			_resultsForMembers.Add(member.Id, feaMemberResults);
		}

		public void SetResult(int iFlr, FeaMember member, IFeaLoadsApi loads, MemberType memberType)
		{
			List<IFeaMemberResult> feaMemberResults = new List<IFeaMemberResult>();

			foreach (int loadCaseId in loads.GetLoadCasesIds())
			{
				int nSect = 0;
				float[,] force = new float[0, 0];

				switch (memberType)
				{
					case MemberType.Column:
						_Hi_DesignData.dsnGetColumnStdForce(iFlr, member.Id, loadCaseId, 1, ref nSect, ref force);
						break;
					case MemberType.Beam:
						_Hi_DesignData.dsnGetBeamStdForce(iFlr, member.Id, loadCaseId, 1, ref nSect, ref force);
						break;
					case MemberType.Brace:
						_Hi_DesignData.dsnGetBraceStdForce(iFlr, member.Id, loadCaseId, 1, ref nSect, ref force);
						break;
				}
				feaMemberResults.AddRange(GetFeaMemberResult(member, nSect, force, loadCaseId));
			}

			_resultsForMembers.Add(member.Id, feaMemberResults);
		}

		private List<IFeaMemberResult> GetFeaMemberResult(FeaMember member, int nSect, float[,] force, int loadCaseId)
		{
			List<IFeaMemberResult> feaMemberResults = new List<IFeaMemberResult>();

			for (int i = 0; i < nSect; i++)
			{
				FeaMemberResult feaMemberResult = new FeaMemberResult()
				{
					MemberId = member.Id,
					ResultType = "Load case",
					LoadCaseId = loadCaseId,
					Location = (double)i / ((double)nSect - 1) * member.GetLength(),
					Index = i + 1,
					N = force[i, 4],
					Vy = force[i, 2],
					Vz = force[i, 3],
					Mx = force[i, 5],
					My = force[i, 0],
					Mz = force[i, 1],
				};

				feaMemberResults.Add(feaMemberResult);
			}

			return feaMemberResults;
		}
	}		
}
