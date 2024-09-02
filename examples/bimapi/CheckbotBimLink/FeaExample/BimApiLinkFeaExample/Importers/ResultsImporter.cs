using BimApiLinkFeaExample.FeaExampleApi;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Results;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BimApiLinkFeaExample.Importers
{
	public class ResultsImporter : IInternalForcesImporter<IIdeaMember1D>
	{
		private readonly IFeaLoadsApi loadsApi;
		private readonly IFeaResultsApi resultsApi;

		// BimApi uses basic SI units
		private const int unitConversionFactor = 1000;
		
		// Precision for check, if 2 results are in the same location
		private const double resultSectionPositionPrecision = 1e-6;

		// BimApi can not handle more results in the same location,
		// so in those cases it is necessary shift the location
		private const double shift = 5e-6;

		public ResultsImporter(IFeaLoadsApi loadsApi, IFeaResultsApi resultsApi)
		{ 
			this.loadsApi = loadsApi;
			this.resultsApi = resultsApi;
		}

		public IEnumerable<ResultsData<IIdeaMember1D>> GetResults(IReadOnlyList<IIdeaMember1D> members)
		{
			IntIdentifier<IIdeaLoadCase>[] loadCases = loadsApi.GetLoadCasesIds().Select(x => new IntIdentifier<IIdeaLoadCase>(x)).ToArray();
			InternalForcesBuilder<IIdeaMember1D> builder = new InternalForcesBuilder<IIdeaMember1D>(ResultLocalSystemType.Local);
			
			foreach (IdeaMember1D member in members) 
			{
				foreach (var loadCase in loadCases)
				{
					InternalForcesBuilder<IIdeaMember1D>.Sections sections = builder.For(member, loadCase);
					GetResultsForMember(sections, member, loadCase, resultsApi);
				}
			}

			return builder;
		}

		private static void GetResultsForMember(InternalForcesBuilder<IIdeaMember1D>.Sections sections,
										IdeaMember1D member, IntIdentifier<IIdeaLoadCase> loadCase,
										IFeaResultsApi resultsApi)
		{			
			var results = resultsApi.GetMemberInternalForces((int)member.Identifier.GetId(), loadCase.Id);
			var memberLength = GetLength(member.Elements.First().Segment.StartNode, member.Elements.Last().Segment.EndNode);

			double currentSection = Math.Max(0, results.ElementAt(0).Location / memberLength);						

			for (var i = 1; i < results.Count(); ++i)
			{
				IFeaMemberResult result = results.ElementAt(i-1);
				double nextSection = Math.Min(1, results.ElementAt(i).Location / memberLength);

				// In source application there can be 2 results with the same location
				// (e.g. local load force in node). BimApi is not able to handle that,
				// so as workaround it is necessary to shift results a bit.
				if (currentSection.AlmostEqual(nextSection, resultSectionPositionPrecision))
				{
					
					if (i == 1)
					{
						// first section should be always 0, move next section only
						nextSection += shift;
					}
					else if (i == results.Count() - 1)
					{
						// last section should be always 1, move section before last only
						currentSection -= shift;
					}
					else
					{
						currentSection -= shift / 2.0;
						nextSection += shift / 2.0;
					}
				}

				AddResult(sections, currentSection, result);
				currentSection = nextSection;
			}

			// add the very last result
			AddResult(sections, currentSection, results.Last());
		}

		private static double GetLength(IIdeaNode startNode, IIdeaNode endNode)
		{
			IdeaVector3D vector = new IdeaVector3D(
				startNode.Vector.X - endNode.Vector.X,
				startNode.Vector.Y - endNode.Vector.Y,
				startNode.Vector.Z - endNode.Vector.Z);

			return Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y) + (vector.Z * vector.Z));
		}

		private static void AddResult(InternalForcesBuilder<IIdeaMember1D>.Sections sections, double location, IFeaMemberResult result)
		{
			// Convert source results to correct units
			// and correct coordinate system if necessary
			double N = result.N * unitConversionFactor;
			double Vy = result.Vy * unitConversionFactor;
			double Vz = result.Vz * unitConversionFactor;
			double Tx = result.Mx * unitConversionFactor;
			double My = result.My * unitConversionFactor;
			double Mz = result.Mz * unitConversionFactor;
			sections.Add(location, N, Vy, Vz, Tx, My, Mz);			
		}
	}
}