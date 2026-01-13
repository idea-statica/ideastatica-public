using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.ImportedObjects;
using IdeaStatiCa.RamToIdea.Factories;
using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	public class RamCombiInput : IIdeaCombiInput
	{
		public RamCombiInput(IObjectFactory objectFactory, ILoadCombination ramCombination)
		{
			Id = ramCombination.lUID.ToString();

			Name = $"CO {Id} ({ramCombination.strDisplayString})";

			// Use this method to get LoadCases which make up combination and factors
			ILoadCombinationTerms comboTerms = ramCombination.GetLoadCombinationTerms();

			int length = comboTerms.GetCount();
			for (int i = 0; i < length; i++)
			{
				ILoadCombinationTerm term = comboTerms.GetAt(i);
				var ci = new CombiItemBIM
				{
					Coeff = term.dFactor,
					LoadCase = objectFactory.GetLoadCase(term.lLoadCaseID),
				};

				CombiItems.Add(ci);
			}
		}

		public TypeOfCombiEC TypeCombiEC => TypeOfCombiEC.ULS;

		public TypeCalculationCombiEC TypeCalculationCombi => TypeCalculationCombiEC.Linear;

		public List<IIdeaCombiItem> CombiItems { get; } = new List<IIdeaCombiItem>();

		public string Id { get; }

		public string Name { get; }
	}
}