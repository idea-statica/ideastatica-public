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

empty_corner_project_file_path = os.path.join(dir_path, r'..\examples\projects', 'corner-empty.ideaCon')

template_corner_file_name = os.path.join(dir_path, r'..\examples\projects', 'template-I-corner.contemp')

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

def test_should_apply_template():
    with ideastatica_client.IdeaStatiCaClient(configuration, empty_corner_project_file_path) as is_client:
        # Get the project data
        project_data = is_client.project.get_project_data(is_client.project_id)

        # Get list of all connections in the project
        connections_in_project = is_client.connection.get_connections(is_client.project_id)

        # first connection in the project 
        connection1 = connections_in_project[0]

        operations_before_template = is_client.operation.get_operations(is_client.project_id, connection1.id)
        assert len(operations_before_template) == 0, "There should be no operations before applying the template"

        templateParam =  ideastatica_connection_api.ConTemplateMappingGetParam() # ConTemplateMappingGetParam | Data of the template to get default mapping (optional)

        with open(template_corner_file_name, 'r', encoding='utf-16') as file:
            templateParam.template = file.read()

        # get the default mapping for the selected template and connection  
        default_mapping = is_client.template.get_default_template_mapping(is_client.project_id, connection1.id, templateParam)

        # Apply the template to the connection with the default mapping
        applyTemplateData =  ideastatica_connection_api.ConTemplateApplyParam() # ConTemplateApplyParam | Template to apply (optional)
        applyTemplateData.connection_template = templateParam.template
        applyTemplateData.mapping = default_mapping

        applyTemplateResult = is_client.template.apply_template(is_client.project_id, connection1.id, applyTemplateData)

        operations_after_template = is_client.operation.get_operations(is_client.project_id, connection1.id)
        assert len(operations_after_template) == 5, "There should be 5 operations before applying the template"
