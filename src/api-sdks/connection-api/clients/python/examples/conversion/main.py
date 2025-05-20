import os
from ideastatica_connection_api import *
import ideastatica_connection_api.connection_api_service_attacher as connection_api_service_attacher

## Configure base URL
base_url = "http://localhost:5000"

# Create client attached to already running service
with connection_api_service_attacher.ConnectionApiServiceAttacher(base_url).create_api_client() as api_client:
    try:
        for i in range(1, 3):  # Adjust range as needed
            # Absolute path to the input model
            project_file_path = os.path.abspath(f'./Models/Model_{i}.ideaCon')

            if not os.path.exists(project_file_path):
                print(f"File not found: {project_file_path}")
                continue  # Skip to next file

            # Open the project
            api_client.project.open_project_from_filepath(project_file_path)

            # Get active project ID
            project_id = api_client.project.active_project_id

            # Perform conversion
            default_mapping = api_client.conversion.get_conversion_mapping(project_id, CountryCode.AMERICAN)
            for css in default_mapping.cross_sections:
                css.target_value = css.source_value  # Keep same cross-section profile

            api_client.conversion.change_code(project_id, default_mapping)

            # Prepare output path
            output_file_path = os.path.abspath(f'./Models/Model_{i}_AISC.ideaCon')
            api_client.project.download_project(project_id, output_file_path)

            print(f"Model_{i} converted and saved to {output_file_path}")

    except Exception as e:
        print(f"Operation failed: {e}")
