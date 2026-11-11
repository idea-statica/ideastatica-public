using BimApiLinkFeaExample.BimApi;
using FeaExampleApi;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BimApiLinkFeaExample.Importers
{
	/// <summary>
	/// Imports the internal forces of members for every load case. Checkbot builds the forces of each combination from
	/// them.
	/// </summary>
	internal class ResultsImporter : IInternalForcesImporter<IIdeaMember1D>
	{
		/// <summary>
		/// The FEA application gives forces in kN and moments in kNm; IDEA StatiCa takes N and Nm.
		/// </summary>
		private const double KiloToBase = 1000.0;

		/// <summary>
		/// The smallest step kept between two results, as a fraction of the member length.
		/// </summary>
		private const double MinimumStep = 1e-5;

		private readonly IFeaApi _fea;

		public ResultsImporter(IFeaApi fea)
		{
			_fea = fea;
		}

		/// <summary>
		/// The internal forces of the members for every load case, in each member's own axes. The members are the
		/// ones <see cref="MemberImporter"/> created.
		/// </summary>
		public IEnumerable<ResultsData<IIdeaMember1D>> GetResults(IReadOnlyList<IIdeaMember1D> members)
		{
			var builder = new InternalForcesBuilder<IIdeaMember1D>(ResultLocalSystemType.Local);

			foreach (Member1D member in members)
			{
				int memberId = (int)member.Identifier.GetId();
				double length = _fea.Geometry.GetMemberLength(memberId);

				foreach (FeaLoadCase loadCase in _fea.Loads.GetAllLoadCases())
				{
					InternalForcesBuilder<IIdeaMember1D>.Sections sections =
						builder.For(member, new IntIdentifier<IIdeaLoadCase>(loadCase.Id));

					IReadOnlyList<FeaMemberResult> results = _fea.Results.GetMemberResults(memberId, loadCase.Id);
					IReadOnlyList<double> positions = GetPositions(results, length);
					for (int i = 0; i < results.Count; i++)
					{
						Add(sections, positions[i], results[i]);
					}
				}
			}

			return builder;
		}

		/// <summary>
		/// The position of each result as a fraction of the member length, from 0 at its start to 1 at its end.
		/// </summary>
		/// <remarks>
		/// An FEA application can give more than one result at a position, for example on both sides of a point load.
		/// A section takes only one result per load case, so each result is moved, where needed, to at least a tiny step
		/// after the one before it. Then, going back from the end, each result keeps that step to the one after it, and
		/// the moves never push the last one past the end of the member.
		/// </remarks>
		private static IReadOnlyList<double> GetPositions(IReadOnlyList<FeaMemberResult> results, double length)
		{
			List<double> positions = results.Select(result => result.Position / length).ToList();

			for (int i = 1; i < positions.Count; i++)
			{
				positions[i] = Math.Max(positions[i], positions[i - 1] + MinimumStep);
			}

			int last = positions.Count - 1;
			for (int i = last; i >= 0; i--)
			{
				double limit = i == last ? Math.Max(1, results[i].Position / length) : positions[i + 1] - MinimumStep;
				positions[i] = Math.Min(positions[i], limit);
			}

			return positions;
		}

		/// <summary>
		/// Adds one result at the position, in N and Nm.
		/// </summary>
		private static void Add(InternalForcesBuilder<IIdeaMember1D>.Sections sections, double position, FeaMemberResult result)
		{
			sections.Add(
				position,
				result.N * KiloToBase,
				result.Vy * KiloToBase,
				result.Vz * KiloToBase,
				result.Mx * KiloToBase,
				result.My * KiloToBase,
				result.Mz * KiloToBase);
		}
	}
}
