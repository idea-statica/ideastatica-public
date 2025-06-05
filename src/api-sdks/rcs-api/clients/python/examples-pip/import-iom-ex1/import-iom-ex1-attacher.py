import os
import asyncio
from pprint import pprint
from urllib.parse import urljoin
import ideastatica_rcs_api
import ideastatica_rcs_api.rcs_api_service_attacher as rcs_api_service_attacher

baseUrl = "http://localhost:5000"

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_rcs_api.Configuration(
    host = baseUrl
)

dir_path = os.path.dirname(os.path.realpath(__file__))
iom_file_path = os.path.join(dir_path, r'..\..\projects', 'ImportOpenModel.xml')
rcs_project_file_path = os.path.join(dir_path, r'..\..\projects', 'Project1.ideaRcs')

async def main():
    with rcs_api_service_attacher.RcsApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        # project_data = api_client.project.open_project_from_file(rcs_project_file_path)
        
        # Import project from IOM file
        project_data = api_client.project.import_iom_file(iom_file_path)

        # Get the project data
        project_data2 = api_client.project.get_active_project()

        # api_client.project.save_project(project_data.project_id, r'c:\x\pro1.idearcs')

        api_client.project.close_project(project_data2.project_id)

asyncio.run(main())