using IdeaRS.OpenModel.Material;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		public static async Task GetAndAddBoltAssemblies(ConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			//Get projectId Guid
			Guid projectId = conProject.ProjectId;

			//Create a map of Name and Id. Used if Prefer to use Name.
			Dictionary<string, int> BoltGradesMap = new Dictionary<string, int>();
			Dictionary<string, int> BoltAssembliesMap = new Dictionary<string, int>();


			//FIX 'Blot' && Upgrade to new IOM Bolt Grade Definitions.
			await conClient.Material.GetBlotGradeMaterialsAsync(projectId);


			//GET AND ADD NEW BOLT ASSEMBLIES

			List<BoltAssembly> boltAssemblies = (await conClient.Material.GetBoltAssembliesAsync(projectId)).Cast<BoltAssembly>().ToList();
			boltAssemblies.ForEach(x => BoltAssembliesMap.Add(x.Name, x.Id));

			//List of new bolt Assemblies to Add.
			List<string> boltAssembliesToAdd = new List<string>() { "M16 8.8", "M20 8.8", "M24 8.8" };

			foreach (var assembly in boltAssembliesToAdd)
			{
				//Only Add Assemblies which are not in the model currently.
				if (!BoltAssembliesMap.ContainsKey(assembly))
				{
					//FIX: This should Output the created Bolt Assembly Object. We need the ID.
					ConMprlElement added = await conClient.Material.AddBoltAssemblyAsync(projectId, new ConMprlElement(assembly));
					Console.WriteLine("Successfully Added new Bolt Assembly: " + added.MprlName);

					//Need to check what happens if name is not found...
				}
				else
					Console.WriteLine("Assembly already in project:" + assembly);
			}

			string exampleFolder = GetExampleFolderPathOnDesktop("GetAndAddMaterials");
			string fileName = "simple cleat connection-added materials.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);

			//Save the applied template
			await conClient.Project.SaveProjectAsync(projectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			await conClient.Project.CloseProjectAsync(projectId.ToString());
		}
	}
}
