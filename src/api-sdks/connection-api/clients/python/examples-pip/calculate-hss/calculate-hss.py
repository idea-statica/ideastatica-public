import logging
import os
import json
from pprint import pprint
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
project_file_path = os.path.join(dir_path, '..\..\examples\projects', 'HSS_norm_cond.ideaCon')
print(project_file_path)

try:
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
    pprint(project_data)

    # Get list of all connections in the project
    connections_in_project = api_client.connection.get_connections(api_client.project_id)

    # first connection in the project 
    connection1 = connections_in_project[0]
    pprint(connection1)

    # run stress-strain CBFEM analysis for the connection id = 1
    calcParams = ideastatica_connection_api.ConCalculationParameter() # ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional)
    calcParams.connection_ids = [connection1.id]

    # run stress-strain analysis for the connection
    con1_cbfem_results = api_client.calculation.calculate(api_client.project_id, calcParams)
    pprint(con1_cbfem_results)

    # TODO - fix serialization JSON
    
    # get detailed results. Results are list of strings
    # the number of strings in the list correspond to the number of calculated connections
    results_text = api_client.calculation.get_raw_json_results(api_client.project_id, calcParams)
    firstConnectionResult = results_text[0];
    pprint(firstConnectionResult)

    raw_results = json.loads(firstConnectionResult)
    pprint(raw_results)

    detailed_results = api_client.calculation.get_results(api_client.project_id, calcParams)
    pprint(detailed_results)

    # get connection setup
    connection_setup =  api_client.project.get_setup(api_client.project_id)
    pprint(connection_setup)

    # modify setup
    connection_setup.hss_limit_plastic_strain = 0.02
    modifiedSetup = api_client.project.update_setup(api_client.project_id, connection_setup)

    # recalculate connection
    recalculate_results = api_client.calculation.calculate(api_client.project_id, calcParams)
    pprint(recalculate_results)

except Exception as e:
    print("Operation failed : %s\n" % e)
finally:
        # Close project
    api_client.project.close_project(api_client.project_id)
    api_client.project = None

