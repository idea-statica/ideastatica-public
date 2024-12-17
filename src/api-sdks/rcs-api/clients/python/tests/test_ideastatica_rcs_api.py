import sys
import os
from pprint import pprint
from urllib.parse import urljoin


# Get the parent directory
parent_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))

# Add the parent directory to sys.path
sys.path.append(parent_dir)

import ideastatica_rcs_api
import ideastatica_rcs_api.rcs_api_service_attacher as rcs_api_service_attacher

baseUrl = "http://localhost:5000"

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_rcs_api.Configuration(
    host = baseUrl
)

dir_path = os.path.dirname(os.path.realpath(__file__))
rcs_project_file_path = os.path.join(dir_path, r'..\projects', 'Project1.ideaRcs')
iom_test_file_path = os.path.join(dir_path, r'..\projects', 'ImportOpenModel.xml')

def test_should_open_idearcs():
    # Create client attached to already running service
    with rcs_api_service_attacher.RcsApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        uploadRes = api_client.project.open_project_from_file(rcs_project_file_path)

        # Get the project data
        project_data = api_client.project.get_active_project()
        assert project_data.project_data.code == "ECEN"

def test_should_import_iom():
    # Create client attached to already running service
    with rcs_api_service_attacher.RcsApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        uploadRes = api_client.project.create_project_from_iom_file(iom_test_file_path)

        # Get the project data
        project_data = api_client.project.get_active_project()
        assert project_data.project_data.code == "ECEN"

def test_should_calculate():
    # Create client attached to already running service
    with rcs_api_service_attacher.RcsApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        uploadRes = api_client.project.open_project_from_file(rcs_project_file_path)

        # Get the project data
        project_data = api_client.project.get_active_project()

        calcParams = ideastatica_rcs_api.RcsCalculationParameters()
        calcParams.sections = [project_data.sections[0].id]

        # run stress-strain analysis for the connection
        cal_results = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)
        
        assert cal_results, "cal_results should not be empty"


