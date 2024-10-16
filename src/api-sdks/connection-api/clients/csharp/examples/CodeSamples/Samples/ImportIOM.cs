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
        public static async Task ImportIOM(ConnectionApiClient conClient) 
        {
            string filePath = "Inputs/multiple_connections.xml";
            ConProject conProject = await conClient.Project.CreateProjectFromIomFileAsync(filePath);

            //Get projectId Guid
            Guid projectId = conProject.ProjectId;
            var connections = await conClient.Connection.GetAllConnectionsDataAsync(projectId);
            int connectionId = connections[0].Id;

            string saveFilePath = "connection-file-from-IOM.ideaCon";

            await conClient.Project.SaveProjectAsync(projectId, saveFilePath);
        }
    }
}
