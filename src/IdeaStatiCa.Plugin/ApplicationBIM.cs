using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
#if NET48
	[ErrorHandlerAttribute(typeof(WcfErrorHandler))]
#endif
	public abstract class ApplicationBIM : IApplicationBIM
	{
		protected IPluginLogger ideaLoggerBase; 

		protected abstract string ApplicationName { get; }

		public abstract void ActivateInBIM(List<BIMItemId> items);

		/// <summary>
		/// communication canal for sending message to the checkbot
		/// </summary>
		public IIdeaStaticaApp IdeaStaticaApp { get; set; }
		/// <summary>
		/// communication canal for sending progress messages
		/// </summary>
		public IProgressMessaging Progress { get; set; }

		public virtual List<BIMItemId> GetActiveSelection() => null;

		public int Id { get; internal set; }

		/// <summary>
		/// Default contructor
		/// </summary>
		public ApplicationBIM()
		{
			ideaLoggerBase = new NullLogger();
		}

		/// <summary>
		/// Constructor for injecting the instance of a pluginLogger
		/// </summary>
		/// <param name="logger">The wrapper for pluginLogger</param>
		public ApplicationBIM(IPluginLogger logger)
		{
			ideaLoggerBase = logger ?? new NullLogger();
		}

		public ModelBIM GetActiveSelectionModel(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType)
		{
			ideaLoggerBase.LogDebug($"Importing active selection model: county code: {countryCode}, type = {requestedType}.");

			var model = ImportActive(countryCode, requestedType);
			if (model != null)
			{
				ideaLoggerBase.LogTrace("Obtained correct model.");
				model.RequestedItems = requestedType;
			}
			else
			{
				ideaLoggerBase.LogTrace("Obtained null model.");
			}

			return model;
		}

		public string GetActiveSelectionModelXML(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType)
		{
			try
			{
				return GetActiveSelectionModelXMLAsync(countryCode, requestedType).GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				ideaLoggerBase.LogDebug("Import failed", ex);

				throw;
			}
		}

		public async Task<string> GetActiveSelectionModelXMLAsync(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType)
		{
			ideaLoggerBase.LogDebug($"Getting active selection model as XML: county code: {countryCode}, type = {requestedType}.");
			var model = await Task.Run(() => GetActiveSelectionModel(countryCode, requestedType));

			ideaLoggerBase.LogTrace("Converting to XML.");
			return Tools.ModelToXml(model);
		}

		public string GetApplicationName() => ApplicationName;

		public List<ModelBIM> GetModelForSelection(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items)
		{
			ideaLoggerBase.LogDebug($"Importing selection: county code: {countryCode}, {items.Count} item(s).");
			var res = ImportSelection(countryCode, items);

			ideaLoggerBase.LogTrace($"Obtained {res?.Count} model(s).");
			return res;
		}

		public string GetModelForSelectionXML(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items)
		{
			try
			{
				return GetModelForSelectionXMLAsync(countryCode, items).GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				ideaLoggerBase.LogDebug("Import failed", ex);

				throw;
			}
		}

		public async Task<string> GetModelForSelectionXMLAsync(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items)
		{
			ideaLoggerBase.LogDebug($"Getting model for selection as XML: county code: {countryCode}, {items.Count} item(s).");
			var model = await Task.Run(() => GetModelForSelection(countryCode, items));

			ideaLoggerBase.LogTrace("Converting to XML.");
			return Tools.ModelToXml(model);
		}


		public virtual bool IsCAD() => false;

		public Task SelectAsync(List<BIMItemId> items) => Task.Run(() => ActivateInBIM(items));

		public virtual bool IsDataUpToDate()
			=> true;

		protected abstract ModelBIM ImportActive(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType);

		protected abstract List<ModelBIM> ImportSelection(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items);
	}
}