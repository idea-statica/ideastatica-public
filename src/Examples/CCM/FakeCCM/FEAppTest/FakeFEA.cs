using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FEAppTest
{
	public class FakeFEA : ApplicationBIM
	{
		private IHistoryLog log;
		private ModelBIM feaModel;

		public FakeFEA(IHistoryLog log, ModelBIM feaModel = null)
		{
			this.log = log;
		}

		public ModelBIM FeaModel { get => feaModel; set => feaModel = value; }

		protected override string ApplicationName => "My FEA";

		public override void ActivateInBIM(List<BIMItemId> items)
		{
			if (items == null)
			{
				log.Add("Unselect all");
				return;
			}

			log.Add($"ActivateInFEA {items.Take(1).Select(i => $"{i.Type}: {i.Id}").FirstOrDefault()}");
			Thread.Sleep(5000);
			log.Add("ActivateInFEA finished");
		}

		protected override ModelBIM ImportActive(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType)
		{
			log.Add("ImportActive");
			return FeaModel;
		}

		protected override List<ModelBIM> ImportSelection(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items)
		{
			log.Add("ImportSelection");
			return new List<ModelBIM>() { FeaModel };
		}
	}
}