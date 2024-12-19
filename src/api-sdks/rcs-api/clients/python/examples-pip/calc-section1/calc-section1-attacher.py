import os
from pprint import pprint
from urllib.parse import urljoin
import ideastatica_rcs_api
import ideastatica_rcs_api.rcs_api_service_attacher as rcs_api_service_attacher

baseUrl = "http://localhost:5000"

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_rcs_api.Configuration(
    host = baseUrl
)

dir_path = os.path.dirname(os.path.realpath(__file__))
rcs_project_file_path = os.path.join(dir_path, r'..\..\projects', 'Project1.ideaRcs')


with rcs_api_service_attacher.RcsApiServiceAttacher(baseUrl).create_api_client() as api_client:
    # Open project
    uploadRes = api_client.project.open_project_from_file(rcs_project_file_path)

    # Get the project data
    project_data = api_client.project.get_active_project()

    calcParams = ideastatica_rcs_api.RcsCalculationParameters()
    calcParams.sections = [project_data.sections[0].id]

        # run stress-strain analysis for the connection
    cal_results = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)

    section_results = cal_results[0]

    print(f"Results section id : {section_results.section_id}\n")
    for item in section_results.overall_items:
        print(f"Status: {item.result_type} Check Value: {item.check_value}")

