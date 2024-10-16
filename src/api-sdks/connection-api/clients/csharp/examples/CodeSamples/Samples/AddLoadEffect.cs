﻿using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples
{
    public partial class ClientExamples
    {
        public static async Task AddLoadEffect(ConnectionApiClient conClient)
        {
            string filePath = "inputs/simple cleat connection.ideaCon";
            ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

            //Get projectId Guid
            Guid projectId = conProject.ProjectId;
            var connections = await conClient.Connection.GetAllConnectionsDataAsync(projectId);
            int connectionId = connections[0].Id;

            ConLoadSettings loadSettings = await conClient.LoadEffect.GetLoadSettingsAsync(projectId, connectionId);

            Console.WriteLine(loadSettings.ToString());

            // Get Load Effects
            List<ConLoadEffect> loadEffects = await conClient.LoadEffect.GetLoadEffectsAsync(projectId, connectionId);

            
            Console.WriteLine("Add new Load effect.");
            Console.WriteLine("Specify Name or Hit Enter for Quick Add");
            string input = Console.ReadLine()?? "";

            if (string.IsNullOrEmpty(input))
            {
                //Generic quick add of a load effect
                //FIX: DOES NOT WORK.
                //FIX: Add LoadEffect should return ConLoadEffect
                //FIX: Default should be active.
                LoadEffectData newLoadEffect = await conClient.LoadEffect.AddLoadEffectAsync(projectId, connectionId);
                
                if(newLoadEffect != null) 
                    Console.WriteLine($"Load Effect Added: Name= {newLoadEffect.Name}, Id= {newLoadEffect.Id}"); 
            }
            {
                ConLoadEffect loadEffect = new ConLoadEffect() { Name = input };

                //FIX: DOES WORK.
                LoadEffectData newLoadEffect = await conClient.LoadEffect.AddLoadEffectAsync(projectId, connectionId, loadEffect);
                if (newLoadEffect != null)
                    Console.WriteLine($"Load Effect Added: Name= {newLoadEffect.Name}, Id= {newLoadEffect.Id}");
            }

            string exampleFolder = GetExampleFolderPathOnDesktop("AddLoadEffect");
            
            // Save updated file.
            string fileName = "add-load-effects.ideaCon";
            string saveFilePath = Path.Combine(exampleFolder, fileName);
            await conClient.Project.SaveProjectAsync(projectId, saveFilePath);

            Console.WriteLine("File saved to: " + saveFilePath);

            //Close the opened project.
            await conClient.Project.CloseProjectAsync(projectId.ToString());

        }
    }
}
