using FeaExampleApi;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using System;
using System.Linq;

namespace BimApiLinkFeaExample.Importers
{
	/// <summary>
	/// Imports a combination by its FEA id, with its load cases.
	/// </summary>
	internal class CombinationImporter : IntIdentifierImporter<IIdeaCombiInput>
	{
		private readonly IFeaLoadsApi _loads;

		public CombinationImporter(IFeaLoadsApi loads)
		{
			_loads = loads;
		}

		/// <summary>
		/// Converts the combination with the given FEA id. Its terms add up: each load case multiplied by its factor.
		/// The load cases are imported by their own importer.
		/// </summary>
		public override IIdeaCombiInput Create(int id)
		{
			FeaCombination combination = _loads.GetCombination(id);

			return new IdeaCombiInput(id)
			{
				Name = combination.Name,
				TypeCombiEC = ToIdea(combination.LimitState),
				TypeCalculationCombi = TypeCalculationCombiEC.Linear,
				CombiItems = combination.Factors
					.Select(factor => CreateItem(id, factor))
					.ToList<IIdeaCombiItem>(),
			};
		}

		/// <summary>
		/// One term of the combination: a load case and its factor.
		/// </summary>
		private IdeaCombiItem CreateItem(int combinationId, FeaCombinationFactor factor)
		{
			return new IdeaCombiItem($"{combinationId}-{factor.LoadCaseId}")
			{
				LoadCase = Get<IIdeaLoadCase>(factor.LoadCaseId),
				Coeff = factor.Factor,
			};
		}

		/// <summary>
		/// The Eurocode combination type for a limit state. The serviceability combinations here are characteristic.
		/// </summary>
		private static TypeOfCombiEC ToIdea(FeaLimitState limitState)
		{
			switch (limitState)
			{
				case FeaLimitState.Ultimate:
					return TypeOfCombiEC.ULS;
				case FeaLimitState.Serviceability:
					return TypeOfCombiEC.SLS_Char;
				default:
					throw new NotSupportedException($"Limit state {limitState} is not supported.");
			}
		}
	}
}
