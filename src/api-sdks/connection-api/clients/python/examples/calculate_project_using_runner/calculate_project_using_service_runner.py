import logging
from pathlib import Path
import sys
import os
import json
from pprint import pprint
import asyncio

# Get the parent directory and add the parent directory to sys.path
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), 'ideastatica_connection_api')))

import ideastatica_connection_api
from ideastatica_connection_api.connection_api_service_runner import ConnectionApiServiceRunner

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Define the path to the executable
PROGRAM_FILES = os.environ.get("ProgramFiles", "C:\\Program Files")
SETUP_DIR = Path(PROGRAM_FILES) / "IDEA StatiCa" / "StatiCa 24.1" / "net6.0-windows"

# Path to the project file
CURRENT_DIR = os.path.dirname(os.path.realpath(__file__))
PROJECT_FILE_PATH = os.path.realpath(os.path.join(CURRENT_DIR, "..", "projects", "HSS_norm_cond.ideaCon"))

async def main():
    # Start api service 
    async with ConnectionApiServiceRunner(SETUP_DIR) as service_runner:
        # Create API client attached to started service
        with service_runner.create_api_client() as api_client:
            try:
                # Open project
                upload_res = api_client.project.open_project_from_filepath(PROJECT_FILE_PATH)
                logger.info("Project opened successfully.")

                # Get project data
                project_data = api_client.project.get_project_data(api_client.project.active_project_id)
                pprint(project_data)

                # Get list of all connections in the project
                connections_in_project = api_client.connection.get_connections(api_client.project.active_project_id)

                # First connection in the project
                connection1 = connections_in_project[0]
                pprint(connection1)

                # Run stress-strain CBFEM analysis for the first connection
                calc_params = ideastatica_connection_api.ConCalculationParameter()
                calc_params.connection_ids = [connection1.id]

                con1_cbfem_results = api_client.calculation.calculate(api_client.project.active_project_id, calc_params)
                pprint(con1_cbfem_results)

                # Get detailed results
                results_text = api_client.calculation.get_raw_json_results(api_client.project.active_project_id, calc_params)
                first_connection_result = results_text[0]
                raw_results = json.loads(first_connection_result)
                pprint(raw_results)

                # Modify setup
                connection_setup = api_client.project.get_setup(api_client.project.active_project_id)
                connection_setup.hss_limit_plastic_strain = 0.02
                modified_setup = api_client.project.update_setup(api_client.project.active_project_id, connection_setup)

                # Recalculate connection
                recalculate_results = api_client.calculation.calculate(api_client.project.active_project_id, calc_params)
                pprint(recalculate_results)

            except Exception as e:
                logger.error(f"Operation failed: {e}")

# Run the asynchronous main function
if __name__ == "__main__":
    asyncio.run(main())
