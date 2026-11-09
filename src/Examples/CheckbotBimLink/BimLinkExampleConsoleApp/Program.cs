using BimApiLinkCadExample;
using CadExampleApi;
using System;
using System.IO;
using System.Threading.Tasks;

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
			string checkbotPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.1\\IdeaCheckbot.exe";
			string projectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CadExampleCheckbotProject");
			Task.Run(() => CheckbotLink.Run(checkbotPath, projectPath, cadApi, Logger)).Wait();
		}
	}
}