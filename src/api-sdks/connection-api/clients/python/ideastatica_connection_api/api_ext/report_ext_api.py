from pydantic import StrictStr, StrictInt

from ideastatica_connection_api.api.report_api import ReportApi


class ReportExtApi(ReportApi):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.active_project_id = None


    def save_report_pdf(self, project_id: StrictStr, connection_id: StrictInt, file_name: str):
        """
        Saves the report for the specified project and connection as a PDF file.

        Args:
            project_id (str): The ID of the project.
            connection_id (int): The ID of the connection.
            file_name (str): The name of the file to save the report.

        Returns:
            None
        """
        response = super().generate_pdf_with_http_info(project_id, connection_id)
        with open(file_name, 'wb') as file:
            file.write(response.raw_data)

    def save_multiple_report_pdf(self, project_id: str, connection_ids: list[int], file_name: str):
        """
        Saves the report for the specified project and multiple connections as a PDF file.

        Args:
            project_id (str): The ID of the project.
            connection_ids list[int]: The list of the connection ids.
            file_name (str): The name of the file to save the report.

        Returns:
            None
        """
        response = super().generate_pdf_for_mutliple_with_http_info(
            project_id,
            connection_ids,
            "application/octet-stream"
        )
        with open(file_name, 'wb') as file:
            file.write(response.raw_data)

    def save_report_word(self, project_id: StrictStr, connection_id: StrictInt, file_name: str):
        """
        Saves the report for the specified project and connection as a Word file.

        Args:
            project_id (str): The ID of the project.
            connection_id (int): The ID of the connection.
            file_name (str): The name of the file to save the report.

        Returns:
            None
        """
        response = super().generate_word_with_http_info(project_id, connection_id)
        with open(file_name, 'wb') as file:
            file.write(response.raw_data)

    def save_multiple_report_word(self, project_id: str, connection_ids: list[int], file_name: str):
        """
        Saves the report for the specified project and multiple connections as a Word file.

        Args:
            project_id (str): The ID of the project.
            connection_ids list[int]: The list of the connection ids.
            file_name (str): The name of the file to save the report.

        Returns:
            None
        """
        response = super().generate_word_for_multiple_with_http_info(
            project_id,
            connection_ids,
            "application/octet-stream"
        )
        with open(file_name, 'wb') as file:
            file.write(response.raw_data)