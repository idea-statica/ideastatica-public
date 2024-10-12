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
import ideastatica_connection_api.ideastatica_client as ideastatica_client
from ideastatica_connection_api.rest import ApiException

# Verify that your service is running on following URL
baseUrl = "http://localhost:5000"

dir_path = os.path.dirname(os.path.realpath(__file__))
project_file_path = os.path.join(dir_path, '..\projects', 'HSS_norm_cond.ideaCon')
print(project_file_path)

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_connection_api.Configuration(
    host = baseUrl
)

# Enter a context with an instance of the API client
with ideastatica_client.IdeaStatiCaClient(configuration, project_file_path) as is_client:

    try:
        con_calculation_parameter = ideastatica_connection_api.ConCalculationParameter()
        con_calculation_parameter.connection_ids = [1]

        calc_Results = ideastatica_connection_api.CalculationApi(is_client.client)
        api_response = is_client.calculation.calculate(is_client.project_id, con_calculation_parameter)
        print("The response of CalculationApi->calculate:\n")
        pprint(calc_Results)      
                
        # will be solved in bug
        reportPdf = is_client.report.generate_pdf(is_client.project_id, 1)
        with open('output.pdf', 'wb') as file:
            file.write(reportPdf)
            
        pprint('PDF file saved successfully as output.pdf')
        
    except ApiException as e:
        print("Exception when calling CalculationApi->calculate: %s\n" % e)