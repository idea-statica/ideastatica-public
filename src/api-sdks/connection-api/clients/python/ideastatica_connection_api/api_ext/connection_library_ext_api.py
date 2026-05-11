import os

from ideastatica_connection_api.api.connection_library_api import ConnectionLibraryApi


class ConnectionLibraryExtApi(ConnectionLibraryApi):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)


    def save_design_item_picture(self, design_set_id: str, design_item_id: str, file_name: str):
        """
        Downloads the picture (PNG) for a given design item and saves it to the specified file.

        Args:
            design_set_id (str): The unique identifier of the design set.
            design_item_id (str): The unique identifier of the design item for which the picture is requested.
            file_name (str): The full path of the PNG file to write (will be created or overwritten).

        Returns:
            None
        """

        response = super().get_design_item_picture_with_http_info(
            design_set_id=design_set_id,
            design_item_id=design_item_id,
        )

        png_bytes = response.raw_data

        if not png_bytes:
            raise RuntimeError("Picture response is empty.")

        if not png_bytes.startswith(b"\x89PNG"):
            raise RuntimeError("Picture response is not PNG data.")

        directory = os.path.dirname(file_name)
        if directory:
            os.makedirs(directory, exist_ok=True)

        with open(file_name, 'wb') as file:
            file.write(png_bytes)
