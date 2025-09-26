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

dir_path = os.path.dirname(os.path.realpath(__file__))
project_file_path = os.path.join(dir_path, '..\projects', 'corner-empty.ideaCon')
print(project_file_path)


# Create client attached to already running service
with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
    try:
        # Open project
        uploadRes = api_client.project.open_project_from_filepath(project_file_path)

        # Get the project data
        project_data = api_client.project.get_project_data(api_client.project.active_project_id)
        pprint(project_data)

        # Get list of all connections in the project
        connections_in_project = api_client.connection.get_connections(api_client.project.active_project_id)

        # first connection in the project 
        connection1 = connections_in_project[0]
        pprint(connection1)

        templateParam =  ideastatica_connection_api.ConTemplateMappingGetParam() # ConTemplateMappingGetParam | Data of the template to get default mapping (optional)

        template_file_name = os.path.join(dir_path, '..\projects', 'template-I-corner.contemp')
        with open(template_file_name, 'r', encoding='utf-16') as file:
            templateParam.template = file.read()

        # get the default mapping for the selected template and connection  
        default_mapping = api_client.template.get_default_template_mapping(api_client.project.active_project_id, connection1.id, templateParam)
        pprint(default_mapping)

        # TODO
        # Modify mapping

        # Apply the template to the connection with the modified mapping
        applyTemplateData =  ideastatica_connection_api.ConTemplateApplyParam() # ConTemplateApplyParam | Template to apply (optional)
        applyTemplateData.connection_template = templateParam.template
        applyTemplateData.mapping = default_mapping

        applyTemplateResult = api_client.template.apply_template(api_client.project.active_project_id, connection1.id, applyTemplateData)

        pprint(applyTemplateResult)

        # run stress-strain CBFEM analysis for the connection id = 1
        calcParams =  ideastatica_connection_api.ConCalculationParameter() # ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional)
        calcParams.connection_ids = [connection1.id]

        # run stress-strain analysis for the connection
        con1_cbfem_results1 = api_client.calculation.calculate(api_client.project.active_project_id, calcParams.connection_ids)
        pprint(con1_cbfem_results1)

    except Exception as e:
        print("Operation failed : %s\n" % e)

