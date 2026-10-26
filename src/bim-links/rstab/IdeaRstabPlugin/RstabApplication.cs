using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Providers;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaRstabPlugin
{
	internal class RstabApplication : BimApiApplication
	{
		private readonly RstabModel _model;
		private readonly ImportSession _importSession;
		private readonly IResultsProvider _resultsProvider;

		private readonly List<IDataCache> _dataCaches = new List<IDataCache>();

		public RstabApplication(IPluginLogger logger, RstabModel model, IObjectRestorer objectRestorer, IGeometryProvider geometryProvider,
			IResultsProvider resultsProvider, ImportSession importSession, string workingDirectory)
			: base(logger, model, objectRestorer, geometryProvider, workingDirectory)
		{
			_model = model;
			_resultsProvider = resultsProvider;
			_importSession = importSession;

			ImportStarted += OnImportStarted;
			SychronizationStarted += OnSychronizationStarted;
		}

		public void AddDataCache(IDataCache dataCache)
		{
			_dataCaches.Add(dataCache);
		}

		protected override string ApplicationName => "RSTAB";

		protected override void Deselect()
		{
			_model.DeselectObjects();
		}

		protected override void Select(IEnumerable<IIdeaObject> objects)
		{
			_model.SelectObject(objects);
		}

		private void OnImportStarted(CountryCode countryCode, RequestedItemsType requestedItems)
		{
			_importSession.Setup(countryCode, requestedItems);

			foreach (IDataCache dataCache in _dataCaches)
			{
				dataCache.Clear();
			}
		}

		private void OnSychronizationStarted(IEnumerable<IIdeaObject> objects)
		{
			foreach (RstabMember member in objects.OfType<RstabMember>())
			{
				_resultsProvider.Prefetch(member.No);
			}
		}
	}
}