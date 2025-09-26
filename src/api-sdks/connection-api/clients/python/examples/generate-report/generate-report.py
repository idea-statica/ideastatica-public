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
from ideastatica_connection_api.rest import ApiException

# Verify that your service is running on following URL
baseUrl = "http://localhost:5000"

dir_path = os.path.dirname(os.path.realpath(__file__))
project_file_path = os.path.join(dir_path, '..\projects', 'HSS_norm_cond.ideaCon')
print(project_file_path)

# Create client attached to already running service
with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
    try:
        # Open project
        uploadRes = api_client.project.open_project_from_filepath(project_file_path)

        con_calculation_parameter = ideastatica_connection_api.ConCalculationParameter()
        con_calculation_parameter.connection_ids = [1]

        calc_Results = ideastatica_connection_api.CalculationApi(api_client.client)
        api_response = api_client.calculation.calculate(api_client.project.active_project_id, con_calculation_parameter.connection_ids)
        print("The response of CalculationApi->calculate:\n")
        pprint(calc_Results)      
                
        # will be solved in bug
        reportPdf = api_client.report.generate_pdf(api_client.project.active_project_id, 1)
        with open('output.pdf', 'wb') as file:
            file.write(reportPdf)
            
        pprint('PDF file saved successfully as output.pdf')
        
    except ApiException as e:
        print("Exception when calling CalculationApi->calculate: %s\n" % e)