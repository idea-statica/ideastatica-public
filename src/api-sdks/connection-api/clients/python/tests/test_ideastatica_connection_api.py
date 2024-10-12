import sys
import os
from pprint import pprint
from urllib.parse import urljoin

# Get the parent directory
parent_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))

# Add the parent directory to sys.path
sys.path.append(parent_dir)

import ideastatica_connection_api
import ideastatica_connection_api.ideastatica_client as ideastatica_client
from ideastatica_connection_api.models.con_project import ConProject


baseUrl = "http://localhost:5000"

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_connection_api.Configuration(
    host = baseUrl
)

dir_path = os.path.dirname(os.path.realpath(__file__))
project_file_path = os.path.join(dir_path, r'..\examples\projects', 'HSS_norm_cond.ideaCon')

iom_test_file_path = os.path.join(dir_path, r'..\examples\projects', 'OneConnectionImport.xml')

def test_should_open_ideacon():

    # Enter a context with an instance of the API client
    with ideastatica_client.IdeaStatiCaClient(configuration, project_file_path) as is_client:
        # Get the project data
        project_data = is_client.project.get_project_data(is_client.project_id)
        assert project_data.project_info.design_code == "ECEN"

def test_should_import_iom():
    # Enter a context with an instance of the API client
    with ideastatica_client.IdeaStatiCaClient(configuration, iom_test_file_path) as is_client:
        # Get the project data
        project_data = is_client.project.get_project_data(is_client.project_id)
        assert project_data.project_info.design_code == "ECEN"

def test_should_calculate():
    # Enter a context with an instance of the API client
    with ideastatica_client.IdeaStatiCaClient(configuration, project_file_path) as is_client:
        # Get the project data
        project_data = is_client.project.get_project_data(is_client.project_id)
        
        # run stress-strain CBFEM analysis for the connection id = 1
        calcParams = ideastatica_connection_api.ConCalculationParameter() # ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional)
        calcParams.connection_ids = [project_data.connections[0].id]

        # run stress-strain analysis for the connection
        con1_cbfem_results = is_client.calculation.calculate(is_client.project_id, calcParams)

        assert con1_cbfem_results, "con1_cbfem_results should not be empty"
