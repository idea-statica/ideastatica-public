import os
import sys
from pprint import pprint
from urllib.parse import urljoin

# Get the parent directory
parent_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..'))

# Add the parent directory to sys.path
sys.path.append(parent_dir)


import ideastatica_rcs_api
import ideastatica_rcs_api.rcs_api_service_attacher as rcs_api_service_attacher
import ideastatica_rcs_api.helpers as helpers
import ideastatica_rcs_api.loading_tools as loading_tools
import ideastatica_rcs_api.brief_result_tools as brief_result_tools
import ideastatica_rcs_api.detail_results_tools as detail_results_tools

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

    # get loading
    sectionLoadingXml = api_client.internal_forces.get_section_loading(api_client.project.active_project_id,project_data.sections[0].id)
    sectionLoadingDict = helpers.xml_to_dict(sectionLoadingXml)

    #get extremes in section
    extremesInSection = loading_tools.get_extremes(sectionLoadingDict)
    for extreme in extremesInSection:
        print(f"Id: {extreme['Id']} Description: {extreme['Description']}")
        loads = extreme['Loads']['Loads']
        for inputLoad in loads['InputLoad']:
            internalForce = inputLoad['InternalForce']
            print(f"N {internalForce['N']}, Qy {internalForce['Qy']}, Qz {internalForce['Qz']}, Mx {internalForce['Mx']}, My {internalForce['My']}, Mz {internalForce['Mz']}, LoadType {inputLoad['LoadType']}, CombiType {inputLoad['CombiType']}")

    # run stress-strain analysis for the connection
    cal_results = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)
    brief_section_results = cal_results[0]

    capacity_check_val = brief_result_tools.get_check_value(brief_section_results, "capacity")

    print(f"Results section id : {brief_section_results.section_id}\n")
    for item in brief_section_results.overall_items:
        print(f"Status: {item.result_type} Check Value: {item.check_value}")

    resultsParams = ideastatica_rcs_api.RcsResultParameters()
    resultsParams.sections = [project_data.sections[0].id]

    detail_results = api_client.calculation.get_results(api_client.project.active_project_id, resultsParams)

    detail_results1 = detail_results[0]
    print(detail_results1.id)
    pprint(detail_results1.issues)

    sectionResultMap = detail_results_tools.get_section_result_map(detail_results)
    sect1_res = sectionResultMap[project_data.sections[0].id]

    counter = 0
    for extreme in extremesInSection:
        capacityCheckRes = detail_results_tools.get_result_by_type(sect1_res.extreme_results[counter], "capacity")
        print(f"Id: {extreme['Id']} Description: {extreme['Description']} Capacity Check Value: {capacityCheckRes.check_value}")
        counter += 1


