using System.Collections.Generic;
using System.Linq;

namespace FeaExampleApi
{
	/// <summary>
	/// Reads the internal forces the example FEA application has calculated. In a real link, this is the API of your
	/// application.
	/// </summary>
	public interface IFeaResultsApi
	{
		/// <summary>
		/// The internal forces along the member for the load case, from the start of the member to its end. Two results
		/// can be at the same position.
		/// </summary>
		IReadOnlyList<FeaMemberResult> GetMemberResults(int memberId, int loadCaseId);
	}

	/// <summary>
	/// Serves fixed internal forces for the two members and three load cases of the example model.
	/// </summary>
	public class FeaResultsApi : IFeaResultsApi
	{
		private readonly List<(int MemberId, int LoadCaseId, FeaMemberResult Result)> _results =
			new List<(int MemberId, int LoadCaseId, FeaMemberResult Result)>();

		public FeaResultsApi()
		{
			CreateResults();
		}

		public IReadOnlyList<FeaMemberResult> GetMemberResults(int memberId, int loadCaseId)
		{
			return _results
				.Where(row => row.MemberId == memberId && row.LoadCaseId == loadCaseId)
				.Select(row => row.Result)
				.OrderBy(result => result.Position)
				.ToList();
		}

		/// <summary>
		/// Fills in the results. The frame lies in the XZ plane and is loaded in that plane, so only N, Vz and My are
		/// not zero.
		/// </summary>
		private void CreateResults()
		{
			// Member, load case, position, N, Vz, My
			Add(1, 1, 0.000, -1.299, -0.227, 0.218);
			Add(1, 1, 0.300, -1.234, -0.227, 0.150);
			Add(1, 1, 0.600, -1.168, -0.227, 0.082);
			Add(1, 1, 0.900, -1.102, -0.227, 0.014);
			Add(1, 1, 1.200, -1.036, -0.227, -0.054);
			Add(1, 1, 1.500, -0.970, -0.227, -0.122);
			Add(1, 1, 1.500, -0.970, -0.227, -0.122);
			Add(1, 1, 1.800, -0.904, -0.227, -0.190);
			Add(1, 1, 2.100, -0.839, -0.227, -0.258);
			Add(1, 1, 2.400, -0.773, -0.227, -0.326);
			Add(1, 1, 2.700, -0.707, -0.227, -0.394);
			Add(1, 1, 3.000, -0.641, -0.227, -0.462);

			Add(1, 2, 0.000, -8.764, -3.103, 2.988);
			Add(1, 2, 0.300, -8.764, -3.103, 2.057);
			Add(1, 2, 0.600, -8.764, -3.103, 1.126);
			Add(1, 2, 0.900, -8.764, -3.103, 0.196);
			Add(1, 2, 1.200, -8.764, -3.103, -0.735);
			Add(1, 2, 1.500, -8.764, -3.103, -1.666);
			Add(1, 2, 1.500, -8.764, -3.103, -1.666);
			Add(1, 2, 1.800, -8.764, -3.103, -2.597);
			Add(1, 2, 2.100, -8.764, -3.103, -3.528);
			Add(1, 2, 2.400, -8.764, -3.103, -4.459);
			Add(1, 2, 2.700, -8.764, -3.103, -5.390);
			Add(1, 2, 3.000, -8.764, -3.103, -6.321);

			Add(1, 3, 0.000, -2.921, -1.034, 0.996);
			Add(1, 3, 0.300, -2.921, -1.034, 0.686);
			Add(1, 3, 0.600, -2.921, -1.034, 0.375);
			Add(1, 3, 0.900, -2.921, -1.034, 0.065);
			Add(1, 3, 1.200, -2.921, -1.034, -0.245);
			Add(1, 3, 1.500, -2.921, -1.034, -0.555);
			Add(1, 3, 1.500, -2.921, -1.034, -0.555);
			Add(1, 3, 1.800, -2.921, -1.034, -0.866);
			Add(1, 3, 2.100, -2.921, -1.034, -1.176);
			Add(1, 3, 2.400, -2.921, -1.034, -1.486);
			Add(1, 3, 2.700, -2.921, -1.034, -1.797);
			Add(1, 3, 3.000, -2.921, -1.034, -2.107);

			Add(2, 1, 0.000, -0.227, 0.641, -0.462);
			Add(2, 1, 0.417, -0.227, 0.550, -0.214);
			Add(2, 1, 0.833, -0.227, 0.458, -0.004);
			Add(2, 1, 1.250, -0.227, 0.367, 0.168);
			Add(2, 1, 1.667, -0.227, 0.275, 0.302);
			Add(2, 1, 2.083, -0.227, 0.184, 0.397);
			Add(2, 1, 2.500, -0.227, 0.092, 0.455);
			Add(2, 1, 2.500, -0.227, 0.092, 0.455);
			Add(2, 1, 2.917, -0.227, 0.001, 0.474);
			Add(2, 1, 3.333, -0.227, -0.091, 0.456);
			Add(2, 1, 3.750, -0.227, -0.182, 0.399);
			Add(2, 1, 4.167, -0.227, -0.273, 0.304);
			Add(2, 1, 4.583, -0.227, -0.365, 0.171);
			Add(2, 1, 5.000, -0.227, -0.456, 0.000);

			Add(2, 2, 0.000, -3.103, 8.764, -6.321);
			Add(2, 2, 0.417, -3.103, 7.514, -2.929);
			Add(2, 2, 0.833, -3.103, 6.264, -0.059);
			Add(2, 2, 1.250, -3.103, 5.014, 2.291);
			Add(2, 2, 1.667, -3.103, 3.764, 4.120);
			Add(2, 2, 2.083, -3.103, 2.514, 5.427);
			Add(2, 2, 2.500, -3.103, 1.264, 6.215);
			Add(2, 2, 2.500, -3.103, 1.264, 6.215);
			Add(2, 2, 2.917, -3.103, 0.014, 6.481);
			Add(2, 2, 3.333, -3.103, -1.236, 6.226);
			Add(2, 2, 3.750, -3.103, -2.486, 5.451);
			Add(2, 2, 4.167, -3.103, -3.736, 4.155);
			Add(2, 2, 4.583, -3.103, -4.986, 2.338);
			Add(2, 2, 5.000, -3.103, -6.236, 0.000);

			Add(2, 3, 0.000, -1.034, 2.921, -2.107);
			Add(2, 3, 0.417, -1.034, 2.505, -0.976);
			Add(2, 3, 0.833, -1.034, 2.088, -0.020);
			Add(2, 3, 1.250, -1.034, 1.671, 0.764);
			Add(2, 3, 1.667, -1.034, 1.255, 1.373);
			Add(2, 3, 2.083, -1.034, 0.838, 1.809);
			Add(2, 3, 2.500, -1.034, 0.421, 2.072);
			Add(2, 3, 2.500, -1.034, 0.421, 2.072);
			Add(2, 3, 2.917, -1.034, 0.005, 2.160);
			Add(2, 3, 3.333, -1.034, -0.412, 2.075);
			Add(2, 3, 3.750, -1.034, -0.829, 1.817);
			Add(2, 3, 4.167, -1.034, -1.245, 1.385);
			Add(2, 3, 4.583, -1.034, -1.662, 0.779);
			Add(2, 3, 5.000, -1.034, -2.079, 0.000);
		}

		private void Add(int memberId, int loadCaseId, double position, double n, double vz, double my)
		{
			_results.Add((memberId, loadCaseId, new FeaMemberResult(position, n, 0.0, vz, 0.0, my, 0.0)));
		}
	}
}
