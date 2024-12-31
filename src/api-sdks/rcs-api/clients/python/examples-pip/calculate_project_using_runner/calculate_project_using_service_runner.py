import os
import asyncio
from pathlib import Path
import ideastatica_rcs_api
from ideastatica_rcs_api.rcs_api_service_runner import RcsApiServiceRunner

# Define the path to the executable
PROGRAM_FILES = os.environ.get("ProgramFiles", "C:\\Program Files")
SETUP_DIR = Path(PROGRAM_FILES) / "IDEA StatiCa" / "StatiCa 24.1" / "net6.0-windows"

# Path to the project file
CURRENT_DIR = os.path.dirname(os.path.realpath(__file__))
PROJECT_FILE_PATH = os.path.realpath(os.path.join(CURRENT_DIR, "..", "..", "projects", "Project1.ideaRcs"))

async def main():
    # Initialize service runner
    async with RcsApiServiceRunner(SETUP_DIR) as runner:
        # Create API client
        with runner.create_api_client() as api_client:
            # Open project
            upload_res = api_client.project.open_project_from_file(PROJECT_FILE_PATH)

            # Get the project data
            project_data = api_client.project.get_active_project()

            # Prepare calculation parameters
            calc_params = ideastatica_rcs_api.RcsCalculationParameters()
            calc_params.sections = [project_data.sections[0].id]

            # Run stress-strain analysis for the connection
            calc_results = api_client.calculation.calculate(
                api_client.project.active_project_id,
                calc_params
            )

            # Get results
            section_results = calc_results[0]

            print(f"Results section id : {section_results.section_id}\n")
            for item in section_results.overall_items:
                print(f"Status: {item.result_type} Check Value: {item.check_value}")


asyncio.run(main())
