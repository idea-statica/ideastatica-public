import logging
import sys
import os
import json
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
project_file_path = os.path.join(dir_path, '..\projects', 'fatigue-check.ideaCon')
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

        # run stress-strain CBFEM analysis for the connection id = 1
        calcParams = ideastatica_connection_api.ConCalculationParameter() # ConCalculationParameter | List of connections to calculate and a type of CBFEM analysis (optional)
        calcParams.connection_ids = [connection1.id]    # calculate only connection1
        calcParams.analysis_type = "stress_Strain"      # obsolete solution (the analysis_type is taked from the connection now)
  

        # run stress-strain analysis for connection1
        con1_cbfem_results = api_client.calculation.calculate(api_client.project.active_project_id, calcParams.connection_ids)

        #results stress-strain analysis
        pprint(con1_cbfem_results)

        detailed_results = api_client.calculation.get_results(api_client.project.active_project_id, calcParams)
        pprint(detailed_results)

        # run Fatigue analysis for connection1

        # set analysis type stress-strain
        calcParams.analysis_type = "fatigues"
        connection1.analysis_type = calcParams.analysis_type
        updated_connection1 = api_client.connection.update_connection(api_client.project.active_project_id, connection1.id, connection1)

        if(connection1.analysis_type != updated_connection1.analysis_type):
            raise ValueError("Connection analysis type was not updated successfully.")

        # re-run analysis for the connection - now analysis_type is "fatigues"
        con1_cbfem_results = api_client.calculation.calculate(api_client.project.active_project_id, calcParams.connection_ids)

        # results Fatigue analysis
        pprint(con1_cbfem_results)

        fatiguedetailed_results = api_client.calculation.get_results(api_client.project.active_project_id, calcParams)
        pprint(fatiguedetailed_results)

        results_text = api_client.calculation.get_raw_json_results(api_client.project.active_project_id, calcParams)
        firstConnectionRawResult = results_text[0]
        pprint(firstConnectionRawResult)

    except Exception as e:
        print("Operation failed : %s\n" % e)

