import logging
import sys
import os
from pprint import pprint
from urllib.parse import urljoin

# Get the parent directory
parent_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))

# Add the parent directory to sys.path
sys.path.append(parent_dir)

import ideastatica_connection_api.connection_api_service_attacher as connection_api_service_attacher

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

baseUrl = "http://localhost:5000"

dir_path = os.path.dirname(os.path.realpath(__file__))
project_file_path = os.path.join(dir_path, '..\projects', 'OneConnectionImport.xml')
print(project_file_path)

# Get the Downloads folder path
downloads_folder = os.path.join(os.path.expanduser('~'), 'Downloads')

# Create client attached to already running service
with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
    try:
        # Open project
        uploadRes = api_client.project.open_project_from_filepath(project_file_path)

        # Get the project data
        project_data = api_client.project.get_project_data(api_client.project.active_project_id)
        pprint(project_data)

        # Download project and save to the Downloads folder
        download_path = os.path.join(downloads_folder, "downloaded_project.ideaCon")
        api_client.project.download_project(api_client.project.active_project_id, download_path)

        logger.info(f"Project downloaded to: {download_path}")

    except Exception as e:
        print("Operation failed : %s\n" % e)