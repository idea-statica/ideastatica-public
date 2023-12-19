using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using System.Diagnostics;
using System.Management;

namespace IdeaStatiCa.TeklaStructuresPlugin.Hooks
{
	internal class AppVisibility : IPluginHook
	{
		public AppVisibility()
		{
		}
		public void EnterImport(CountryCode countryCode)
		{

		}

		public void EnterImportSelection(RequestedItemsType requestedType)
		{
			var teklaStructureProcess = GetParentProcess();
			if (teklaStructureProcess != null)
			{
				WindowVisibilityHelper.ForceForegroundWindow(teklaStructureProcess.MainWindowHandle);
			}
		}

		public void ExitImport(CountryCode countryCode)
		{

		}

		public void ExitImportSelection(RequestedItemsType requestedType)
		{

		}

		private static Process GetParentProcess()
		{
			try
			{
				var myId = Process.GetCurrentProcess().Id;
				var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myId);
				var search = new ManagementObjectSearcher("root\\CIMV2", query);
				var results = search.Get().GetEnumerator();
				search.Dispose();
				results.MoveNext();
				var queryObj = results.Current;
				var parentId = (uint)queryObj["ParentProcessId"];
				var parent = Process.GetProcessById((int)parentId);
				return parent;
			}
			catch
			{
				return null;
			}
		}
	}
}
