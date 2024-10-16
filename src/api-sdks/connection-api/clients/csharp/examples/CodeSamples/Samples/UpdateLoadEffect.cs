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
        public static async Task UpdateLoadEffect(ConnectionApiClient conClient)
        {
            string filePath = "inputs/simple knee connection.ideaCon";
            ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

            //Get projectId Guid
            Guid projectId = conProject.ProjectId;
            var connections = await conClient.Connection.GetAllConnectionsDataAsync(projectId);
            int connectionId = connections[0].Id;

            ConLoadSettings loadSettings = await conClient.LoadEffect.GetLoadSettingsAsync(projectId, connectionId);

            Console.WriteLine(loadSettings.ToString());

            // Get Load Effects
            List<ConLoadEffect> loadEffects = await conClient.LoadEffect.GetLoadEffectsAsync(projectId, connectionId);

            double effectMultiplier = 1.25;

            ConLoadEffect loadEffectBasis = null;

            for (int i = 0; i < loadEffects.Count; i++)
            {
                if (i == 0)
                {
                    loadEffectBasis = loadEffects[i];
                    continue;
                }

                ConLoadEffect loadEffect = loadEffects[i];
                // FIX: LoadEffect provides all the member loadings - even if LoadsInEquilibrium is set to False. Is this correct??

                for (int j = 0; j < loadEffect.MemberLoadings.Count; j++)
                {
                    ConLoadEffectMemberLoad loading = loadEffect.MemberLoadings[j];
                    ConLoadEffectMemberLoad loadingBasis = loadEffectBasis.MemberLoadings[j];

                    loading.SectionLoad.N = loadingBasis.SectionLoad.N * effectMultiplier;
                    loading.SectionLoad.Vy = loadingBasis.SectionLoad.Vy * effectMultiplier;
                    loading.SectionLoad.Vz = loadingBasis.SectionLoad.Vz * effectMultiplier;
                    loading.SectionLoad.Mz = loadingBasis.SectionLoad.Mz * effectMultiplier;
                    loading.SectionLoad.My = loadingBasis.SectionLoad.My * effectMultiplier;
                    loading.SectionLoad.Mx = loadingBasis.SectionLoad.Mx * effectMultiplier;
                }

                // 
                await conClient.LoadEffect.UpdateLoadEffectAsync(projectId, connectionId, loadEffect.Id, loadEffect);

                // Increase each increment by 25% of the original value.
                effectMultiplier += 0.25;
            }

            string exampleFolder = GetExampleFolderPathOnDesktop("ApplyTemplate");
            
            // Save updated file.
            string fileName = "updated-load-effects.ideaCon";
            string saveFilePath = Path.Combine(exampleFolder, fileName);
            await conClient.Project.SaveProjectAsync(projectId, saveFilePath);

            Console.WriteLine("File saved to: " + saveFilePath);

            //Close the opened project.
            await conClient.Project.CloseProjectAsync(projectId.ToString());


        }
    }
}
