using APIData;
using CsToYjk;
using IdeaStatiCa.Plugin;
using yjk.ViewModels;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Loading;
using IdeaRS.OpenModel.Result;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using yjk.BimApis;
using yjk.Helpers;

namespace yjk.FeaApis
{
	public interface IFeaResultsApi
	{
		IEnumerable<IFeaMemberResult> GetMemberInternalForces(int memberId, int loadCaseId);
		void SetResult(int iFlr, FeaMember member, IFeaLoadsApi loadsApi, MemberType memberType, IFeaCrossSectionApi crossSectionApi);
		void ClearResults();
	}

	internal class FeaResultsApi : IFeaResultsApi
	{
		private Dictionary<int, List<IFeaMemberResult>> _resultsForMembers = new Dictionary<int, List<IFeaMemberResult>>();
		private IPluginLogger _logger = AppLogger.Instance;

		public FeaResultsApi()
		{
			//LoadResultsFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultsData.json"));
		}

		public IEnumerable<IFeaMemberResult> GetMemberInternalForces(int memberId, int loadCaseId)
		{
			_logger.LogInformation($"FeaResultsApi.GetMemberInternalForces: memberId={memberId}, loadCaseId={loadCaseId}");
			return _resultsForMembers[memberId].Where(x => x.LoadCaseId == loadCaseId);
		}

		private void LoadResultsFromFile(string filePath)
		{
			string jsonString = File.ReadAllText(filePath);
			List<FeaMemberResult> allResults = JsonConvert.DeserializeObject<List<FeaMemberResult>>(jsonString);
			_resultsForMembers = allResults.GroupBy(result => result.MemberId)
						.ToDictionary(group => group.Key, group => group.Cast<IFeaMemberResult>().ToList());
		}

		public void ClearResults()
		{
			_logger.LogInformation("FeaResultsApi.ClearResults");
			_resultsForMembers.Clear();
		}

		public void SetResult(int iFlr, FeaMember member, IFeaLoadsApi loadsApi, MemberType memberType, IFeaCrossSectionApi crossSectionApi)
		{
			_logger.LogInformation($"FeaResultsApi.SetResult: memberId={member.Id}, floor={iFlr}, type={memberType}");
			List<IFeaMemberResult> feaMemberResults = new List<IFeaMemberResult>();

			foreach (int loadCaseId in loadsApi.GetLoadCasesIds())
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
				if (nSect == 0)
					_logger.LogWarning($"No results returned for member {member.Id}, load case {loadCaseId}, floor {iFlr}");

				feaMemberResults.AddRange(GetFeaMemberResult(member, nSect, force, loadCaseId, memberType, crossSectionApi));
			}

			_resultsForMembers.Add(member.Id, feaMemberResults);
		}

		private List<IFeaMemberResult> GetFeaMemberResult(FeaMember member, int nSect, float[,] force, int loadCaseId,
			MemberType memberType, IFeaCrossSectionApi crossSectionApi)
		{
			_logger.LogInformation($"FeaResultsApi.GetFeaMemberResult: memberId={member.Id}, nSect={nSect}, loadCaseId={loadCaseId}, type={memberType}");
			List<IFeaMemberResult> feaMemberResults = new List<IFeaMemberResult>();

			//Transform if L angle
			IFeaCrossSection crossSection = crossSectionApi.GetCrossSection(member.CrossSectionId);

			if (crossSection.CrossSectionBy == CrossSectionBy.ByParameters &&
				crossSection.CrossSectionParameterYjk.CrossSectionType == CrossSectionType.RolledAngle)
			{
				var css = crossSection.CrossSectionParameterYjk;
				double B = LAnglePrincipalAxesConverter.GetParameter(css, "B");
				double D = LAnglePrincipalAxesConverter.GetParameter(css, "D");
				double t = LAnglePrincipalAxesConverter.GetParameter(css, "t");

				double alpha = LAnglePrincipalAxesConverter.ComputePrincipalAngle(B, D, t);
				_logger.LogInformation($"L-angle principal axis angle: B={B}, D={D}, t={t}, alpha={alpha:F4} rad ({alpha * 180 / Math.PI:F2} deg)");

				for (int i = 0; i < nSect; i++)
				{
					double Mu = force[i, 0];
					double Mv = force[i, 1];
					double Vu = force[i, 2];
					double Vv = force[i, 3];

					if (memberType == MemberType.Column) Mu *= -1;

					var (My, Mz, Vy, Vz) = LAnglePrincipalAxesConverter.ToLocalAxes(Mu, Mv, Vu, Vv, alpha);

					feaMemberResults.Add(BuildResult(member, i, nSect, loadCaseId, memberType,
						force[i, 4], (float)Vy, (float)Vz, force[i, 5], (float)My, (float)Mz));
				}
			}
			else
			{
				for (int i = 0; i < nSect; i++)
				{
					var feaMemberResult = BuildResult(member, i, nSect, loadCaseId, memberType,
						force[i, 4], force[i, 2], force[i, 3], force[i, 5], force[i, 0], force[i, 1]);

					if (memberType == MemberType.Column) feaMemberResult.My *= -1;

					feaMemberResults.Add(feaMemberResult);
				}
			}

			return feaMemberResults;
		}

		private static FeaMemberResult BuildResult(FeaMember member, int i, int nSect,
			int loadCaseId, MemberType memberType,
			float n, float vy, float vz, float mx, float my, float mz)
		{
			var r = new FeaMemberResult
			{
				MemberId = member.Id,
				ResultType = "Load case",
				LoadCaseId = loadCaseId,
				Location = (double)i / ((double)nSect - 1) * member.GetLength(),
				Index = i + 1,
				N = n,
				Vy = vy,
				Vz = vz,
				Mx = mx,
				My = my,
				Mz = mz,
			};

			r.Mz *= -1;
			r.Vy *= -1;

			return r;
		}
	}
}
