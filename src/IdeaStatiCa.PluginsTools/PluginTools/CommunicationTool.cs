using IdeaRS.OpenModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using IdeaStatiCa.Diagnostics;

namespace IdeaStatiCa.PluginsTools
{
	static public class CommunicationTool
	{
		private static IIdeaLogger ideaLogger = IdeaDiagnostics.GetLogger("ideastatica.plugintools");

		public static void SendToServer(OpenModelTuple openModelContainer, string dataFilePath = "")
		{
			var projectTempFile = GetTempPath();
			if (!Directory.Exists(projectTempFile))
			{
				ideaLogger.LogInformation($"CreateDirectory: {Path.GetDirectoryName(projectTempFile)}");
				Directory.CreateDirectory(Path.GetDirectoryName(projectTempFile));
			}

			try
			{

				{
					XmlSerializer xs = new XmlSerializer(typeof(OpenModelTuple));
					Stream fs = new FileStream(projectTempFile, FileMode.Create);
					XmlTextWriter writer = new XmlTextWriter(fs, Encoding.Unicode)
					{
						// Serialize using the XmlTextWriter.
						Formatting = Formatting.Indented
					};
					xs.Serialize(writer, openModelContainer);
					writer.Close();
					fs.Close();
				}
				ideaLogger.LogInformation("OpenModel Container seriazed");

				var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
				var location = Path.GetDirectoryName(thisAssembly.Location);
				string cmdLine = Path.Combine(location, @"IdeaStatiCa.PluginComunicationProvider.exe");

				ideaLogger.LogInformation($"PluginComunicationProvider path: {cmdLine}");
				using (Process proc = new Process())
				{
					string workDir = dataFilePath;
					if (string.IsNullOrEmpty(dataFilePath))
					{
						workDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
					}
					else
					{
						if (!Directory.Exists(workDir))
						{
							Directory.CreateDirectory(Path.GetDirectoryName(workDir));
						}
					}
					ideaLogger.LogInformation($"Process start -p:{workDir} -o:{projectTempFile}");
					ProcessStartInfo psi = new ProcessStartInfo(cmdLine, string.Format("-p \"{0}\" -o \"{1}\"", workDir, projectTempFile))
					{
						WindowStyle = ProcessWindowStyle.Normal,
						UseShellExecute = false,
						CreateNoWindow = false,
					};

					proc.StartInfo = psi;
					proc.Start();

					if (!proc.WaitForExit(15 * 60 * 1000))
					{
						ideaLogger.LogError("Time out");
						proc.Kill();
						Debug.Fail("Time out");
						throw new InvalidOperationException("Communication time out");
					}
				}

				File.Delete(projectTempFile);
			}
			catch (Exception e)
			{
				ideaLogger.LogError(e.ToString());
				ideaLogger.LogError(e.StackTrace.ToString());
				Debug.Fail(e.ToString());
			}
		}

		private static string GetTempPath()
		{
			return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IdeaStatiCa\\CommunicationProvider", string.Format("{0}.xml", Guid.NewGuid()));
		}

		public static string GetDataFilePath(string projectPath, List<string> listOfIds)
		{
			string subfolder = Path.GetFileNameWithoutExtension(projectPath);
			string path = Path.GetDirectoryName(projectPath);
			string subfolderPath = Path.Combine(path, subfolder);
			Directory.CreateDirectory(Path.Combine(path, subfolder));

			string BeamsIdString = String.Join("-", listOfIds.ToArray());

			return Path.Combine(subfolderPath, BeamsIdString + ".Data.json");
		}
	}
}
