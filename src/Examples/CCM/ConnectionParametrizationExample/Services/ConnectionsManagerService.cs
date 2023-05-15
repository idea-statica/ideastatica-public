using ConnectionParametrizationExample.Models;
using ConnectionParametrizationExample.ViewModels;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Xml;

namespace ConnectionParametrizationExample.Services
{
	public class ConnectionsManagerService
	{
		public event Action<int> ProgressChanged;

		private ConnectionHiddenCheckClient GetConnectionClient(string ideaAppLocation)
		{
			// Create the instance of factory - it looks for IDEA StatiCa in the directory 'ideaStaticaInstallDir'
			ConnHiddenClientFactory calcFactory = new ConnHiddenClientFactory(ideaAppLocation.Replace(@"\\", @"\"));

			// Create the instance of the IDEA StatiCa Client
			ConnectionHiddenCheckClient client = calcFactory.Create();

			return client;
		}

		public void SaveDataToCsv(Dictionary<string, List<string>> data, string csvPath)
		{
			ResultBuilder builder = new ResultBuilder();
			builder.WriteDataToCsv(data, csvPath);
		}

		public Dictionary<string, List<string>> SaveConnectionsCssToCsv(ModelInfoSettings settings)
		{
			ConnectionHiddenCheckClient client = GetConnectionClient(settings.IdeaAppLocation);
			var cssData = new Dictionary<string, List<string>>();

			foreach (var project in settings.IdeaConFiles)
			{
				// Report progress
				double indexOf = settings.IdeaConFiles.FindIndex(x => x == project);
				ProgressChanged?.Invoke((int)(indexOf / settings.IdeaConFiles.Count * 100));

				// Open project on the service side
				client.OpenProject(project);

				// Get connection project info
				var projInfo = client.GetProjectInfo();

				// Iterate over all connections of the project
				if (projInfo?.Connections.Count > 0)
				{
					foreach (var connection in projInfo.Connections)
					{
						var connData = client.GetAllConnectionData(connection.Identifier);

						var conn = new XmlDocument();
						conn.LoadXml(connData);

						XmlNodeList entities = conn.SelectNodes("OpenModelContainer/OpenModel/CrossSection/CrossSection/Name");

						foreach (XmlElement entity in entities)
						{
							var cssName = entity.InnerText;
							var connectionName = $"{System.IO.Path.GetFileNameWithoutExtension(projInfo.Name)}__{connection.Name}";

							if (cssData.ContainsKey(cssName))
							{
								cssData[cssName].Add(connectionName);
							}
							else
							{
								cssData[cssName] = new List<string>() { connectionName };
							}
						}
					}
				}

				client.CloseProject();
			}
			client?.Close();

			// Save data to Csv
			SaveDataToCsv(cssData, System.IO.Path.Combine(settings.IdeaConFilesLocation, "Css.csv"));

			// At the end set progress to 0
			ProgressChanged?.Invoke(0);

			return cssData;
		}
	}
}
