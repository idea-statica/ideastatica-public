import logging
import sys
import os
from pprint import pprint
from urllib.parse import urljoin

# Get the parent directory
parent_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))

# Add the parent directory to sys.path
sys.path.append(parent_dir)

import ideastatica_connection_api
import ideastatica_connection_api.connection_api_service_attacher as connection_api_service_attacher

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

baseUrl = "http://localhost:5000"

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_connection_api.Configuration(
    host = baseUrl
)

dir_path = os.path.dirname(os.path.realpath(__file__))
project_file_path = os.path.join(dir_path, '..\projects', 'OneConnectionImport.xml')
print(project_file_path)

# Get the Downloads folder path
downloads_folder = os.path.join(os.path.expanduser('~'), 'Downloads')

try:
    # Create client attached to already running service
    attacher = connection_api_service_attacher.ConnectionApiServiceAttacher(configuration)
    api_client = attacher.create_api_client()

    # Open project
    with open(project_file_path, 'rb') as file:
            byte_array = file.read()
    uploadRes = api_client.project.import_iom(byte_array, _content_type='multipart/form-data')
    api_client.project_id = uploadRes.project_id

    # Get the project data
    project_data = api_client.project.get_project_data(api_client.project_id)
    pprint(project_data)

    # Download project and save to the Downloads folder
    download_path = os.path.join(downloads_folder, "downloaded_project.ideaCon")
    api_client.project.download_project(api_client.project_id, download_path)

    logger.info(f"Project downloaded to: {download_path}")

except Exception as e:
    print("Operation failed : %s\n" % e)
finally:
        # Close project
    api_client.project.close_project(api_client.project_id)
    api_client.project = None