using yjk.FeaApis;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using System.Collections.Generic;
using IdeaStatiCa.Plugin;
using yjk.ViewModels;

namespace yjk.Importers
{
	public class LoadCombinationImporter : IntIdentifierImporter<IIdeaCombiInput>
	{
		private readonly IFeaLoadsApi loadsApi;
		private readonly IPluginLogger _logger = AppLogger.Instance;

		public LoadCombinationImporter(IFeaLoadsApi loadsApi)
		{
			this.loadsApi = loadsApi;
		}

		public override IIdeaCombiInput Create(int id)
		{
			var loadCombi = loadsApi.GetLoadCombination(id);
			_logger.LogInformation($"LoadCombination created: id={id}, name={loadCombi.Name}, category={loadCombi.Category}, type={loadCombi.Type}");
			return new IdeaCombiInput(id)
			{
				Name = loadCombi.Name,
				TypeCombiEC = GetCombiType(loadCombi.Category, id),
				TypeCalculationCombi = GetCalculationType(loadCombi.Type, id),
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

		private TypeCalculationCombiEC GetCalculationType(Type loadCombiType, int id)
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
					_logger.LogWarning($"LoadCombination id={id}: unrecognised calculation type {loadCombiType}, defaulting to Linear");
					return TypeCalculationCombiEC.Linear;
			}
		}

		private TypeOfCombiEC GetCombiType(Category combiCategory, int id)
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
					_logger.LogWarning($"LoadCombination id={id}: unrecognised category {combiCategory}, defaulting to ULS");
					return TypeOfCombiEC.ULS;
			}
		}
	}
}
