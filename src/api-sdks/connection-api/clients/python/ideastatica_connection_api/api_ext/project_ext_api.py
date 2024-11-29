import logging
import os
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
    

    def open_project_from_filepath(self, file_path : str):
        """
        Opens a project from a file located at the given file path.
        
        Args:
            file_path (str): The path to the project file to be opened.

        Returns:
            ApiResponse: An ApiResponse object containing the details of the uploaded project.

        Raises:
            ValueError: If the file type is unsupported.
            Exception: If any error occurs while reading or opening the file.
        """
        logger.info(f"Opening the project: {file_path}")
        try:
            with open(file_path, 'rb') as file:
                byte_array = file.read()      

            _, ext = os.path.splitext(file_path)
            ext = ext.lower()

            if ext == '.ideacon':
                uploadRes = super().open_project(idea_con_file=byte_array, _content_type='multipart/form-data')

            elif ext == '.xml' or ext == '.iom':
                uploadRes = super().import_iom(byte_array, _content_type='multipart/form-data')
                
            else:
                raise ValueError(f"Unsupported file type: {ext}")

            logger.info(f"Open project_id: {uploadRes.project_id}")  

            return uploadRes
        
        except Exception as e:
            logger.error(f"Failed to open the project: {file_path}", exc_info=True)
            raise e
         
