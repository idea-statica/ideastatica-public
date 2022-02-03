using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	[ErrorHandlerAttribute(typeof(WcfErrorHandler))]
	public abstract class ApplicationBIM : IApplicationBIM
	{
		private IPluginLogger ideaLogger; //= IdeaDiagnostics.GetLogger("bim.plugin.application");

		protected abstract string ApplicationName { get; }

		public abstract void ActivateInBIM(List<BIMItemId> items);

		/// <summary>
		/// communication canal for sending message to the checkbot
		/// </summary>
		public IIdeaStaticaApp IdeaStaticaApp { get; set; }

		public virtual List<BIMItemId> GetActiveSelection() => null;

		public int Id { get; internal set; }

		/// <summary>
		/// Default contructor
		/// </summary>
		public ApplicationBIM()
		{
			ideaLogger = new NullLogger();
		}

		/// <summary>
		/// Constructor for injecting the instance of a logger
		/// </summary>
		/// <param name="logger">The wrapper for logger</param>
		public ApplicationBIM(IPluginLogger logger)
		{
			ideaLogger = logger ?? new NullLogger();
		}

		public ModelBIM GetActiveSelectionModel(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType)
		{
			ideaLogger.LogDebug($"Importing active selection model: county code: {countryCode}, type = {requestedType}.");

			var model = ImportActive(countryCode, requestedType);
			if (model != null)
			{
				ideaLogger.LogTrace("Obtained correct model.");
				model.RequestedItems = requestedType;
			}
			else
			{
				ideaLogger.LogTrace("Obtained null model.");
			}

			return model;
		}

		public string GetActiveSelectionModelXML(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType)
		{
			ideaLogger.LogDebug($"Getting active selection model as XML: county code: {countryCode}, type = {requestedType}.");
			var model = GetActiveSelectionModel(countryCode, requestedType);

			ideaLogger.LogTrace("Converting to XML.");
			return Tools.ModelToXml(model);
		}

		public string GetApplicationName() => ApplicationName;

		public List<ModelBIM> GetModelForSelection(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items)
		{
			ideaLogger.LogDebug($"Importing selection: county code: {countryCode}, {items.Count} item(s).");
			var res = ImportSelection(countryCode, items);

			ideaLogger.LogTrace($"Obtained {res.Count} model(s).");
			return res;
		}

		public string GetModelForSelectionXML(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items)
		{
			ideaLogger.LogDebug($"Getting model for selection as XML: county code: {countryCode}, {items.Count} item(s).");
			var model = GetModelForSelection(countryCode, items);

			ideaLogger.LogTrace("Converting to XML.");
			return Tools.ModelToXml(model);
		}

		public virtual bool IsCAD() => false;

		public Task SelectAsync(List<BIMItemId> items) => Task.Run(() => ActivateInBIM(items));

		protected abstract ModelBIM ImportActive(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType);

		protected abstract List<ModelBIM> ImportSelection(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items);
	}
}