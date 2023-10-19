using BimApiLinkCadExample;
using BimApiLinkCadExample.CadExampleApi;

namespace BimLinkExampleConsoleApp
{
	internal class Program
	{
		public static AppLogger Logger { get; } = new AppLogger() { EnableDebug = true, };

		static void Main(string[] args)
		{

			//Api 3rd part app 
			ICadApi cadApi = new CadApi();

			//start checkbot app 
			Task.Run(() => TestPlugin.Run("C:\\Program Files\\IDEA StatiCa\\StatiCa 23.0\\IdeaCheckbot.exe", cadApi, Logger)).Wait();
		}
	}
}