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
    with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        uploadRes = api_client.project.open_project_from_filepath(project_file_path)

        # Get the project data
        project_data = api_client.project.get_project_data(api_client.project.active_project_id)
        assert project_data.project_info.design_code == "ECEN"

def test_should_import_iom():
    # Create client attached to already running service
    with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        uploadRes = api_client.project.open_project_from_filepath(iom_test_file_path)

        # Get the project data
        project_data = api_client.project.get_project_data(api_client.project.active_project_id)
        assert project_data.project_info.design_code == "ECEN"

def test_should_calculate():
    # Create client attached to already running service
    with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        uploadRes = api_client.project.open_project_from_filepath(project_file_path)

        # Get the project data
        project_data = api_client.project.get_project_data(api_client.project.active_project_id)
        
        # run stress-strain CBFEM analysis for the connection id = 1
        calcParams = ideastatica_connection_api.ConCalculationParameter() # ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional)
        calcParams.connection_ids = [project_data.connections[0].id]

        # run stress-strain analysis for the connection
        con1_cbfem_results = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)
        
        assert con1_cbfem_results, "con1_cbfem_results should not be empty"

def test_should_apply_template():
     # Create client attached to already running service
    with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        uploadRes = api_client.project.open_project_from_filepath(empty_corner_project_file_path)

        # Get the project data
        project_data = api_client.project.get_project_data(api_client.project.active_project_id)

        # Get list of all connections in the project
        connections_in_project = api_client.connection.get_connections(api_client.project.active_project_id)

        # first connection in the project 
        connection1 = connections_in_project[0]

        operations_before_template = api_client.operation.get_operations(api_client.project.active_project_id, connection1.id)
        assert len(operations_before_template) == 0, "There should be no operations before applying the template"

        templateParam =  ideastatica_connection_api.ConTemplateMappingGetParam() # ConTemplateMappingGetParam | Data of the template to get default mapping (optional)

        with open(template_corner_file_name, 'r', encoding='utf-16') as file:
            templateParam.template = file.read()

        # get the default mapping for the selected template and connection  
        default_mapping = api_client.template.get_default_template_mapping(api_client.project.active_project_id, connection1.id, templateParam)

        # Apply the template to the connection with the default mapping
        applyTemplateData =  ideastatica_connection_api.ConTemplateApplyParam() # ConTemplateApplyParam | Template to apply (optional)
        applyTemplateData.connection_template = templateParam.template
        applyTemplateData.mapping = default_mapping

        applyTemplateResult = api_client.template.apply_template(api_client.project.active_project_id, connection1.id, applyTemplateData)

        operations_after_template = api_client.operation.get_operations(api_client.project.active_project_id, connection1.id)

        assert len(operations_after_template) == 5, "There should be 5 operations before applying the template"


def test_should_export_ifc():
    # Create client attached to already running service
    with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        upload_res = api_client.project.open_project_from_filepath(project_file_path)

        # Get list of all connections in the project
        connections_in_project = api_client.connection.get_connections(api_client.project.active_project_id)

        # first connection in the project
        connection1 = connections_in_project[0]

        # export IFC file
        export_file_name = os.path.join(dir_path, r'..\examples\projects', 'test-export.ifc')
        api_client.export.export_ifc_file(api_client.project.active_project_id, connection1.id, export_file_name)

        assert os.path.exists(export_file_name), f"Exported IFC file {export_file_name} does not exist"

        # Clean up
        os.remove(export_file_name)


def test_should_generate_pdf_report():
    # Create client attached to already running service
    with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
        # Open project
        upload_res = api_client.project.open_project_from_filepath(project_file_path)

        # Get list of all connections in the project
        connections_in_project = api_client.connection.get_connections(api_client.project.active_project_id)

        # first connection in the project
        connection1 = connections_in_project[0]

        # generate PDF report
        pdf_file_name = os.path.join(dir_path, r'..\examples\projects', 'test-report.pdf')
        api_client.report.save_report_pdf(api_client.project.active_project_id, connection1.id, pdf_file_name)

        assert os.path.exists(pdf_file_name), f"Generated PDF report {pdf_file_name} does not exist"

        # Clean up
        os.remove(pdf_file_name)