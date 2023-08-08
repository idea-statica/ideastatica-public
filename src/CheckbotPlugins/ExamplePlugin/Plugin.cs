using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.CheckbotPlugin.Models;
using IdeaStatiCa.CheckbotPlugin.Services;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace ExamplePlugin
{
	internal class Plugin
	{
		private readonly IProjectService _projectService;

		public Plugin(IProjectService projectService, IEventService eventService)
		{
			_projectService = projectService;

			eventService.Subscribe(OnEvent);
		}

		private async Task OnEvent(Event evt)
		{
			if (evt is EventOpenCheckApplication openCheckApplication)
			{

				ModelExportOptions exportOptions = new ModelExportOptions();
				exportOptions.WithResults = false;

				OpenModelContainer iom = await _projectService.GetObjects(new[] { openCheckApplication.ModelObject }, exportOptions);
				ConnectionPoint conn = iom.OpenModel.ConnectionPoint[0];

				PrintConnectionInfo(iom, conn);

				string path = SaveToTemp(iom);

				ProcessStartInfo startInfo = new()
				{
					FileName = "code",
					UseShellExecute = true
				};
				startInfo.ArgumentList.Add(path);

				Process.Start(startInfo);
			}
			if (evt is EventCustomButtonClicked customButtonClicked)
			{
				Console.WriteLine("Clicked on " + customButtonClicked.ButtonName);
			}
		}

		private static void PrintConnectionInfo(OpenModelContainer iom, ConnectionPoint conn)
		{
			Console.WriteLine($"Got connection {conn.Name} with member:");
			foreach (ConnectedMember connectedMember in conn.ConnectedMembers)
			{
				Member1D? member = iom.OpenModel.Member1D.FirstOrDefault(x => x.Id == connectedMember.Id);

				if (member is null)
				{
					continue;
				}

				Console.WriteLine($"    - {member.Name}");
			}
		}

		private string SaveToTemp(OpenModelContainer iom)
		{
			XmlSerializer serializer = new(typeof(OpenModelContainer));

			string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xml");

			using XmlWriter writer = XmlWriter.Create(tempPath, new XmlWriterSettings()
			{
				Indent = true
			});

			serializer.Serialize(writer, iom);

			return tempPath;
		}
	}
}