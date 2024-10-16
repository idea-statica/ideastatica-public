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
        public static async Task ExportIom(ConnectionApiClient conClient) 
        {
            string filePath = "Inputs/HSS_norm_cond.ideaCon";
            ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

            //Get projectId Guid
            Guid projectId = conProject.ProjectId;
            var connections = await conClient.Connection.GetAllConnectionsDataAsync(projectId);
            int connectionId = connections[0].Id;


            ConnectionData conData = await conClient.Export.ExportConnectionDataAsync(projectId, connectionId);





            string saveFilePath = "connection-file-from-IOM.ideaCon";

            await conClient.Project.SaveProjectAsync(projectId, saveFilePath);
        }
    }
}
