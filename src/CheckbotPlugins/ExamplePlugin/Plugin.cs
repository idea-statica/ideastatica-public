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
		private TaskCompletionSource _tcs = new();

		private readonly IProjectService _projectService;

		public Plugin(IProjectService projectService, IEventService eventService)
		{
			_projectService = projectService;

			eventService.Subscribe(OnEvent);
		}

		public void Run()
		{
			_tcs.Task.GetAwaiter().GetResult();
		}

		private async Task OnEvent(Event evt)
		{
			if (evt is EventOpenCheckApplication openCheckApplication)
			{

				ModelExportOptions exportOptions = new ModelExportOptions();
				exportOptions.WithResults = false;

				OpenModelContainer iom = Tools.DeserializeModel<OpenModelContainer>(_projectService.GetObjects(new[] { openCheckApplication.ModelObject }, ModelExportOptions.Default));
				ConnectionPoint conn = iom.OpenModel.ConnectionPoint[0];

				PrintConnectionInfo(iom, conn);

				string path = SaveToTemp(iom);

				var info = await _projectService.GetInfo();


				ProcessStartInfo startInfo = new()
				{
					FileName = "code",
					UseShellExecute = true
				};
				startInfo.ArgumentList.Add(path);

				var proc = Process.Start(startInfo);
				if(proc == null)
				{
					throw new Exception();
				}
				proc.EnableRaisingEvents = true;
				proc.Exited += Proc_Exited;
			}
			if (evt is EventCustomButtonClicked customButtonClicked)
			{
				Console.WriteLine("Clicked on " + customButtonClicked.ButtonName);
			}
		}

		private void Proc_Exited(object? sender, EventArgs e) => _tcs.SetResult();

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