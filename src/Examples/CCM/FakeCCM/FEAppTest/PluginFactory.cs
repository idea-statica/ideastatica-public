using IdeaStatiCa.Plugin;
using System.IO;

namespace FEAppTest
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


		public string IdeaStaticaAppPath => @"C:\code\IdeaStatiCa\bin\Debug\IdeaCodeCheck.exe";
	}
}