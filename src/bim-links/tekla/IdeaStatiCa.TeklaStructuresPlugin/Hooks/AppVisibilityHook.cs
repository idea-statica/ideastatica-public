using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Management;

namespace IdeaStatiCa.TeklaStructuresPlugin.Hooks
{
	internal class AppVisibility : IPluginHook
	{
		protected IPluginLogger _plugInLogger { get; }
		protected bool _skipHook = false;

		public AppVisibility(IPluginLogger plugInLogger)
		{
			_plugInLogger = plugInLogger;

			string configvalue = ConfigurationManager.AppSettings["skipAppVisibilityHook"];

			if (string.IsNullOrEmpty(configvalue))
			{
				_plugInLogger.LogDebug("AppVisibility hook is enabled by default");
				_skipHook = false;
			}
			else if (string.Equals(configvalue, "true", StringComparison.CurrentCultureIgnoreCase))
			{
				_plugInLogger.LogDebug("AppVisibility hook is enabled by config");
				_skipHook = true;
			}
			else
			{
				_plugInLogger.LogDebug($"AppVisibility hook is disabled by default unknown config value {configvalue} ");
				_skipHook = false;
			}
		}
		public void EnterImport(CountryCode countryCode)
		{

		}

		public void EnterImportSelection(RequestedItemsType requestedType)
		{

			_plugInLogger.LogDebug("AppVisibility - EnterImportSelection");
			if (!_skipHook)
			{
				var teklaStructureProcess = GetParentProcess(_plugInLogger);

				if (teklaStructureProcess != null)
				{
					_plugInLogger.LogDebug("AppVisibility - found parent process - force to popup window");
					WindowVisibilityHelper.ForceForegroundWindow(teklaStructureProcess.MainWindowHandle);
				}
				else
				{
					_plugInLogger.LogDebug("AppVisibility - not found parent process");
				}
			}
			else
			{
				_plugInLogger.LogDebug("AppVisibility - force window skipped by configuration");
			}
		}

		public void ExitImport(CountryCode countryCode)
		{

		}

		public void ExitImportSelection(RequestedItemsType requestedType)
		{

		}

		private static Process GetParentProcess(IPluginLogger plugInLogger)
		{
			try
			{
				var myId = Process.GetCurrentProcess().Id;
				plugInLogger.LogDebug($"AppVisibility - GetParentProcess GetCurrentProcess id {myId}");
				var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myId);
				var search = new ManagementObjectSearcher("root\\CIMV2", query);

				var results = search.Get().GetEnumerator();
				plugInLogger.LogDebug($"AppVisibility - GetParentProcess search {query} result {results}");
				search.Dispose();
				results.MoveNext();
				var queryObj = results.Current;
				var parentId = (uint)queryObj["ParentProcessId"];

				plugInLogger.LogDebug($"AppVisibility - parent GetProcessById id {parentId} ");

				var parent = Process.GetProcessById((int)parentId);
				return parent;
			}
			catch (Exception e)
			{
				plugInLogger.LogDebug("AppVisibility - GetParentProcess find parent failed", e);
				return null;
			}
		}
	}
}
