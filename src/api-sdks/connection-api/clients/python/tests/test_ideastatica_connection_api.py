import sys
import os
from pprint import pprint
from urllib.parse import urljoin

# Get the parent directory
parent_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))

# Add the parent directory to sys.path
sys.path.append(parent_dir)

import ideastatica_connection_api
import ideastatica_connection_api.connection_api_service_attacher as connection_api_service_attacher


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
    # Create client attached to already running service
    attacher = connection_api_service_attacher.ConnectionApiServiceAttacher(configuration)
    api_client = attacher.create_api_client()

    # Open project
    with open(project_file_path, 'rb') as file:
            byte_array = file.read()
    uploadRes = api_client.project.open_project(idea_con_file=byte_array, _content_type='multipart/form-data')
    api_client.project_id = uploadRes.project_id

    # Get the project data
    project_data = api_client.project.get_project_data(api_client.project_id)

    # Close project
    api_client.project.close_project(api_client.project_id)
    api_client.project = None
    assert project_data.project_info.design_code == "ECEN"

def test_should_import_iom():
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

    # Close project
    api_client.project.close_project(api_client.project_id)
    api_client.project = None
    assert project_data.project_info.design_code == "ECEN"

def test_should_calculate():
    # Create client attached to already running service
    attacher = connection_api_service_attacher.ConnectionApiServiceAttacher(configuration)
    api_client = attacher.create_api_client()


    # Open project
    with open(project_file_path, 'rb') as file:
            byte_array = file.read()
    uploadRes = api_client.project.open_project(idea_con_file=byte_array, _content_type='multipart/form-data')
    api_client.project_id = uploadRes.project_id

    # Get the project data
    project_data = api_client.project.get_project_data(api_client.project_id)
    
    # run stress-strain CBFEM analysis for the connection id = 1
    calcParams = ideastatica_connection_api.ConCalculationParameter() # ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional)
    calcParams.connection_ids = [project_data.connections[0].id]

    # run stress-strain analysis for the connection
    con1_cbfem_results = api_client.calculation.calculate(api_client.project_id, calcParams)

    # Close project
    api_client.project.close_project(api_client.project_id)
    api_client.project = None
    
    assert con1_cbfem_results, "con1_cbfem_results should not be empty"

def test_should_apply_template():
    # Create client attached to already running service
    attacher = connection_api_service_attacher.ConnectionApiServiceAttacher(configuration)
    api_client = attacher.create_api_client()

    # Open project
    with open(project_file_path, 'rb') as file:
            byte_array = file.read()
    uploadRes = api_client.project.open_project(idea_con_file=byte_array, _content_type='multipart/form-data')
    api_client.project_id = uploadRes.project_id

    # Get the project data
    project_data = api_client.project.get_project_data(api_client.project_id)

    # Get list of all connections in the project
    connections_in_project = api_client.connection.get_connections(api_client.project_id)

    # first connection in the project 
    connection1 = connections_in_project[0]

    operations_before_template = api_client.operation.get_operations(api_client.project_id, connection1.id)
    assert len(operations_before_template) == 0, "There should be no operations before applying the template"

    templateParam =  ideastatica_connection_api.ConTemplateMappingGetParam() # ConTemplateMappingGetParam | Data of the template to get default mapping (optional)

    with open(template_corner_file_name, 'r', encoding='utf-16') as file:
        templateParam.template = file.read()

    # get the default mapping for the selected template and connection  
    default_mapping = api_client.template.get_default_template_mapping(api_client.project_id, connection1.id, templateParam)

    # Apply the template to the connection with the default mapping
    applyTemplateData =  ideastatica_connection_api.ConTemplateApplyParam() # ConTemplateApplyParam | Template to apply (optional)
    applyTemplateData.connection_template = templateParam.template
    applyTemplateData.mapping = default_mapping

    applyTemplateResult = api_client.template.apply_template(api_client.project_id, connection1.id, applyTemplateData)

    operations_after_template = api_client.operation.get_operations(api_client.project_id, connection1.id)

    # Close project
    api_client.project.close_project(api_client.project_id)
    api_client.project = None

    assert len(operations_after_template) == 5, "There should be 5 operations before applying the template"
