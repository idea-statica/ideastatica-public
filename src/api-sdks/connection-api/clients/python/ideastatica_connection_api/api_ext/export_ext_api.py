from pydantic.v1 import StrictInt
from ideastatica_connection_api.api.export_api import ExportApi


class ExportExtApi(ExportApi):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.active_project_id = None


    def export_ifc_file(self, project_id: str, connection_id: StrictInt, file_name: str):
        """
        Exports the project with the given projectId to an IFC file and saves it to the specified fileName.

        Args:
            project_id (str): The ID of the project to export.
            connection_id (StrictInt): The ID of the connection to export.
            file_name (str): The name of the file to save the exported IFC file.

        Returns:
            None
        """

        response = super().export_ifc_with_http_info(project_id, connection_id)
        with open(file_name, 'wb') as file:
            file.write(response.raw_data)