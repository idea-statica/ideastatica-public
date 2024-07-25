using BimApiLinkFeaExample.FeaExampleApi;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using System.Collections.Generic;

namespace BimApiLinkFeaExample.Importers
{
	public class LoadCombinationImporter : IntIdentifierImporter<IIdeaCombiInput>
	{
		private readonly IFeaLoadsApi loadsApi;
	
		public LoadCombinationImporter(IFeaLoadsApi loadsApi)
		{
			this.loadsApi = loadsApi;
		}

		public override IIdeaCombiInput Create(int id)
		{
			var loadCombi = loadsApi.GetLoadCombination(id);
			return new IdeaCombiInput(id)
			{
				Name = loadCombi.Name,
				TypeCombiEC = GetCombiType(loadCombi.Category),
				TypeCalculationCombi = GetCalculationType(loadCombi.Type),
				CombiItems = GetCombiInputs(loadCombi.CombiFactors, id),
			};
		}

		private List<IIdeaCombiItem> GetCombiInputs(IEnumerable<CombiFactor> loadFactors, int combiId) 
		{
			var combiInputs = new List<IIdeaCombiItem>();
			foreach (var item in loadFactors)
			{
				var combiInput = new IdeaCombiItem("combi" + combiId + "_loadCase" + item.LoadCaseId)
				{
					Coeff = item.CombiMultiplier,
					LoadCase = GetMaybe<IIdeaLoadCase>(item.LoadCaseId),
				};
				combiInputs.Add(combiInput);
			}
			return combiInputs;
		}

		private static TypeCalculationCombiEC GetCalculationType(Type loadCombiType)
		{
			switch (loadCombiType) 
			{
				case Type.Linear:
					return TypeCalculationCombiEC.Linear;
				case Type.Envelope: 
					return TypeCalculationCombiEC.Envelope;
				case Type.Nonlinear:
					return TypeCalculationCombiEC.NonLinear;
				default: 
					return TypeCalculationCombiEC.Linear;
			}
		}

		private static TypeOfCombiEC GetCombiType(Category combiCategory) 
		{
			switch (combiCategory)
			{
				case Category.ULS:
					return TypeOfCombiEC.ULS;
				case Category.SLS:
					return TypeOfCombiEC.SLS_Char;
				case Category.ALS:
					return TypeOfCombiEC.Accidental;
				default:
					return TypeOfCombiEC.ULS;
			}
		}
	}
}
