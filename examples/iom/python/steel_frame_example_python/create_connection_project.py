"""Creates an IDEA StatiCa Connection project from SteelFrame.xml, which steel_frame.py saves, and saves the project as
SteelFrame.ideaCon. It uses the Connection API of the IDEA StatiCa installed on this computer.

Run it in the folder with SteelFrame.xml, with the folder IDEA StatiCa is installed in as the first argument, or without
one for the default folder:

    python create_connection_project.py "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.1"
"""

import asyncio
import sys
from pathlib import Path

from ideastatica_connection_api.connection_api_service_runner import ConnectionApiServiceRunner

DEFAULT_IDEA_FOLDER = r"C:\Program Files\IDEA StatiCa\StatiCa 26.1"


async def main():
	idea_folder = Path(sys.argv[1] if len(sys.argv) > 1 else DEFAULT_IDEA_FOLDER)
	if not (idea_folder / ConnectionApiServiceRunner.API_EXECUTABLE_NAME).exists():
		sys.exit(f"IDEA StatiCa is not in '{idea_folder}'. Give the folder it is installed in as the first argument.")

	iom_path = Path("SteelFrame.xml").resolve()
	if not iom_path.exists():
		sys.exit(f"There is no {iom_path}. Run steel_frame.py in this folder first.")

	project_path = Path("SteelFrame.ideaCon").resolve()
	print(f"Starting the Connection API of {idea_folder} ...")
	async with ConnectionApiServiceRunner(str(idea_folder)) as runner:
		with runner.create_api_client() as client:
			project = client.project.open_project_from_filepath(str(iom_path))
			print("Connections in the project: " + ", ".join(connection.name for connection in project.connections))

			client.project.download_project(project.project_id, str(project_path))

	print(f"The project is saved in {project_path}")


if __name__ == "__main__":
	asyncio.run(main())
