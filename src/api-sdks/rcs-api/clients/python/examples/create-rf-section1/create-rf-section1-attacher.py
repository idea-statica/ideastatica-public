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
import ideastatica_rcs_api.raw_results_tools as raw_results_tools

baseUrl = "http://localhost:5000"

# Defining the host is optional and defaults to http://localhost
# See configuration.py for a list of all supported configuration parameters.
configuration = ideastatica_rcs_api.Configuration(
    host = baseUrl
)

dir_path = os.path.dirname(os.path.realpath(__file__))
rcs_project_file_path = os.path.join(dir_path, r'..\..\projects', 'Project1.ideaRcs')
rcs_template_file_path = os.path.join(dir_path, r'..\..\projects', 'rect-L-4-2.nav')

with rcs_api_service_attacher.RcsApiServiceAttacher(baseUrl).create_api_client() as api_client:
    # Open project
    uploadRes = api_client.project.open_project_from_file(rcs_project_file_path)

    # Get the project data
    project_data = api_client.project.get_active_project()

    print("Existing reinforced cross-sections:")
    for rcs in project_data.reinforced_cross_sections:
        print(f"Id: {rcs.id} Description: {rcs.name}")

    importParams = ideastatica_rcs_api.RcsReinforcedCrosssSectionImportSetting()
    importData = ideastatica_rcs_api.RcsReinforcedCrossSectionImportData()
    importData.setting = importParams

    with open(rcs_template_file_path, 'r', encoding='utf-8') as file:
        importData.template = file.read()

    apply_res = api_client.cross_section.import_reinforced_cross_section(api_client.project.active_project_id, importData)

    # Get the project data
    project_data = api_client.project.get_active_project()

    print("Existing reinforced cross-sections after import:")
    for rcs in project_data.reinforced_cross_sections:
        print(f"Id: {rcs.id} Description: {rcs.name}")        