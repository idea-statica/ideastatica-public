import logging
from ideastatica_connection_api.api.project_api import ProjectApi
from ideastatica_connection_api.api_response import ApiResponse

logger = logging.getLogger(__name__)

class ProjectExtApi(ProjectApi):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        # Add any additional initialization here

    def download_project(
                self,
                projectId : str,
                fileName : str):
            """
            Downloads a project with the given projectId and saves it to the specified fileName.

            Args:
                projectId (str): The ID of the project to download.
                fileName (str): The name of the file to save the downloaded project.

            Returns:
                None
            """

            response = super().download_project_with_http_info(projectId)
            with open(fileName, 'wb') as file:
                file.write(response.raw_data)

            pass
