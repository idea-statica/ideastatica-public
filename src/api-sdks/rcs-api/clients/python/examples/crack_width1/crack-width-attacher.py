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
rcs_project_file_path = os.path.join(dir_path, r'..\..\projects', 'crack-width-example.IdeaRcs')


with rcs_api_service_attacher.RcsApiServiceAttacher(baseUrl).create_api_client() as api_client:
    # Open project
    uploadRes = api_client.project.open_project_from_file(rcs_project_file_path)

    # Get the project data
    project_data = api_client.project.get_active_project()

    sectId = project_data.sections[0].id
    secToCalculateIds = [sectId]
    calcParams = ideastatica_rcs_api.RcsCalculationParameters()
    calcParams.sections = secToCalculateIds

    # get loading
    sectionLoadingXml = api_client.internal_forces.get_section_loading(api_client.project.active_project_id, sectId)
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
    calc1_briefResults = api_client.calculation.calculate(api_client.project.active_project_id, calcParams)

    resultsParams = ideastatica_rcs_api.RcsResultParameters()
    resultsParams.sections = secToCalculateIds

    detail_results = api_client.calculation.get_results(api_client.project.active_project_id, resultsParams)

    detail_results1 = detail_results[0]

    sectionResultMap = detail_results_tools.get_section_result_map(detail_results)
    sect1_res = sectionResultMap[project_data.sections[0].id]

    capacities = []
    for sectId in secToCalculateIds:
        counter = 0
        for extreme in extremesInSection:
            capacity_res = detail_results_tools.get_result_by_type(sect1_res.extreme_results[counter], "capacity")
            fu = capacity_res['Fu']
            fu = capacity_res['Fu']
  
            # this is the max My bending moment for reinforced cross-section
            # fu_my =  float(capacity_res.Fu.Fy)
            # counter += 1