using IdeaStatiCa.Plugin;
using System.IO;

namespace FEAppExample_1
{
	public interface IHistoryLog
	{
		void Add(string action);
	}

	public class PluginFactory : IBIMPluginFactory
	{
		private IHistoryLog log;

		public PluginFactory(IHistoryLog log)
		{
			this.log = log;
		}

		public string FeaAppName => "FakeFEA";

		public IApplicationBIM Create()
		{
			return new FakeFEA(log);
		}

		public string IdeaStaticaAppPath
		{
			get
			{
				return Properties.Settings.Default.IdeaStatiCaDir;
			}
		}
	}
}