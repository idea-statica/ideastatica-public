using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	public abstract class ApplicationBIM : IApplicationBIM
	{
		protected abstract string ApplicationName { get; }

		public abstract void ActivateInBIM(List<BIMItemId> items);

		public virtual List<BIMItemId> GetActiveSelection() => null;

		public int Id { get; internal set; }

		public ModelBIM GetActiveSelectionModel(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType)
		{
			var model = ImportActive(countryCode, requestedType);
			if (model != null)
			{
				model.RequestedItems = requestedType;
			}

			return model;
		}

		public string GetActiveSelectionModelXML(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType)
		{
			var model = GetActiveSelectionModel(countryCode, requestedType);
			string xml = Tools.ModelToXml(model);
			return xml;
		}

		public string GetApplicationName() => ApplicationName;

		public List<ModelBIM> GetModelForSelection(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items)
		{
			var res = ImportSelection(countryCode, items);
			return res;
		}

		public string GetModelForSelectionXML(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items)
		{
			var model = GetModelForSelection(countryCode, items);
			return Tools.ModelToXml(model);
		}

		public virtual bool IsCAD() => false;

		public Task SelectAsync(List<BIMItemId> items) => Task.Run(() => ActivateInBIM(items));

		protected abstract ModelBIM ImportActive(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType);

		protected abstract List<ModelBIM> ImportSelection(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items);
	}
}