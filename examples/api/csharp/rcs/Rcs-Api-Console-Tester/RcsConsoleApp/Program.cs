using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaStatiCa.RcsClient.Client;
using IdeaStatiCa.RcsClient.Factory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RcsConsoleApp
{
	internal class Program
	{
		static void Main(string[] args)
		{

			//RcsClientFactory factory = new RcsClientFactory(new NullLogger());

			//RcsApiClient? client = factory.CreateRcsApiClient() as RcsApiClient;

			//string filepath = "myCrossSectionFile.ideaRcs";

			//var project = client.OpenProject(filepath, System.Threading.CancellationToken.None);

			////List<RcsCrossSectionOverviewModel> sections = client.GetSectionItems(project);
			
			//List<RcsCheckMemberModel> members = client.GetDesignMemberItems(project)[1].Sections();

			////Get Cross-sections on Member
			//List<int> check = sections.Select(x=>x.Id).Where(x=> x.id;

			//client.CalculateProjectAsync(project, )
		}
	}
}