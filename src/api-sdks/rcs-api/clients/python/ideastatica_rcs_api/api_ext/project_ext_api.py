import logging
import os
from ideastatica_rcs_api.api.project_api import ProjectApi
from ideastatica_rcs_api.models.rcs_project import RcsProject

logger = logging.getLogger(__name__)

class ProjectExtApi(ProjectApi):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.active_project_id = None
        # Add any additional initialization here
        

    def open_project_from_file(self, path: str) -> RcsProject:
        """
        Opens a project from the specified file path.

        Args:
            path (str): The file path of the project to open.

        Returns:
            RcsProject: The opened project.
        """
        logger.info(f"Opening the project: {path}")
        try:
            with open(path, 'rb') as file:
                byte_array = file.read()      

            _, ext = os.path.splitext(path)
            ext = ext.lower()

            if ext == '.idearcs':
                upload_res = super().open_project(rcs_file=byte_array, _content_type='multipart/form-data')

            else:
                raise ValueError(f"Unsupported file type: {ext}")

            self.active_project_id = upload_res.project_id
            logger.info(f"Open project_id: {upload_res.project_id}")  

            return upload_res
        
        except Exception as e:
            logger.error(f"Failed to open the project: {path}", exc_info=True)
            raise e


    def save_project(self, project_id, file_name):
        """
        Saves the project with the given project ID to the specified file name.

        Args:
            project_id (str): The ID of the project to save.
            file_name (str): The name of the file to save the project.

        Returns:
            None
        """
        response = self.download_project_with_http_info(project_id)
        with open(file_name, 'wb') as file_stream:
            file_stream.write(response.raw_data)


    def create_project_from_iom_file(self, path) -> RcsProject:
        """
        Creates a project from the specified IOM file path.

        Args:
            path (str): The file path of the IOM file.

        Returns:
            RcsProject: The created project.
        """
        logger.info(f"Opening the project: {path}")
        try:
            with open(path, 'rb') as file:
                byte_array = file.read()      

            _, ext = os.path.splitext(path)
            ext = ext.lower()

            if ext == '.xml':
                upload_res = super().import_iom_file(byte_array, _content_type='multipart/form-data')

            else:
                raise ValueError(f"Unsupported file type: {ext}")

            self.active_project_id = upload_res.project_id
            logger.info(f"Open project_id: {upload_res.project_id}")  

            return upload_res
        
        except Exception as e:
            logger.error(f"Failed to open the project: {path}", exc_info=True)
            raise e