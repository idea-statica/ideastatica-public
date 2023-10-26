using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IdeaStatiCa.BimApiLink
{
	internal class ApplicationBimRunnable : IApplicationBimRunnable
	{
		private readonly IApplicationBIM _app;
		private readonly IBimHostingFactory _bimHostingFactory;
		private readonly IPluginLogger _logger;
		private readonly string _name;
		private readonly string _ideaStatiCaPath;
		private readonly string _projectPath;

		internal ApplicationBimRunnable(
			IApplicationBIM app,
			IBimHostingFactory bimHostingFactory,
			IPluginLogger logger,
			string name,
			string ideaStatiCaPath,
			string projectPath)
		{
			Debug.Assert(app != null);
			Debug.Assert(bimHostingFactory != null);
			Debug.Assert(logger != null);
			Debug.Assert(!string.IsNullOrEmpty(name));
			Debug.Assert(!string.IsNullOrEmpty(ideaStatiCaPath));
			Debug.Assert(!string.IsNullOrEmpty(projectPath));

			_app = app;
			_bimHostingFactory = bimHostingFactory;
			_logger = logger;
			_name = name;
			_ideaStatiCaPath = ideaStatiCaPath;
			_projectPath = projectPath;
		}

		public Task Run()
		{
			PluginFactory pluginFactory = new PluginFactory(
				this,
				_name,
				_ideaStatiCaPath);

			IBIMPluginHosting pluginHosting = _bimHostingFactory.Create(pluginFactory, _logger);

#if NET6_0
			string pid = Environment.ProcessId.ToString();
#else
			string pid = Process.GetCurrentProcess().Id.ToString();
#endif
			return pluginHosting.RunAsync(pid, _projectPath);
		}

		public List<BIMItemId> GetActiveSelection()
			=> _app.GetActiveSelection();

		public string GetActiveSelectionModelXML(CountryCode countryCode, RequestedItemsType requestedType)
			=> _app.GetActiveSelectionModelXML(countryCode, requestedType);

		public Task<string> GetActiveSelectionModelXMLAsync(CountryCode countryCode, RequestedItemsType requestedType)
			=> _app.GetActiveSelectionModelXMLAsync(countryCode, requestedType);

		public string GetApplicationName()
			=> _app.GetApplicationName();

		public string GetModelForSelectionXML(CountryCode countryCode, List<BIMItemsGroup> items)
			=> _app.GetModelForSelectionXML(countryCode, items);

		public Task<string> GetModelForSelectionXMLAsync(CountryCode countryCode, List<BIMItemsGroup> items)
			=> _app.GetModelForSelectionXMLAsync(countryCode, items);

		public bool IsCAD()
			=> _app.IsCAD();

		public bool IsDataUpToDate()
			=> _app.IsDataUpToDate();

		public Task SelectAsync(List<BIMItemId> items)
			=> _app.SelectAsync(items);
	}
}